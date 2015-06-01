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
            if (context == null ||
                context.Request == null)
            {
                return null;
            }

            var taxonUrlEvaluatorAdapter = new TaxonUrlEvaluatorAdapter();

            return taxonUrlEvaluatorAdapter.GetTaxonFromUrl(context.Request.RawUrl, UrlEvaluationMode.QueryString);
        }
    }
}