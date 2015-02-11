using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using ServiceStack.Text;
using Telerik.Microsoft.Practices.EnterpriseLibrary.Caching;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Multisite.Model;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// Instances of this class represent an HTTP handler that returns JavaScript containing dependencies to the server.
    /// </summary>
    internal class ServerContextHandler : IHttpHandler
    {
        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            // Set that this is the backend in order to resolve the current site correctly
            context.Items[SystemManager.IsBackendRequestKey] = true;

            var script = this.GetScript();

            context.Response.Output.Write(script);
            context.Response.ContentType = "application/x-javascript";
        }

        /// <summary>
        /// Gets the processed script.
        /// </summary>
        /// <remarks>The result will be cached.</remarks>
        /// <returns>The processed script.</returns>
        protected virtual string GetScript()
        {
            var currentPackage = new PackageManager().GetCurrentPackage() ?? string.Empty;
            var currentSiteId = this.GetCurrentSiteId();
            var currentUserId = this.CurrentUserId.ToString();

            var cacheKey = string.Format(CultureInfo.InvariantCulture, ServerContextHandler.ScriptCacheKeyPattern, currentPackage, currentSiteId, currentUserId);

            var cache = this.GetCacheManager();
            var script = cache[cacheKey] as string;

            if (script == null)
            {
                lock (ServerContextHandler.scriptLock)
                {
                    if (script == null)
                    {
                        script = this.GetRawScript()
                            .Replace("{{applicationPath}}", this.GetApplicationPath())
                            .Replace("{{currentPackage}}", currentPackage)
                            .Replace("{{frontendLanguages}}", this.GetFrontendLanguages())
                            .Replace("{{currentFrontendRootNodeId}}", this.CurrentFrontendRootNodeId.ToString())
                            .Replace("{{currentUserId}}", currentUserId)
                            .Replace("{{isMultisiteMode}}", SystemManager.CurrentContext.IsMultisiteMode.ToString());

                        cache.Add(
                            cacheKey,
                            script,
                            CacheItemPriority.Normal,
                            null,
                            this.GetCacheDependency(currentSiteId));
                    }
                }
            }

            return script;
        }

        /// <summary>
        /// Gets the script from a predefined file.
        /// </summary>
        /// <returns>The script as it is recorded in the file.</returns>
        protected virtual string GetRawScript()
        {
            var path = ServerContextHandler.ScriptPath.Arrange(FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(ServerContextHandler)));
            var serverContextJs = HostingEnvironment.VirtualPathProvider.GetFile(path);

            using (var reader = new StreamReader(serverContextJs.Open()))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <returns>The application path.</returns>
        protected virtual string GetApplicationPath()
        {
            return RouteHelper.ResolveUrl("~/", UrlResolveOptions.Rooted);
        }

        /// <summary>
        /// Gets the frontend languages for the current site.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetFrontendLanguages()
        {
            var appSettings = SystemManager.CurrentContext.AppSettings;
            var languages = appSettings.DefinedFrontendLanguages.Select(l => l.Name);

            var serialziedLanguages = JsonSerializer.SerializeToString(languages);

            return serialziedLanguages;
        }

        /// <summary>
        /// Gets the id of the current site.
        /// </summary>
        /// <returns></returns>
        protected virtual Guid GetCurrentSiteId()
        {
            return SystemManager.CurrentContext.CurrentSite.Id;
        }

        /// <summary>
        /// Gets the current front-end root node id.
        /// </summary>
        /// <value>The current front-end root node id.</value>
        protected virtual Guid CurrentFrontendRootNodeId
        {
            get
            {
                return SiteInitializer.CurrentFrontendRootNodeId;
            }
        }

        /// <summary>
        /// Gets the current user id.
        /// </summary>
        /// <value>The current user id.</value>
        protected virtual Guid CurrentUserId
        {
            get
            {
                var identity = ClaimsManager.GetCurrentIdentity();

                return identity == null ? Guid.Empty : identity.UserId;
            }
        }

        /// <summary>
        /// Gets the cache manager.
        /// </summary>
        /// <returns></returns>
        protected virtual ICacheManager GetCacheManager()
        {
            return SystemManager.GetCacheManager(CacheManagerInstance.Global);
        }

        /// <summary>
        /// Gets the cache dependancy that will invalidate the script's cache.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected virtual ICacheItemExpiration GetCacheDependency(Guid key)
        {
            return new DataItemCacheDependency(typeof(Site), key);
        }

        private static object scriptLock = new object();

        private const string ScriptPath = "~/{0}Resources/ServerContext.js";

        private const string ScriptCacheKeyPattern = "FeatherServerContext-{0}-{1}-{2}";
    }
}
