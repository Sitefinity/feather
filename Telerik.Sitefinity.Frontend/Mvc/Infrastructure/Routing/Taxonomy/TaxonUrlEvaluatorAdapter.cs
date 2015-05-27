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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool TryGetTaxonFromUrl(string url, out ITaxon taxon)
        {
            taxon = null;

            if (this.taxonomyEvaluator == null)
            {
                return false;
            }

            string taxonomyName;
            string taxonName;

            this.taxonomyEvaluator.ParseTaxonomyParams(UrlEvaluationMode.UrlPath, url, null, out taxonName, out taxonomyName);

            if (!string.IsNullOrEmpty(taxonName) && !string.IsNullOrEmpty(taxonomyName))
            {
                try
                {
                    taxon = this.taxonomyEvaluator.GetTaxonByName(taxonomyName, taxonName);
                }
                catch
                {
                    return false;
                }
            }

            return taxon != null;
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