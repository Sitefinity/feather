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
    public class RelatedDataViewModel : RelatedViewModel
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDataViewModel"/> class.
        /// </summary>
        public RelatedDataViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedDataVeiwModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RelatedDataViewModel(IDataItem item) : base(item) 
        {
        }

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
    }
}
