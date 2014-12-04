using System;
using System.Linq;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.DataResolving;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper class that provides utility methods for resolving urls to a content pages.
    /// </summary>
    public static class HyperLinkHelpers
    {
        /// <summary>
        /// Returns an absolute URL to a content item's details page.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="detailsPageId">The details page id.</param>
        /// <returns>The absolute url of the details page.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetDetailPageUrl(IDataItem item, Guid detailsPageId)
        {
            string url = DataResolver.Resolve(item, "URL", null, detailsPageId.ToString());
            return UrlPath.ResolveUrl(url, true);
        }
    }
}
