using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class is used as a model for the personalization designer controller.
    /// </summary>
    internal class PersonalizationDesignerModel : IPersonalizationDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalizationDesignerModel"/> class.
        /// </summary>
        public PersonalizationDesignerModel()
        {
        }

        /// <summary>
        /// Gets or sets the personalization dialog title.
        /// </summary>
        /// <value>
        /// The dialog title.
        /// </value>
        public string Title
        {
            get;
            set;
        }
    }
}
