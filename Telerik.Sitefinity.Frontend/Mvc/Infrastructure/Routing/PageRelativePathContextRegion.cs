using System;
using System.Web;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    internal sealed class PageRelativePathContextRegion : IDisposable
    {
        public PageRelativePathContextRegion(HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.originalPath = context.Request.AppRelativeCurrentExecutionFilePath;

            var currentNode = SiteMapBase.GetCurrentNode();
            if (currentNode != null)
            {
                var nodeUrl = currentNode.Url.StartsWith("~/", StringComparison.Ordinal) ? RouteHelper.ResolveUrl(currentNode.Url, UrlResolveOptions.ApplicationRelative | UrlResolveOptions.AppendTrailingSlash) : currentNode.Url;
                if (this.originalPath.StartsWith(nodeUrl, StringComparison.OrdinalIgnoreCase))
                {
                    this.context.RewritePath("~/" + this.originalPath.Right(this.originalPath.Length - nodeUrl.Length));
                }
            }
        }

        public void Dispose()
        {
            this.context.RewritePath(this.originalPath);
        }

        private readonly HttpContextBase context;
        private readonly string originalPath;
    }
}
