using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Mvc.Models;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Models
{
    /// <summary>
    /// This class represents a dummy designer model.
    /// </summary>
    internal class DummyDesignerModel : DesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyDesignerModel"/> class.
        /// </summary>
        public DummyDesignerModel() : base(new List<string>(), new List<string>(), string.Empty, Guid.Empty, string.Empty)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyDesignerModel"/> class.
        /// </summary>
        /// <param name="views">The views that are available to the controller.</param>
        /// <param name="viewLocations">The locations where view files can be found.</param>
        /// <param name="widgetName">Name of the widget that is being edited.</param>
        /// <param name="controlId">Id of the control that is being edited.</param>
        /// <param name="preselectedView">Name of the preselected view if there is one. Otherwise use null.</param>
        public DummyDesignerModel(IEnumerable<string> views, IEnumerable<string> viewLocations, string widgetName, Guid controlId, string preselectedView)
            : base(views, viewLocations, widgetName, controlId, preselectedView)
        {
        }

        /// <summary>
        /// Exposes the IsDesignerView method.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public bool IsDesignerViewPublic(string fileName)
        {
            return this.IsDesignerView(fileName);
        }

        /// <summary>
        /// Exposes the ExtractViewName method.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public string ExtractViewNamePublic(string fileName)
        {
            return this.ExtractViewName(fileName);
        }

        /// <summary>
        /// Exposes the PopulateScriptReferences method.
        /// </summary>
        /// <param name="widgetName">Name of the widget.</param>
        /// <param name="viewConfigs">The view configs.</param>
        public void PopulateScriptReferencesPublic(string widgetName, IEnumerable<KeyValuePair<string, DesignerViewConfigModel>> viewConfigs)
        {
            this.PopulateScriptReferences(widgetName, viewConfigs);
        }

        /// <summary>
        /// Exposes the GetViewScriptFileName method.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns></returns>
        public string GetViewScriptFileNamePublic(string view)
        {
            return this.GetViewScriptFileName(view);
        }
    }
}
