using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{    
    /// <summary>
    /// This class represents view model for related data fields.
    /// </summary>
    public class RelatedDataFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDataFieldViewModel"/> class.
        /// </summary>
        /// <param name="relatedItemCollection">The related item collection.</param>
        /// <param name="fieldTitle">The field title.</param>
        /// <param name="identifierField">The identifier field.</param>
        public RelatedDataFieldViewModel(IList<IDataItem> relatedItemCollection, string fieldTitle, string identifierField)
        {
            this.RelatedItemCollection = relatedItemCollection;
            this.FieldTitle = fieldTitle;
            this.IdentifierField = identifierField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDataFieldViewModel"/> class.
        /// </summary>
        /// <param name="singleRelatedItem">The single related item.</param>
        /// <param name="fieldTitle">The field title.</param>
        /// <param name="identifierField">The identifier field.</param>
        public RelatedDataFieldViewModel(IDataItem singleRelatedItem, string fieldTitle, string identifierField)
        {
            this.SingleRelatedItem = singleRelatedItem;
            this.FieldTitle = fieldTitle;
            this.IdentifierField = identifierField;
        }

        /// <summary>
        /// Gets the related item collection.
        /// </summary>
        /// <value>
        /// The related item collection.
        /// </value>
        public IList<IDataItem> RelatedItemCollection { get; private set; }

        /// <summary>
        /// Gets or sets the single related item.
        /// </summary>
        /// <value>
        /// The single related item.
        /// </value>
        public IDataItem SingleRelatedItem { get; set; }

        /// <summary>
        /// Gets or sets the field title.
        /// </summary>
        /// <value>
        /// The field title.
        /// </value>
        public string FieldTitle { get; set; }

        /// <summary>
        /// Gets or sets the identifier field.
        /// </summary>
        /// <value>
        /// The identifier field.
        /// </value>
        public string IdentifierField { get; set; }
    }
}
