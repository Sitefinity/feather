using System.Diagnostics.CodeAnalysis;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    internal class TaxonUrlEvaluatorAdapter : ITaxonUrlEvaluatorAdapter
    {
        private readonly TaxonomyEvaluator taxonomyEvaluator;

        public TaxonUrlEvaluatorAdapter()
        {
            this.taxonomyEvaluator = this.GetDefaultEvaluator();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool TryGetTaxonFromUrl(string url, out ITaxon taxon)
        {
            taxon = this.GetTaxonFromUrl(url, UrlEvaluationMode.UrlPath);

            return taxon != null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public ITaxon GetTaxonFromUrl(string url, UrlEvaluationMode mode, string urlPrefix = null)
        {
            if (this.taxonomyEvaluator == null)
            {
                return null;
            }

            string taxonomyName;
            string taxonName;

            this.taxonomyEvaluator.ParseTaxonomyParams(mode, url, urlPrefix, out taxonName, out taxonomyName);

            if (!string.IsNullOrEmpty(taxonName) && !string.IsNullOrEmpty(taxonomyName))
            {
                try
                {
                    return this.taxonomyEvaluator.GetTaxonByName(taxonomyName, taxonName);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private TaxonomyEvaluator GetDefaultEvaluator()
        {
            try
            {
                return UrlEvaluator.GetEvaluator("Taxonomy") as TaxonomyEvaluator;
            }
            catch
            {
                return null;
            }
        }
    }
}