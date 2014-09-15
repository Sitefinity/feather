using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// A base URL params mapper that provides responsibility chain logic.
    /// </summary>
    internal abstract class UrlParamsMapperBase : IUrlParamsMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlParamsMapperBase"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public UrlParamsMapperBase(ControllerBase controller)
        {
            this.Controller = controller;
        }

        /// <inheritdoc />
        public IUrlParamsMapper Next
        {
            get { return this.next; }
        }

        /// <inheritdoc />
        public IUrlParamsMapper SetNext(IUrlParamsMapper nextResolver)
        {
            this.next = nextResolver;
            return this.Next;
        }

        /// <inheritdoc />
        public void ResolveUrlParams(string[] urlParams, RequestContext requestContext)
        {
            var isMatch = this.TryMatchUrl(urlParams, requestContext);

            if (!isMatch && this.Next != null)
                this.Next.ResolveUrlParams(urlParams, requestContext);
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        protected ControllerBase Controller { get; private set; }

        /// <summary>
        /// Creates parameter map in order to map the URL parameters to the provided URL template
        /// </summary>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="metaParams">The meta parameters.</param>
        /// <param name="urlParams">The URL parameters.</param>
        /// <returns></returns>
        protected virtual IList<UrlSegmentInfo> MapParams(MethodInfo actionMethod, string[] metaParams, string[] urlParams)
        {
            if (metaParams.Length != urlParams.Length)
                return null;

            var parameterInfos = new List<UrlSegmentInfo>();
            for (int i = 0; i < urlParams.Length; i++)
            {
                if (metaParams[i].Length > 2 && metaParams[i].First() == '{' && metaParams[i].Last() == '}')
                {
                    var routeParam = metaParams[i].Sub(1, metaParams[i].Length - 2);
                    if (!this.TryResolveRouteParam(actionMethod, routeParam, urlParams[i], parameterInfos))
                        return null;
                }
                else if (!string.Equals(metaParams[i], urlParams[i], StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            return parameterInfos;
        }

        /// <summary>
        /// Tries to resolve route parameters and map them to specific part of the url.
        /// </summary>
        /// <remarks>
        /// If needed tries to interpret the route value through <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing.IRouteParamResolver"/>
        /// </remarks>
        /// <param name="actionMethod">The action method.</param>
        /// <param name="routeParam">The route parameter.</param>
        /// <param name="urlParam">The URL parameter.</param>
        /// <param name="parameterMap">The parameter map.</param>
        /// <returns></returns>
        protected bool TryResolveRouteParam(MethodInfo actionMethod, string routeParam, string urlParam, IList<UrlSegmentInfo> parameterInfos)
        {
            if (routeParam.IsNullOrEmpty())
                return false;

            var parts = routeParam.Split(':');
            var paramName = parts[0];

            object paramValue = urlParam;

            if (parts.Length >= 2)
            {
                var actionParam = actionMethod.GetParameters().FirstOrDefault(p => p.Name == paramName);
                if (actionParam != null)
                {
                    var routeConstraints = parts[1]
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => r.Trim());

                    IRouteParamResolver first = null;
                    IRouteParamResolver last = null;
                    foreach (var resolverName in routeConstraints)
                    {
                        var resolver = ObjectFactory.Resolve<IRouteParamResolver>(resolverName);
                        if (resolver != null)
                        {
                            if (last != null)
                            {
                                last = last.SetNext(resolver);
                            }
                            else
                            {
                                first = last = resolver;
                            }
                        }
                    }

                    if (first == null || first.TryResolveParam(urlParam, out paramValue) == false)
                        return false;
                }
            }

            var paramInfo = new UrlSegmentInfo();
            paramInfo.ParameterName = paramName;

            if (parts.Length >= 2)
            {
                paramInfo.ResolverName = parts[1];
            }

            paramInfo.ParameterValue = paramValue;
            parameterInfos.Add(paramInfo);

            return true;
        }

        /// <summary>
        /// Matches the URL parameters.
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns>true if resolving was successful. In this case does not fallback to next mappers. Else returns false</returns>
        protected abstract bool TryMatchUrl(string[] urlParams, RequestContext requestContext);

        /// <summary>
        /// The action name key for the RouteData values.
        /// </summary>
        protected const string ActionNameKey = "action";

        private IUrlParamsMapper next;
    }
}
