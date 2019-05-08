using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    internal interface ITaxonUrlEvaluatorAdapter
    {
        bool TryGetTaxonFromUrl(string url, string urlKeyPrefix, out ITaxon taxon);
    }
}