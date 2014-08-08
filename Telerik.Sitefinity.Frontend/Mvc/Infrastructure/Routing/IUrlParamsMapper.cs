using System.Web.Mvc;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Classes that implement this interface should provide responsibility chain logic for mapping URL parameters to route data.
    /// </summary>
    public interface IUrlParamsMapper
    {
        /// <summary>
        /// Gets the next URL parameter mapper.
        /// </summary>
        /// <value>
        /// The next URL parameter mapper.
        /// </value>
        IUrlParamsMapper Next { get; }

        /// <summary>
        /// Sets the next URL parameter mapper.
        /// </summary>
        /// <param name="nextMapper">The next mapper.</param>
        /// <returns></returns>
        IUrlParamsMapper SetNext(IUrlParamsMapper nextMapper);

        /// <summary>
        /// Resolves the URL parameters. Should call RouteHelper.SetUrlParametersResolved if the mapping was successful.
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="requestContext">The request context.</param>
        void ResolveUrlParams(string[] urlParams, RequestContext requestContext);
    }
}
