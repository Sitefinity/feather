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
        /// <param name="viewFilesMappings">Map of the view file location for each view.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DesignerModel(IEnumerable<string> views, IEnumerable<string> viewLocations, string widgetName, Guid controlId, string preselectedView, Dictionary<string, string> viewFilesMappings)
        {
            this.Caption = widgetName;

            var formatedViewsDictionary = this.GetViews(views, preselectedView);
            var viewConfigs = this.GetViewConfigs(formatedViewsDictionary, viewLocations, viewFilesMappings);

            if (string.IsNullOrEmpty(preselectedView))
            {
                this.views = formatedViewsDictionary.Values.Where(v => !viewConfigs.Any(vc => vc.Key == v) || viewConfigs.Single(vc => vc.Key == v).Value.Hidden == false).ToArray();
                viewConfigs = viewConfigs.Where(c => !c.Value.Hidden).ToList();
            }
            else
            {
                this.views = formatedViewsDictionary.Values;
            }

            // If no configs have set priority and at least one config is generated - set its priority to 1.
            if (!viewConfigs.Any(c => c.Value.Priority != 0) && viewConfigs.Any(c => c.Value.IsGenerated))
                viewConfigs.FirstOrDefault(c => c.Value.IsGenerated).Value.Priority = 1;

            this.PopulateDependencies(widgetName, viewConfigs);

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
        public virtual IEnumerable<string> ModuleDependencies
        {
            get 
            {
                return this.moduleDependencies;
            }

            private set
            {
                this.moduleDependencies = value;
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
            if (filename.EndsWith(".cshtml", StringComparison.Ordinal))
            {
                parts = parts.Take(parts.Length - 1).ToArray();
            }

            if (parts.Length > 2)
            {
                return string.Join(".", parts.Skip(1).Take(parts.Length - 1));
            }

            if (parts.Length == 2)
            {
                return string.Join(".", parts.Skip(1));
            }

            return filename;
        }

        /// <summary>
        /// Populates the script references and dependant modules.
        /// </summary>
        /// <param name="widgetName">Name of the widget.</param>
        /// <param name="viewConfigs">The view configs.</param>
        protected void PopulateDependencies(string widgetName, IEnumerable<KeyValuePair<string, DesignerViewConfigModel>> viewConfigs)
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
                    {
                        viewScriptReferences.Add(this.GetScriptReferencePath(designerWidgetName, scriptFileName));
                    }
                    else
                    {
                        var viewConfig = viewConfigs.Where(v => v.Key == view).SingleOrDefault();
                        this.ModuleDependencies = this.ModuleDependencies.Concat(ComponentsDependencyResolver.GetModules(viewConfig.Value.Components)).Distinct();
                    }
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

                        this.PopulateModulesForScripts(designerViewConfigModel.Scripts);
                        this.PopulateComponentsScriptReferences(designerViewConfigModel);

                        return designerViewConfigModel;
                    }
                }
            }

            return null;
        }
        
        /// <summary>
        /// Generates the view configuration if its missing from the file system.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewLocations">Locations where view files can be found.</param>
        /// <param name="viewFilesMappings">Map of the view file location for each view.</param>
        /// <returns>Config for the given view.</returns>
        protected DesignerViewConfigModel GenerateViewConfig(string view, IEnumerable<string> viewLocations, Dictionary<string, string> viewFilesMappings)
        {
            var viewFilePath = this.GetViewFilePath(view, viewLocations, viewFilesMappings);
            if (!string.IsNullOrEmpty(viewFilePath))
            {
                using (var fileStream = VirtualPathManager.OpenFile(viewFilePath))
                {
                    var components = ComponentsDependencyResolver.ExtractComponents(fileStream);
                    var scripts = ComponentsDependencyResolver.GetScripts(components, null);

                    // If view that exists has been parsed and no components are used in it - no point in cycling trough the other views
                    return new DesignerViewConfigModel() { Scripts = scripts, Components = components, IsGenerated = true };
                }
            }

            return null;
        }

        private string GetViewFilePath(string view, IEnumerable<string> viewLocations, Dictionary<string, string> viewFilesMappings)
        {
            // If view file mapping exists return it
            if (viewFilesMappings != null && viewFilesMappings.ContainsKey(view))
                return viewFilesMappings[view];

            // Search all locations for the view
            IEnumerable<string> viewExtensions = new string[] { "aspx", "ascx", "master", "cshtml", "vbhtml" };
            foreach (var viewLocation in viewLocations)
            {
                foreach (var viewExtension in viewExtensions)
                {
                    var expectedViewFileName = string.Format("{0}{1}{2}.{3}", viewLocation, DesignerViewPrefix, view, viewExtension);
                    if (VirtualPathManager.FileExists(expectedViewFileName))
                    {
                        return expectedViewFileName;
                    }
                }
            }
            
            return null;
        }

        private IDictionary<string, string> GetViews(IEnumerable<string> rawViewNames, string preselectedView)
        {
            if (string.IsNullOrEmpty(preselectedView))
                return rawViewNames.Where(this.IsDesignerView).ToDictionary(v => v, this.ExtractViewName);
            else
                return new Dictionary<string, string>() { { preselectedView, preselectedView } };
        }

        private IList<KeyValuePair<string, DesignerViewConfigModel>> GetViewConfigs(IDictionary<string, string> formatedViewsDictionary, IEnumerable<string> viewLocations, Dictionary<string, string> viewFilesMappings)
        {
            var designerViewFilesMappings = new Dictionary<string, string>();
            if (viewFilesMappings != null)
            {
                foreach (var kv in formatedViewsDictionary)
                {
                    if (viewFilesMappings.ContainsKey(kv.Key))
                        designerViewFilesMappings[kv.Value] = viewFilesMappings[kv.Key];
                }
            }

            var viewConfigs = new List<KeyValuePair<string, DesignerViewConfigModel>>();

            foreach (var kv in formatedViewsDictionary)
            {
                var config = this.GetViewConfig(kv.Value, viewLocations);
                if (config == null)
                    config = this.GenerateViewConfig(kv.Value, viewLocations, designerViewFilesMappings);

                if (config != null)
                    viewConfigs.Add(new KeyValuePair<string, DesignerViewConfigModel>(kv.Value, config));
            }

            return viewConfigs;
        }

        private void PopulateModulesForScripts(IEnumerable<string> scripts)
        {
            if (scripts == null || scripts.Count() == 0)
            {
                return;
            }

            var modules = ComponentsDependencyResolver.GetModulesByScripts(scripts);
            this.ModuleDependencies = this.ModuleDependencies.Concat(modules);
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

                designerViewConfigModel.Scripts = ComponentsDependencyResolver.GetScripts(designerViewConfigModel.Components, designerViewConfigModel.Scripts);
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
        private IEnumerable<string> moduleDependencies = new List<string>();
        private string defaultView;
    }
}
