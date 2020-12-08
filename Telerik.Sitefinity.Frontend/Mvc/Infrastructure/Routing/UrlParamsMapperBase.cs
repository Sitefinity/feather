using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;

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

        /// <summary>
        /// Gets a value indicating whether to require parameter naming in the widget routings.
        /// </summary>
        /// <value>
        /// <c>true</c> if the routes will work only with named params (e.g /tag/sofia/page/2); otherwise, <c>false</c> when the route will be /sofia/2.
        /// </value>
        public static bool UseNamedParametersRouting
        {
            get
            {
                return Config.Get<FeatherConfig>().UseNamedParametersRouting;
            }
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
            this.ResolveUrlParams(urlParams, requestContext, null);
        }

        /// <inheritdoc />
        public void ResolveUrlParams(string[] urlParams, RequestContext requestContext, string urlKeyPrefix)
        {
            var isMatch = this.TryMatchUrl(urlParams, requestContext, urlKeyPrefix);

            if (!isMatch && this.Next != null)
                this.Next.ResolveUrlParams(urlParams, requestContext, urlKeyPrefix);
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
        /// <param name="urlParamNames">The URL parameter names.</param>
        /// <returns></returns>
        protected virtual IList<UrlSegmentInfo> MapParams(MethodInfo actionMethod, string[] metaParams, string[] urlParams, string[] urlParamNames)
        {
            var useNamedParams = UrlParamsMapperBase.UseNamedParametersRouting;
            if (!useNamedParams && (metaParams.Length != urlParams.Length || metaParams.Length != urlParamNames.Length))
                return null;
            if (useNamedParams && ((2 * metaParams.Length) != urlParams.Length || metaParams.Length != urlParamNames.Length))
                return null;

            var parameterInfos = new List<UrlSegmentInfo>();

            for (int i = 0; i < urlParamNames.Length; i++)
            {
                string currentParam = useNamedParams ? this.MapNamedParam(urlParams, urlParamNames[i], i) : urlParams[i];

                if (metaParams[i].Length > 2 && metaParams[i].First() == '{' && metaParams[i].Last() == '}')
                {
                    var routeParam = metaParams[i].Sub(1, metaParams[i].Length - 2);
                    routeParam = useNamedParams ? this.MapNamedRouteParam(urlParams, urlParamNames[i], i, routeParam) : routeParam;
                    if (!this.TryResolveRouteParam(actionMethod, routeParam, currentParam, parameterInfos))
                        return null;
                }
                else if (!string.Equals(metaParams[i], currentParam, StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
            }

            return parameterInfos;
        }

        /// <summary>
        /// Maps the URL parameter to a value from the provided URL template
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="urlParamName">The URL parameter name.</param>
        /// <param name="paramNameIndex">The index of the named parameter in the URL.</param>
        /// <returns></returns>
        protected virtual string MapNamedParam(string[] urlParams, string urlParamName, int paramNameIndex)
        {
            string currentParam;
            var urlParamActualIndex = 2 * paramNameIndex;
            if (urlParamName != FeatherActionInvoker.TaxonNamedParamter)
            {
                var namedParamIndex = Array.IndexOf(urlParams, urlParamName);
                if (namedParamIndex == -1 || urlParams.Length < namedParamIndex)
                    return null;

                currentParam = urlParams[namedParamIndex + 1];
            }
            else if (urlParamName == FeatherActionInvoker.TaxonNamedParamter && urlParams.Length > (urlParamActualIndex + 1))
            {
                // in this case, in the url will be presented the name of the taxonomy, ex. tag/tag1/page/2 ;  category/cat1/page/2
                currentParam = urlParams[urlParamActualIndex + 1];
                var taxonomyName = urlParams[urlParamActualIndex];

                if (taxonomyName.ToLowerInvariant() == "categories")
                    taxonomyName = "category";
                if (taxonomyName.ToLowerInvariant() == "tags")
                    taxonomyName = "tag";

                var isResolverRegistered = ObjectFactory.IsTypeRegistered<IRouteParamResolver>(taxonomyName.ToLowerInvariant());
                if (!isResolverRegistered)
                    return null;
            }
            else
                return null;

            return currentParam;
        }

        /// <summary>
        /// Maps the classification URL parameter to a value from the provided URL template
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="urlParamName">The URL parameter name.</param>
        /// <param name="paramNameIndex">The index of the named parameter in the URL.</param>
        /// <param name="routeParam">The value of the route param from meta params.</param>
        /// <returns></returns>
        protected virtual string MapNamedRouteParam(string[] urlParams, string urlParamName, int paramNameIndex, string routeParam)
        {
            var urlParamActualIndex = 2 * paramNameIndex;
            if (routeParam.IndexOf(":") > 0 && urlParamName == FeatherActionInvoker.TaxonNamedParamter && urlParams.Length > (urlParamActualIndex + 1))
            {
                // in this case, in the url will be presented the name of the taxonomy, ex. tag/tag1/page/2 ;  category/cat1/page/2
                var taxonomyName = urlParams[urlParamActualIndex];

                if (taxonomyName.ToLowerInvariant() == "categories")
                    taxonomyName = "category";
                if (taxonomyName.ToLowerInvariant() == "tags")
                    taxonomyName = "tag";

                var isResolverRegistered = ObjectFactory.IsTypeRegistered<IRouteParamResolver>(taxonomyName.ToLowerInvariant());
                if (!isResolverRegistered)
                    return null;

                var parts = routeParam.Split(':');
                return parts[0] + ":" + taxonomyName;
            }

            return routeParam;
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
        /// <param name="parameterInfos">The parameter info.</param>
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
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <returns>true if resolving was successful. In this case does not fallback to next mappers. Else returns false</returns>
        protected abstract bool TryMatchUrl(string[] urlParams, RequestContext requestContext, string urlKeyPrefix);

        /// <summary>
        /// The action name key for the RouteData values.
        /// </summary>
        protected const string ActionNameKey = "action";

        private IUrlParamsMapper next;
    }
}
