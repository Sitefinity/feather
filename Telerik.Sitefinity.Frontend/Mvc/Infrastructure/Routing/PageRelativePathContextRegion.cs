using System;
using System.Web;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// In the scope of the region the request path of the context is rewritten to omit the page node URL in order to get node-relative path.
    /// </summary>
    internal sealed class PageRelativePathContextRegion : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageRelativePathContextRegion"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public PageRelativePathContextRegion(HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.originalPath = context.Request.AppRelativeCurrentExecutionFilePath;
            var originalWithSlash = VirtualPathUtility.AppendTrailingSlash(this.originalPath);

            bool? isFrontendPageEdit = context.Items["IsFrontendPageEdit"] as bool?;
            var currentNode = SiteMapBase.GetCurrentNode();
            if (currentNode != null && !(isFrontendPageEdit.HasValue && isFrontendPageEdit.Value))
            {
                var nodeUrl = currentNode.Url.StartsWith("~/", StringComparison.Ordinal) ? RouteHelper.ResolveUrl(currentNode.Url, UrlResolveOptions.ApplicationRelative | UrlResolveOptions.AppendTrailingSlash) : currentNode.Url;
                var comparisonNodeUrl = nodeUrl.Replace("~", string.Empty);
                if (originalWithSlash.StartsWith(nodeUrl, StringComparison.OrdinalIgnoreCase))
                {
                    var newPath = originalWithSlash.Right(originalWithSlash.Length - nodeUrl.Length);

                    this.context.RewritePath("~/" + newPath);
                }
                else if (SystemManager.CurrentContext.CurrentSite.Cultures.Length > 1 && originalWithSlash.StartsWith($"~/{SystemManager.CurrentContext.Culture}/", StringComparison.OrdinalIgnoreCase) && originalWithSlash.Contains(comparisonNodeUrl))
                {
                    var newPath = originalWithSlash.Right(originalWithSlash.Length - nodeUrl.Length - SystemManager.CurrentContext.Culture.Name.Length - 1);

                    this.context.RewritePath("~/" + newPath);
                }
                else if (currentNode.IsHomePage() && 
                    RouteHelper.ResolveUrl(SystemManager.CurrentContext.CurrentSite.GetUri().AbsolutePath, UrlResolveOptions.ApplicationRelative | UrlResolveOptions.AppendTrailingSlash) == originalWithSlash)
                {
                    // The request is to the root of the site
                    this.context.RewritePath("~/");
                }
            }
            else
            {
                this.context.RewritePath("~/");
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.context.RewritePath(this.originalPath);
        }

        private readonly HttpContextBase context;
        private readonly string originalPath;
    }
}
