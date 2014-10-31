using System;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// This class extends the functionality of <see cref="Telerik.Sitefinity.Frontend.Resources.ServerContextHandler"/> . Used for test purposes.
    /// </summary>
    internal class DummyServerContextHandler : ServerContextHandler
    {
        /// <summary>
        /// Function to override the original GetRawScript method.
        /// </summary>
        public Func<string> GetRawScriptOverride;

        /// <summary>
        /// Function to override the original GetApplicationPath method.
        /// </summary>
        public Func<string> GetApplicationPathOverride;

        /// <summary>
        /// Function to override the original GetCurrentSiteId.
        /// </summary>
        public Func<Guid> GetCurrentSiteIdOverride;

        /// <summary>
        /// Function to override the original GetChacheManager.
        /// </summary>
        public Func<ICacheManager> GetCacheManagerOverride;

        /// <summary>
        /// Function to override the original GetFrontendLanguges.
        /// </summary>
        public Func<string> GetFrontendLanguagesOverride;

        /// <summary>
        /// Function to override the original GetCacheDependency.
        /// </summary>
        public Func<Guid, ICacheItemExpiration> GetCacheDependencyOverride;

        /// <summary>
        /// Gets the processed script.
        /// </summary>
        /// <returns>
        /// The processed script.
        /// </returns>
        /// <remarks>
        /// The result will be cached.
        /// </remarks>
        public string PublicGetScript()
        {
            return base.GetScript();
        }

        /// <inheritdoc />
        protected override Guid CurrentFrontendRootNodeId
        {
            get
            {
                return Guid.Empty;
            }
        }

        /// <inheritdoc />
        protected override string GetRawScript()
        {
            if (this.GetRawScriptOverride == null)
            {
                return base.GetRawScript();
            }
            else
            {
                return this.GetRawScriptOverride();
            }
        }

        /// <inheritdoc />
        protected override string GetApplicationPath()
        {
            if (this.GetApplicationPathOverride == null)
            {
                return base.GetApplicationPath();
            }
            else
            {
                return this.GetApplicationPathOverride();
            }
        }

        /// <inheritdoc />
        protected override Guid GetCurrentSiteId()
        {
            if (this.GetCurrentSiteIdOverride == null)
            {
                return base.GetCurrentSiteId();
            }
            else
            {
                return this.GetCurrentSiteIdOverride();
            }
        }

        /// <inheritdoc />
        protected override ICacheManager GetCacheManager()
        {
            if (this.GetCacheManagerOverride == null)
            {
                return base.GetCacheManager();
            }
            else
            {
                return this.GetCacheManagerOverride();
            }
        }

        /// <inheritdoc />
        protected override string GetFrontendLanguages()
        {
            if (this.GetFrontendLanguagesOverride == null)
            {
                return base.GetFrontendLanguages();
            }
            else
            {
                return this.GetFrontendLanguagesOverride();
            }
        }

        /// <inheritdoc />
        protected override ICacheItemExpiration GetCacheDependency(Guid key)
        {
            if (this.GetFrontendLanguagesOverride == null)
            {
                return base.GetCacheDependency(key);
            }
            else
            {
                return this.GetCacheDependencyOverride(key);
            }
        }
    }
}
