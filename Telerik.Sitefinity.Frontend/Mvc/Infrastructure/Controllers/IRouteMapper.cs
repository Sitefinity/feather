using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Classes that implement this interface can provide custom routing logic.
    /// </summary>
    public interface IRouteMapper
    {
        /// <summary>
        /// Maps the route parameters from URL and returns true of the URL is a valid route.
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="requestContext">The request context.</param>
        /// <returns>True if the URL is a valid route. False otherwise.</returns>
        bool TryMapRouteParameters(string[] urlParams, RequestContext requestContext);
    }
}
