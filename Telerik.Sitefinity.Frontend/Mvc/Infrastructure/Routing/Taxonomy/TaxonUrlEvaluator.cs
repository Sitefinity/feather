using System;
using System.Linq;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /*
     * Currently we treat the following url as valid taxon filter route:
     * Pattern matching for flat taxon         - /page/-in-fieldName/taxonomyName/taxa/[page]
     * Pattern matching for hierarchical taxon - /page/-in-fieldName/taxonomyName/parent/[subParent..]/taxon
    */

    /// <summary>
    /// Instances of this class tries to map Route Data parameters to a valid taxon instace.
    /// </summary>
    internal class TaxonUrlMapper
    {
        private readonly ITaxonUrlEvaluatorAdapter taxonomyEvaluator;

        public TaxonUrlMapper(ITaxonUrlEvaluatorAdapter taxonomyEvaluator)
        {
            if (taxonomyEvaluator == null)
            {
                throw new ArgumentNullException("taxonomyEvaluator");
            }

            this.taxonomyEvaluator = taxonomyEvaluator;
        }

        /// <summary>
        /// Tries to parce a valid taxon from the route data.
        /// </summary>
        /// <param name="urlParams">The URL params.</param>
        /// <param name="taxon">The taxon.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        public bool TryMatch(string[] urlParams, out ITaxon taxon, out int pageIndex)
        {
            taxon = null;
            pageIndex = 1;

            if (urlParams == null || urlParams.Length < 3)
            {
                return false;
            }

            string url = string.Join(@"/", urlParams);

            if (!this.taxonomyEvaluator.TryGetTaxonFromUrl(url, out taxon))
            {
                return false;
            }

            if (!this.IsFlatTaxon(taxon))
            {
                return true;
            }

            bool hasPageIndex = this.TryGetLastPageIndex(urlParams, out pageIndex);

            return this.CheckForValidFlatTaxonUrl(urlParams, hasPageIndex);
        }

        /// <summary>
        /// Determines whether [is flat taxon] [the specified taxon].
        /// </summary>
        /// <param name="taxon">The taxon.</param>
        /// <returns></returns>
        private bool IsFlatTaxon(ITaxon taxon)
        {
            return taxon is FlatTaxon;
        }

        /// <summary>
        /// Checks for valid flat taxon URL.
        /// </summary>
        /// <param name="urlParams">The URL params.</param>
        /// <param name="isPreviousPageIndex">Index of the is previous page.</param>
        /// <returns></returns>
        private bool CheckForValidFlatTaxonUrl(string[] urlParams, bool isPreviousPageIndex)
        {
            if (urlParams.Length < 2)
            {
                return false;
            }

            ITaxon taxon;
            int pageIndex;

            bool hasPageIndex = this.TryGetLastPageIndex(urlParams, out pageIndex);

            string[] urlSegments = urlParams.Take(urlParams.Length - 1).ToArray();

            string url = string.Join(@"/", urlSegments);

            bool hasTaxon = this.taxonomyEvaluator.TryGetTaxonFromUrl(url, out taxon);

            if (!hasPageIndex && hasTaxon)
            {
                return false;
            }
            else if (isPreviousPageIndex && !hasPageIndex && hasTaxon)
            {
                return false;
            }
            else if (!hasPageIndex && !hasTaxon)
            {
                return true;
            }

            return this.CheckForValidFlatTaxonUrl(urlSegments, hasPageIndex);
        }

        /// <summary>
        /// Tries the index of the get last page.
        /// </summary>
        /// <param name="urlParams">The URL params.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        private bool TryGetLastPageIndex(string[] urlParams, out int pageIndex)
        {
            string last = urlParams.LastOrDefault();

            return int.TryParse(last, out pageIndex);
        }
    }
}
