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
            this.taxonomy = TaxonomyManager.GetManager().GetTaxonomies<Taxonomy>().FirstOrDefault(t => t.Name == taxonomyName);
            if (this.taxonomy == null)
                throw new ArgumentException("Taxonomy with name {0} was not found!".Arrange(taxonomyName));
        }

        /// <summary>
        /// Gets the taxonomy.
        /// </summary>
        /// <value>
        /// The taxonomy.
        /// </value>
        protected Taxonomy Taxonomy 
        { 
            get
            {
                return this.taxonomy;
            }
        }

        /// <inheritdoc />
        protected override bool TryResolveParamInternal(string urlParam, out object value)
        {
            value = this.Taxonomy.Taxa.FirstOrDefault(t => t.UrlName == urlParam);
            return value != null;
        }

        private Taxonomy taxonomy;
    }
}
