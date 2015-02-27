using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Localization.UrlLocalizationStrategies;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Pages;
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

        /// <summary>
        /// Gets the detail page URL for master/detail widgets.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="detailsPageId">The details page identifier.</param>
        /// <param name="openInSamePage">if set to <c>true</c> [open in same page].</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetDetailPageUrl(ItemViewModel item, Guid detailsPageId, bool openInSamePage)
        {
            string url;
            if (openInSamePage)
            {
                var appRelativeUrl = DataResolver.Resolve(item.DataItem, "URL");
                url = UrlPath.ResolveUrl(appRelativeUrl, true);
            }
            else
            {
                url = HyperLinkHelpers.GetDetailPageUrl(item.DataItem, detailsPageId);
            }

            return url;
        }

        /// <summary>
        /// Gets the detail page URL for master/detail widgets.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="detailsPageId">The details page identifier.</param>
        /// <param name="openInSamePage">if set to <c>true</c> [open in same page].</param>
        /// <param name="itemIndex">Index of the item in collection.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetDetailPageUrl(ItemViewModel item, Guid detailsPageId, bool openInSamePage, int itemIndex)
        {
            string url;
            if (openInSamePage)
            {
                var appRelativeUrl = DataResolver.Resolve(item.DataItem, "URL");
                appRelativeUrl = appRelativeUrl + "?itemIndex=" + itemIndex;
                url = UrlPath.ResolveUrl(appRelativeUrl, true);
            }
            else
            {
                url = HyperLinkHelpers.GetDetailPageUrl(item.DataItem, detailsPageId) + "?itemIndex=" + itemIndex;
            }

            return url;
        }

        /// <summary>
        /// Gets the full page URL.
        /// </summary>
        /// <param name="pageId">The page identifier.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetFullPageUrl(Guid pageId)
        {
            if (pageId != Guid.Empty)
            {
                var siteMap = SitefinitySiteMap.GetCurrentProvider();

                SiteMapNode node;
                var sitefinitySiteMap = siteMap as SiteMapBase;
                if (sitefinitySiteMap != null)
                {
                    node = sitefinitySiteMap.FindSiteMapNodeFromKey(pageId.ToString(), false);
                }
                else
                {
                    node = siteMap.FindSiteMapNodeFromKey(pageId.ToString());
                }

                if (node != null)
                {
                    return UrlPath.ResolveUrl(node.Url, true);
                }
            }

            return null;
        }
    }
}
