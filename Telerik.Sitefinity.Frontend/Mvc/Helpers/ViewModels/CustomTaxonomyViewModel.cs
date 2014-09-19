using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels
{
    /// <summary>
    /// This class represents the view model for the custom taxonomy fields.
    /// </summary>
    public class CustomTaxonomyViewModel
    {
        /// <summary>
        /// Gets or sets the name of the custom field.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title of the taxonomy.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type of the taxonomy.
        /// </summary>
        /// <value>
        /// The type of the taxonomy.
        /// </value>
        public TaxonomyType TaxonomyType { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the taxonomy.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
    }
}
