using System.Collections.Generic;
using System.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This interface is used as a model for the DesignerController.
    /// </summary>
    public interface IDesignerModel
    {
        /// <summary>
        /// Gets the available designer views.
        /// </summary>
        IEnumerable<string> Views { get; }

        /// <summary>
        /// Gets the script references that the designer should render.
        /// </summary>
        IEnumerable<string> ScriptReferences { get; }

        /// <summary>
        /// Gets the default view.
        /// </summary>
        string DefaultView { get; }

        /// <summary>
        /// Gets the control that is being edited.
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Gets the caption of the designer.
        /// </summary>
        /// <value>
        /// The caption.
        /// </value>
        string Caption { get; }
    }
}
