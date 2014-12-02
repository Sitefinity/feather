using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models.Fields
{
    /// <summary>
    /// This class represents base view model for related data and related media fields.
    /// </summary>
    public abstract class RelatedViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedViewModel"/> class.
        /// </summary>
        protected RelatedViewModel()
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        protected RelatedViewModel(IDataItem item)
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
        /// Gets the value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public object GetValue(string propertyName)
        {
            return ((IDynamicFieldsContainer)this.Item).GetValue(propertyName);
        }
    }
}
