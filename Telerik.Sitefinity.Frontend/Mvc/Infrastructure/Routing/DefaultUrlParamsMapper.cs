using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class call the default Sitefinity logic for mapping URL paramters to route data.
    /// </summary>
    public class DefaultUrlParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultUrlParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DefaultUrlParamsMapper(Controller controller)
            : base(controller)
        {

        }

        /// <inheritdoc />
        protected override void ResolveUrlParamsInternal(string[] urlParams, RequestContext requestContext)
        {
            var controllerName = requestContext.RouteData.Values[DynamicUrlParamsResolver.ControllerNameKey] as string;
            requestContext.RouteData.Values.Remove(DynamicUrlParamsResolver.ControllerNameKey);
            ObjectFactory.Resolve<IMvcUrlParamsResolver>(DynamicUrlParamsResolver.DefaultResolverName)
                .ResolveUrlParams(urlParams, requestContext, this.Controller, controllerName);
        }
    }
}
