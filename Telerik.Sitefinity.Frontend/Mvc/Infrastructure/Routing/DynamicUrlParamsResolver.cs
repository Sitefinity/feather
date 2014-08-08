using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class provide logic for custom routing of <see cref="IUrlMappingController" /> controllers.
    /// </summary>
    public class DynamicUrlParamsResolver : IMvcUrlParamsResolver
    {
        /// <inheritdoc />
        public void ResolveUrlParams(string[] urlParams, RequestContext requestContext, Controller controller, string defaultControllerName)
        {
            RouteHelper.SetUrlParametersResolved(false);

            var urlController = controller as IUrlMappingController;
            if (urlController != null)
            {
                requestContext.RouteData.Values[DynamicUrlParamsResolver.ControllerNameKey] = defaultControllerName;
                urlController.UrlParamsMapper.ResolveUrlParams(urlParams, requestContext);
            }
            else
            {
                ObjectFactory.Container.Resolve<IMvcUrlParamsResolver>(DynamicUrlParamsResolver.DefaultResolverName).ResolveUrlParams(urlParams, requestContext, controller, defaultControllerName);
            }
        }

        public const string DefaultResolverName = "default";
        public const string ControllerNameKey = "controller";
    }
}
