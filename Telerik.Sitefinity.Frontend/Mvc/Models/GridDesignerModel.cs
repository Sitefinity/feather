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
        /// <param name="widgetName">Name of the widget.</param>
        /// <param name="controlId">The control identifier.</param>
        public GridDesignerModel(string widgetName, string controlId)
        {
            this.WidgetName = widgetName;
            this.ControlId = controlId;
        }

        /// <summary>
        /// Gets or sets the name of the widget.
        /// </summary>
        /// <value>
        /// The name of the widget.
        /// </value>
        public string WidgetName
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
