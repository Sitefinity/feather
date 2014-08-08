using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Classes that implement this interface should provide logic that maps URL parameters to route data.
    /// </summary>
    public interface IUrlMappingController
    {
        /// <summary>
        /// Gets the URL parameters mapper.
        /// </summary>
        /// <value>
        /// The URL parameters mapper.
        /// </value>
        IUrlParamsMapper UrlParamsMapper { get; }
    }
}
