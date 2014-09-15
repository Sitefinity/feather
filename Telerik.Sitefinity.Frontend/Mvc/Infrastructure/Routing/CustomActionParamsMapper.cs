using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class map URL parameters to Route Data following a provided route template and action name.
    /// </summary>
    internal class CustomActionParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="routeTemplateResolver">This function should return the route template that the mapper will use.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentNullException">routeTemplateResolver</exception>
        public CustomActionParamsMapper(ControllerBase controller, Func<string> routeTemplateResolver, string actionName = CustomActionParamsMapper.DefaultActionName)
            : base(controller)
        {
            if (routeTemplateResolver == null)
                throw new ArgumentNullException("routeTemplateResolver");

            this.actionName = actionName;
            this.actionMethod = controller.GetType().GetMethod(this.actionName, BindingFlags.Instance | BindingFlags.Public);
            this.routeTemplateResolver = routeTemplateResolver;
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext)
        {
            if (urlParams == null)
                return false;

            var routeTemplate = this.routeTemplateResolver();

            var metaParams = routeTemplate
                .Split(new []{'/'}, StringSplitOptions.RemoveEmptyEntries);

            var parameterMap = this.MapParams(this.actionMethod, metaParams, urlParams);

            if (parameterMap == null)
                return false;

            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;
            this.PopulateRouteData(requestContext.RouteData.Values, parameterMap);
            RouteHelper.SetUrlParametersResolved();

            return true;
        }

        private void PopulateRouteData(RouteValueDictionary routeDataValues, IList<UrlSegmentInfo> parameters)
        {
            foreach (var parameter in parameters)
            {
                routeDataValues[parameter.ParameterName] = parameter.ParameterValue;
            }
        }

        private string actionName;
        private MethodInfo actionMethod;
        private Func<string> routeTemplateResolver;
        private const string DefaultActionName = "Index";
    }
}
