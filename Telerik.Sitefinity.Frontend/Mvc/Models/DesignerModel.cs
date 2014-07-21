using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class is used as a model for the designer controller.
    /// </summary>
    public class DesignerModel : IDesignerModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DesignerModel"/> class.
        /// </summary>
        /// <param name="views">The views that are available to the controller.</param>
        public DesignerModel(IEnumerable<string> views, string widgetName)
        {
            this.views = views.Where(this.IsDesignerView).Select(this.ExtractViewName);

            var packagesManager = new PackageManager();
            var packageName = packagesManager.GetCurrentPackage();

            var designerWidgetName = FrontendManager.ControllerFactory.GetControllerName(typeof(DesignerController));

            this.viewScriptReferences = this.views
                .Where(v => this.IsScriptExisting(v, widgetName, packageName) || this.IsScriptExisting(v, designerWidgetName, packageName))
                .Select(v => DesignerModel.DesignerScriptsPath + "/" + designerWidgetName + "/" + this.GetViewScriptFileName(v))
                .ToArray();
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
        public virtual IEnumerable<string> ViewScriptReferences
        {
            get
            {
                return this.viewScriptReferences;
            }
        }

        /// <summary>
        /// Determines whether the given file represents a designer view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected virtual bool IsDesignerView(string filename)
        {
            return filename.StartsWith("DesignerView.");
        }

        /// <summary>
        /// Extracts the name of the view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected virtual string ExtractViewName(string filename)
        {
            var parts = filename.Split('.');
            if (parts.Length > 2)
                return string.Join(".", parts.Skip(1).Take(parts.Length - 2));
            else if (parts.Length == 2)
                return string.Join(".", parts.Skip(1));
            else
                return filename;
        }

        /// <summary>
        /// Determines whether a script is existing for the specified view in the context of the given widget name.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="widgetName">Name of the widget.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <returns>Whether a script is existing.</returns>
        protected virtual bool IsScriptExisting(string view, string widgetName, string packageName)
        {
            var widgetType = FrontendManager.ControllerFactory.ResolveControllerType(widgetName);
            var path = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(widgetType);

            var scriptVirtualPath = path + DesignerModel.DesignerScriptsPath + "/" + widgetName + "/" + this.GetViewScriptFileName(view);
            scriptVirtualPath = packageName.IsNullOrEmpty() ? 
                scriptVirtualPath : UrlTransformations.AppendParam(scriptVirtualPath, PackageManager.PackageUrlParamterName, packageName);

            return VirtualPathManager.FileExists(scriptVirtualPath);
        }

        /// <summary>
        /// Gets the name of the script file for a given view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>File name of the client script file for a given view.</returns>
        protected virtual string GetViewScriptFileName(string view)
        {
            return DesignerModel.ScriptPrefix + "-" + view.Replace('.', '-').ToLower() + ".js";
        }

        private IEnumerable<string> views;
        private IEnumerable<string> viewScriptReferences;
        private const string ScriptPrefix = "designerview";
        private const string DesignerScriptsPath = "Mvc/Scripts";
    }
}
