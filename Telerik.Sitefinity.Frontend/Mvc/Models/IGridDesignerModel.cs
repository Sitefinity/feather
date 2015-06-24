using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This interface is used as a model for the GridDesignerController.
    /// </summary>
    public interface IGridDesignerModel
    {
        /// <summary>
        /// Gets or sets the grid title.
        /// </summary>
        /// <value>
        /// The grid title.
        /// </value>
        string GridTitle { get; set; }

        /// <summary>
        /// Gets the control identifier.
        /// </summary>
        /// <value>
        /// The control identifier.
        /// </value>
        string ControlId { get; }
    }
}