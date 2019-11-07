using System;
using System.Linq;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve taxon of a given taxonomy by URL.
    /// </summary>
    internal class TaxonParamResolver : RouteParamResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxonParamResolver"/> class.
        /// </summary>
        /// <param name="taxonomyName">Name of the taxonomy where the taxon will be expected.</param>
        /// <exception cref="System.ArgumentException">When taxonomy with the given name is not found.</exception>
        public TaxonParamResolver(string taxonomyName)
        {
            this.taxonomy = GetTaxonomy(taxonomyName);
            if (this.taxonomy == null)
                throw new ArgumentException("Taxonomy with name {0} was not found!".Arrange(taxonomyName));
        }

        private ITaxonomyProxy GetTaxonomy(string taxonomyName)
        {
            if (taxonomyName.Equals("category"))
            {
                taxonomyName = "Categories";
            }
            else if (taxonomyName.Equals("tag"))
            {
                taxonomyName = "Tags";
            }

            return TaxonomyManager.GetTaxonomiesCache().FirstOrDefault(t => t.Name.Equals(taxonomyName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the taxonomy.
        /// </summary>
        /// <value>
        /// The taxonomy.
        /// </value>
        protected ITaxonomyProxy Taxonomy 
        { 
            get
            {
                return this.taxonomy;
            }
        }

        /// <inheritdoc />
        protected override bool TryResolveParamInternal(string urlParam, out object value)
        {
            if (urlParam == null)
            {
                value = null;
                return false;
            }

            value = TaxonomyManager.GetManager().GetTaxa<Taxon>().FirstOrDefault(t => t.TaxonomyId == this.Taxonomy.Id && t.UrlName == urlParam);
            return value != null;
        }

        private ITaxonomyProxy taxonomy;
    }
}
