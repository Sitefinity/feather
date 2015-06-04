using System;
using System.Linq;
using Telerik.Sitefinity.RelatedData;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// A view model class that represents the data needed to retrieve related data.
    /// </summary>
    public class RelatedDataViewModel
    {
        /// <summary>
        /// Gets or sets the parent item identifier.
        /// </summary>
        /// <value>
        /// The parent item identifier.
        /// </value>
        public Guid RelatedItemId { get; set; }

        /// <summary>
        /// Gets or sets the type of the parent item.
        /// </summary>
        /// <value>
        /// The type of the parent item.
        /// </value>
        public string RelatedItemType { get; set; }

        /// <summary>
        /// Gets or sets the name of the parent item provider.
        /// </summary>
        /// <value>
        /// The name of the parent item provider.
        /// </value>
        public string RelatedItemProviderName { get; set; }

        /// <summary>
        /// Gets or sets the name of the related data field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string RelatedFieldName { get; set; }

        /// <summary>
        /// Gets or sets the relation type of the items that will be display - children or parent.
        /// </summary>
        /// <value>
        /// The relation type of the items that will be display - children or parent.
        /// </value>
        public RelationDirection RelationTypeToDisplay { get; set; }
    }
}
