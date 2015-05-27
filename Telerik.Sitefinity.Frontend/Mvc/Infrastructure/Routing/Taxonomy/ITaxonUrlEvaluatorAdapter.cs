using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Taxonomies.Model;
using Telerik.Sitefinity.Web.UrlEvaluation;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    internal interface ITaxonUrlEvaluatorAdapter
    {
        bool TryGetTaxonFromUrl(string url, out ITaxon taxon);
    }
}