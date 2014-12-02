using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models.Fields
{
    public class RelatedImageViewModel : RelatedMediaViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedImageViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RelatedImageViewModel(IDataItem item)
            : base(item)
        { 
        }

        /// <summary>
        /// Gets the alternative text.
        /// </summary>
        /// <value>
        /// The alternative text.
        /// </value>
        public string AlternativeText
        {
            get
            {
                var image = this.Item as Image;

                return image.AlternativeText;
            }
        }
    }
}
