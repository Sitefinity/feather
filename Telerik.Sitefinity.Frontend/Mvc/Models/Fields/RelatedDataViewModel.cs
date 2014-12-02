using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.RelatedData;

namespace Telerik.Sitefinity.Frontend.Mvc.Models.Fields
{
    /// <summary>
    /// This class represents view model for related data templates.
    /// </summary>
    public class RelatedDataViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDataVeiwModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RelatedDataViewModel(IDataItem item)
        {
            this.Item = item;
        }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The item.
        /// </value>
        public IDataItem Item { get; set; }

        /// <summary>
        /// Gets the default URL.
        /// </summary>
        /// <value>
        /// The default URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string DefaultUrl
        {
            get
            {
                return ((object)this.Item).GetDefaultUrl();
            }
        }

        public object GetValue(string propertyName)
        {
            return ((IDynamicFieldsContainer)this.Item).GetValue(propertyName);
        }
    }
}
