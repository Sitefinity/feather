using System;
using System.Web;
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

            var currentNode = SiteMapBase.GetCurrentNode();
            if (currentNode != null)
            {
                var nodeUrl = currentNode.Url.StartsWith("~/", StringComparison.Ordinal) ? RouteHelper.ResolveUrl(currentNode.Url, UrlResolveOptions.ApplicationRelative | UrlResolveOptions.AppendTrailingSlash) : currentNode.Url;
                if (originalWithSlash.StartsWith(nodeUrl, StringComparison.OrdinalIgnoreCase))
                {
                    var newPath = originalWithSlash.Right(originalWithSlash.Length - nodeUrl.Length);
                    bool? isFrontendPageEdit = context.Items["IsFrontendPageEdit"] as bool?;
                    if (isFrontendPageEdit.HasValue && isFrontendPageEdit.Value)
                        newPath = string.Empty;

                    this.context.RewritePath("~/" + newPath);
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
