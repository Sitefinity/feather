using System;
using System.Web;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    public static class TaxonUrlEvaluator
    {
        /// <summary>
        /// Gets the taxon from query.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static ITaxon GetTaxonFromQuery(HttpContextBase context)
        {
            return GetTaxonFromQuery(context, null);
        }

        /// <summary>
        /// Gets the taxon from query.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="urlKeyPrefix">The URL key prefix.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static ITaxon GetTaxonFromQuery(HttpContextBase context, string urlKeyPrefix)
        {
            if (context == null ||
                context.Request == null)
            {
                return null;
            }

            var taxonUrlEvaluatorAdapter = new TaxonUrlEvaluatorAdapter();

            return taxonUrlEvaluatorAdapter.GetTaxonFromUrl(context.Request.RawUrl, UrlEvaluationMode.QueryString, urlKeyPrefix);
        }
    }
}