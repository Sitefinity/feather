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
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.DataResolving;
using Telerik.Sitefinity.Web.UrlEvaluation;

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
        /// Gets the full detail page URL for master/detail widgets.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="detailsPageId">The details page identifier.</param>
        /// <param name="openInSamePage">if set to <c>true</c> [open in same page].</param>
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetDetailPageUrl(ItemViewModel item, Guid detailsPageId, bool openInSamePage, string urlKeyPrefix)
        {
            string url;
            if (openInSamePage)
            {
                url = DataResolver.Resolve(item.DataItem, "URL", urlKeyPrefix);
            }
            else
            {
                url = DataResolver.Resolve(item.DataItem, "URL", null, detailsPageId.ToString());
            }

            url = url.Replace("//", "/");

            return UrlPath.ResolveUrl(url, true);
        }

        /// <summary>
        /// Gets the full detail page URL for master/detail widgets.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="detailsPageId">The details page identifier.</param>
        /// <param name="openInSamePage">if set to <c>true</c> [open in same page].</param>
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <param name="itemIndex">Index of the item.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "3#"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetDetailPageUrl(ItemViewModel item, Guid detailsPageId, bool openInSamePage, string urlKeyPrefix, int itemIndex)
        {
            string url;
            if (openInSamePage)
            {
                url = DataResolver.Resolve(item.DataItem, "URL", urlKeyPrefix);
            }
            else
            {
                url = DataResolver.Resolve(item.DataItem, "URL", null, detailsPageId.ToString());
            }

            url = url + "?itemIndex=" + itemIndex;

            url = url.Replace("//", "/");

            return UrlPath.ResolveUrl(url, true);
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

        /// <summary>
        /// Builds the query string parameters for filtering by taxon.
        /// </summary>
        /// <param name="taxon">The taxon.</param>
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static string BuildTaxonQueryStringParams(ITaxon taxon, string urlKeyPrefix)
        {
            var evaluator = new TaxonomyEvaluator();
            var taxonBuildOptions = TaxonBuildOptions.None;
            string taxonRelativeUrl = null;

            if (taxon.Taxonomy is HierarchicalTaxonomy)
            {
                taxonBuildOptions = TaxonBuildOptions.Hierarchical;
                taxonRelativeUrl = (taxon as HierarchicalTaxon).FullUrl;
            }
            else if (taxon.Taxonomy is FlatTaxonomy)
            {
                taxonBuildOptions = TaxonBuildOptions.Flat;
                taxonRelativeUrl = taxon.UrlName.Value;
            }

            var taxonQueryStringParams = evaluator.BuildUrl(taxon.Taxonomy.Name, taxonRelativeUrl, taxon.Taxonomy.Name, taxonBuildOptions, UrlEvaluationMode.QueryString, urlKeyPrefix);
            return taxonQueryStringParams;
        }
    }
}
