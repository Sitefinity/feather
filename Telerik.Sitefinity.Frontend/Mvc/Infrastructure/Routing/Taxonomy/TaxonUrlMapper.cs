﻿using System;
using System.Linq;
using System.Web.Routing;
using Telerik.Sitefinity.Taxonomies.Model;

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
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <param name="taxon">The taxon.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="requestContext">The reqy.</param>
        /// <returns></returns>
        public bool TryMatch(string[] urlParams, string urlKeyPrefix, out ITaxon taxon, out int pageIndex, RequestContext requestContext = null)
        {
            taxon = null;
            pageIndex = 1;

            if (urlParams == null || urlParams.Length < 3)
            {
                if (requestContext == null)
                {
                    return false;
                }

                taxon = TaxonUrlEvaluator.GetTaxonFromQuery(requestContext.HttpContext, null);

                if (urlParams != null && taxon != null)
                {
                    this.TryGetPageIndex(urlParams, out pageIndex, taxon.Name);
                }

                return taxon != null;
            }

            string url = string.Join(@"/", urlParams);

            if (!this.taxonomyEvaluator.TryGetTaxonFromUrl(url, urlKeyPrefix, out taxon))
            {
                return false;
            }

            bool hasPageIndex = this.TryGetPageIndex(urlParams, out pageIndex, taxon.Name);

            if (!this.IsFlatTaxon(taxon))
            {
                return true;
            }

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

            bool hasPageIndex = this.TryGetPageIndex(urlParams, out pageIndex);

            string[] urlSegments = urlParams.Take(urlParams.Length - 1).ToArray();

            string url = string.Join(@"/", urlSegments);

            bool hasTaxon = this.taxonomyEvaluator.TryGetTaxonFromUrl(url, null, out taxon);

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
        /// <param name="taxonName">The taxon which is filtered.</param>
        /// <returns></returns>
        private bool TryGetPageIndex(string[] urlParams, out int pageIndex, string taxonName = null)
        {
            string last = urlParams.LastOrDefault();

            if (int.TryParse(last, out pageIndex))
            {
                if (last.Equals(taxonName))
                {
                    pageIndex = 0;
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}
