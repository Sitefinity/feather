using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This interface is used as a model for the DesignerController.
    /// </summary>
    public interface IDesignerModel
    {
        /// <summary>
        /// Gets the name of the widget that is being edited.
        /// </summary>
        string WidgetName { get; }

        /// <summary>
        /// Gets the available designer views.
        /// </summary>
        IEnumerable<string> AvailableViews { get; }
    }
}
