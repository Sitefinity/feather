using System;
using System.Web.Caching;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    /// <summary>
    /// This class inherits <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes.CacheDependentAttribute"/> for test purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class DummyCacheDependentAttribute : CacheDependentAttribute
    {
        /// <summary>
        /// Gets the cache dependency for the given virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        protected override CacheDependency GetCacheDependency(string virtualPath)
        {
            return new DummyCacheDependency() { Key = virtualPath };
        }
    }
}
