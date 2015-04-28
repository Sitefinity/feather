using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class is used as a model for the designer controller.
    /// </summary>
    internal class DesignerModel : IDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerModel"/> class.
        /// </summary>
        /// <param name="views">The views that are available to the controller.</param>
        /// <param name="viewLocations">The locations where view files can be found.</param>
        /// <param name="widgetName">Name of the widget that is being edited.</param>
        /// <param name="controlId">Id of the control that is edited.</param>
        /// <param name="preselectedView">Name of the preselected view if there is one. Otherwise use null.</param>
        public DesignerModel(IEnumerable<string> views, IEnumerable<string> viewLocations, string widgetName, Guid controlId, string preselectedView)
        {
            this.Caption = widgetName;

            if (preselectedView.IsNullOrEmpty())
            {
                this.views = views.Where(this.IsDesignerView).Select(this.ExtractViewName);
            }
            else
            {
                this.views = new[] { preselectedView };
            }

            var viewConfigs = this.views
                .Select(v => new KeyValuePair<string, DesignerViewConfigModel>(v, this.GetViewConfig(v, viewLocations)))
                .Where(c => c.Value != null);

            if (preselectedView.IsNullOrEmpty())
            {
                this.views = this.views.Where(v => !viewConfigs.Any(vc => vc.Key == v) || viewConfigs.Single(vc => vc.Key == v).Value.Hidden == false).ToArray();
                viewConfigs = viewConfigs.Where(c => !c.Value.Hidden);
            }

            this.PopulateScriptReferences(widgetName, viewConfigs);

            this.defaultView = viewConfigs.OrderByDescending(c => c.Value.Priority).Select(c => c.Key).FirstOrDefault();

            this.Control = this.LoadControl(controlId);
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> Views
        {
            get 
            {
                return this.views;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> ScriptReferences
        {
            get
            {
                return this.scriptReferences;
            }
        }

        /// <inheritdoc />
        public virtual string DefaultView
        {
            get
            {
                return this.defaultView;
            }
        }

        /// <inheritdoc />
        public Control Control { get; set; }

        /// <inheritdoc />
        public string Caption { get; set; }

        /// <summary>
        /// Determines whether the given file represents a designer view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected bool IsDesignerView(string filename)
        {
            return filename != null && filename.StartsWith(DesignerModel.DesignerViewPrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Extracts the name of the view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected string ExtractViewName(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            var parts = filename.Split('.');

            if (parts.Length > 2)
            {
                return string.Join(".", parts.Skip(1).Take(parts.Length - 2));
            }

            if (parts.Length == 2)
            {
                return string.Join(".", parts.Skip(1));
            }

            return filename;
        }

        /// <summary>
        /// Populates the script references.
        /// </summary>
        /// <param name="widgetName">Name of the widget.</param>
        /// <param name="viewConfigs">The view configs.</param>
        protected void PopulateScriptReferences(string widgetName, IEnumerable<KeyValuePair<string, DesignerViewConfigModel>> viewConfigs)
        {
            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();

            var designerWidgetName = FrontendManager.ControllerFactory.GetControllerName(typeof(DesignerController));

            var viewScriptReferences = new List<string>(this.views.Count());

            foreach (var view in this.views)
            {
                var scriptFileName = this.GetViewScriptFileName(view);
                var scriptPath = this.GetScriptPath(scriptFileName, widgetName, packageName);
                if (VirtualPathManager.FileExists(scriptPath))
                {
                    viewScriptReferences.Add(this.GetScriptReferencePath(widgetName, scriptFileName));
                }
                else
                {
                    scriptPath = this.GetScriptPath(scriptFileName, designerWidgetName, packageName);
                    if (VirtualPathManager.FileExists(scriptPath))
                        viewScriptReferences.Add(this.GetScriptReferencePath(designerWidgetName, scriptFileName));
                }
            }

            var configuredScriptReferences = viewConfigs
                .Where(c => c.Value.Scripts != null)
                .SelectMany(c => c.Value.Scripts);

            this.scriptReferences = viewScriptReferences
                .Union(configuredScriptReferences)
                .Distinct();
        }

        /// <summary>
        /// Gets the name of the script file for a given view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>File name of the client script file for a given view.</returns>
        protected string GetViewScriptFileName(string view)
        {
            if (string.IsNullOrEmpty(view))
                throw new ArgumentNullException("view");

            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}{1}.js", DesignerModel.ScriptPrefix, view.Replace('.', '-').ToLowerInvariant());
        }

        /// <summary>
        /// Gets the view configuration.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewLocations">Locations where view files can be found.</param>
        /// <returns>Config for the given view if such config exists.</returns>
        protected DesignerViewConfigModel GetViewConfig(string view, IEnumerable<string> viewLocations)
        {
            if (view == null)
                throw new ArgumentNullException("view");

            if (viewLocations == null)
                throw new ArgumentNullException("viewLocations");

            foreach (var viewLocation in viewLocations)
            {
                var expectedConfigFileName = viewLocation + "/" + DesignerViewPrefix + view + ".json";

                if (VirtualPathManager.FileExists(expectedConfigFileName))
                {
                    var fileStream = VirtualPathManager.OpenFile(expectedConfigFileName);

                    using (var streamReader = new StreamReader(fileStream))
                    {
                        var text = streamReader.ReadToEnd();
                        var designerViewConfigModel = new JavaScriptSerializer().Deserialize<DesignerViewConfigModel>(text);

                        this.PopulateComponentsScriptReferences(designerViewConfigModel);

                        return designerViewConfigModel;
                    }
                }
            }

            return null;
        }

        private void PopulateComponentsScriptReferences(DesignerViewConfigModel designerViewConfigModel)
        {
            if (designerViewConfigModel == null)
            {
                return;
            }

            if (designerViewConfigModel.Components != null && designerViewConfigModel.Components.Count > 0)
            {
                if (designerViewConfigModel.Scripts == null)
                {
                    designerViewConfigModel.Scripts = new List<string>();
                }

                designerViewConfigModel.Scripts = ScriptDependencyResolver.GetScripts(designerViewConfigModel.Components, designerViewConfigModel.Scripts);
            }
        }

        private string GetScriptReferencePath(string widgetName, string scriptFileName)
        {
            return DesignerModel.DesignerScriptsPath + "/" + widgetName + "/" + scriptFileName;
        }

        private string GetScriptPath(string scriptFileName, string widgetName, string packageName)
        {
            var widgetType = FrontendManager.ControllerFactory.ResolveControllerType(widgetName);
            var path = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(widgetType);

            var scriptVirtualPath = path + this.GetScriptReferencePath(widgetName, scriptFileName);
            scriptVirtualPath = packageName.IsNullOrEmpty() ?
                scriptVirtualPath : FrontendManager.VirtualPathBuilder.AddParams(scriptVirtualPath, packageName);

            return scriptVirtualPath;
        }

        private Control LoadControl(Guid controlId)
        {
            if (controlId != Guid.Empty)
            {
                var pageManager = PageManager.GetManager();
                var objectData = pageManager.GetControl<ObjectData>(controlId);

                var controlData = objectData as ControlData;
                if (controlData != null && !controlData.Caption.IsNullOrEmpty())
                {
                    this.Caption = controlData.Caption;
                }

                return pageManager.LoadControl(objectData);
            }
            else
            {
                return null;
            }
        }

        private const string DesignerViewPrefix = "DesignerView.";
        private const string ScriptPrefix = "designerview-";
        private const string DesignerScriptsPath = "Mvc/Scripts";

        private IEnumerable<string> views;
        private IEnumerable<string> scriptReferences;
        private string defaultView;
    }
}
