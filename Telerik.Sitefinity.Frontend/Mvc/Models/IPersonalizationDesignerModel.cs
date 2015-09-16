using System;
using System.Collections.Generic;
using System.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This interface is used as a model for the PersonalizationDesignerController.
    /// </summary>
    public interface IPersonalizationDesignerModel
    {
        /// <summary>
        /// Gets or sets the grid title.
        /// </summary>
        /// <value>
        /// The grid title.
        /// </value>
        string Title { get; set; }
    }
}