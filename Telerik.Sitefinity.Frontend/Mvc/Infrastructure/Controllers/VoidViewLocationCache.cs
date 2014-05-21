using System.Web;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class is used to disable view location cache for controllers.
    /// </summary>
    internal class VoidViewLocationCache : IViewLocationCache
    {
        /// <summary>
        /// Gets the view location by using the specified HTTP context and the cache key.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="key">The cache key.</param>
        /// <returns>
        /// The view location.
        /// </returns>
        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            return null;
        }

        /// <summary>
        /// Inserts the specified view location into the cache by using the specified HTTP context and the cache key.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="key">The cache key.</param>
        /// <param name="virtualPath">The virtual path.</param>
        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            //Do nothing. We don't want cache.
        }
    }
}
