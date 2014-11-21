using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Taxonomies;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    /// <summary>
    /// This class represents view model for classification fields.
    /// </summary>
    public class ClassificationFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassificationFieldViewModel"/> class.
        /// </summary>
        /// <param name="taxonIds">The taxon ids.</param>
        /// <param name="classificationId">The classification identifier.</param>
        /// <param name="fieldTitle">The field title.</param>
        /// <param name="fieldName">Name of the field.</param>
        public ClassificationFieldViewModel(IList<Guid> taxonIds, Guid classificationId, string fieldTitle, string fieldName)
        {
            this.TaxonIds = taxonIds;
            this.ClassificationId = classificationId;
            this.FieldTitle = fieldTitle;
            this.FieldName = fieldName;
        }

        #region Public fields

        /// <summary>
        /// Gets the taxon ids.
        /// </summary>
        /// <value>
        /// The taxon ids.
        /// </value>
        public IList<Guid> TaxonIds { get; private set; }

        /// <summary>
        /// Gets or sets the classification identifier.
        /// </summary>
        /// <value>
        /// The classification identifier.
        /// </value>
        public Guid ClassificationId { get; set; }

        /// <summary>
        /// Gets or sets the field title.
        /// </summary>
        /// <value>
        /// The field title.
        /// </value>
        public string FieldTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        #endregion
    }
}
