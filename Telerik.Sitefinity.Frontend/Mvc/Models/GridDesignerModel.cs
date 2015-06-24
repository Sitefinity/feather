using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    internal class GridDesignerModel : IGridDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDesignerModel"/> class.
        /// </summary>
        /// <param name="gridTitle">Titel of the grid.</param>
        /// <param name="controlId">The control identifier.</param>
        public GridDesignerModel(string gridTitle, string controlId)
        {
            this.GridTitle = gridTitle;
            this.ControlId = controlId;
        }

        /// <summary>
        /// Gets or sets the grid title.
        /// </summary>
        /// <value>
        /// The grid title.
        /// </value>
        public string GridTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the control identifier.
        /// </summary>
        /// <value>
        /// The control identifier.
        /// </value>
        public string ControlId
        {
            get;
            set;
        }
    }
}
