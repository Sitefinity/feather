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
    /// Instances of this class map URL parameters to Route Data following a provided route template and action name.
    /// </summary>
    public class CustomActionParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="routeTemplateResolver">This function should return the route template that the mapper will use.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentNullException">routeTemplateResolver</exception>
        public CustomActionParamsMapper(Controller controller, Func<string> routeTemplateResolver, string actionName = CustomActionParamsMapper.DefaultActionName)
            : base(controller)
        {
            if (routeTemplateResolver == null)
                throw new ArgumentNullException("routeTemplateResolver");

            this.actionName = actionName;
            this.actionMethod = controller.GetType().GetMethod(this.actionName, BindingFlags.Instance | BindingFlags.Public);
            this.routeTemplateResolver = routeTemplateResolver;
        }

        /// <inheritdoc />
        protected override void ResolveUrlParamsInternal(string[] urlParams, RequestContext requestContext)
        {
            if (urlParams == null)
                return;

            var routeTemplate = routeTemplateResolver();

            var metaParams = routeTemplate
                .Split(new [] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (metaParams.Length != urlParams.Length)
                return;

            var parameterMap = new Dictionary<string, object>();
            for (int i = 0; i < urlParams.Length; i++)
            {
                if (metaParams[i].Length > 2 && metaParams[i].First() == '{' && metaParams[i].Last() == '}')
                {
                    var routeParam = metaParams[i].Sub(1, metaParams[i].Length - 2);
                    if (!this.TryResolveRouteParam(routeParam, urlParams[i], parameterMap))
                        return;
                }
                else if (!string.Equals(metaParams[i], urlParams[i], StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;
            this.PopulateRouteData(requestContext.RouteData.Values, parameterMap);
            RouteHelper.SetUrlParametersResolved();
        }

        private bool TryResolveRouteParam(string routeParam, string urlParam, IDictionary<string, object> parameterMap)
        {
            if (routeParam.IsNullOrEmpty())
                return false;

            var parts = routeParam.Split(':');
            var paramName = parts[0];
            object paramValue = urlParam;

            if (parts.Length >= 2)
            {
                var actionParam = this.actionMethod.GetParameters().FirstOrDefault(p => p.Name == paramName);
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

            parameterMap[paramName] = paramValue;
            return true;
        }

        private void PopulateRouteData(RouteValueDictionary routeDataValues, IDictionary<string, object> parameters)
        {
            foreach (var parameter in parameters)
            {
                routeDataValues[parameter.Key] = parameter.Value;
            }
        }

        private string actionName;
        private MethodInfo actionMethod;
        private Func<string> routeTemplateResolver;
        private const string DefaultActionName = "Index";
    }
}
