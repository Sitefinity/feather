using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.HttpContext
{
    /// <summary>
    /// This class represents fake <see cref="HttpResponseBase"/> for unit testing.
    /// </summary>
    public class DummyHttpResponse : HttpResponseBase
    {
        /// <summary>
        /// When overridden in a derived class, adds a session ID to the virtual path if the session is using <see cref="P:System.Web.Configuration.SessionStateSection.Cookieless" /> session state, and returns the combined path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of a resource.</param>
        /// <returns>
        /// The virtual path, with the session ID inserted.
        /// </returns>
        public override string ApplyAppPathModifier(string virtualPath)
        {
            return virtualPath;
        }

        /// <summary>
        /// When overridden in a derived class, associates cache dependencies with the response that enable the response to be invalidated if it is cached and if the specified dependencies change.
        /// </summary>
        /// <param name="dependencies">A file, cache key, or <see cref="T:System.Web.Caching.CacheDependency" /> object to add to the list of application dependencies.</param>
        public override void AddCacheDependency(params CacheDependency[] dependencies)
        {
            foreach (var dependency in dependencies)
                this.CacheDependencies.Add(dependency);
        }

        /// <summary>
        /// Gets the cache dependencies that are added to the response.
        /// </summary>
        public IList<CacheDependency> CacheDependencies 
        { 
            get
            {
                return this.cacheDependencies;
            }
        }

        private IList<CacheDependency> cacheDependencies = new List<CacheDependency>();
    }
}
