using System;
using System.Linq;
using System.Collections.Generic;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions.VirtualPath;

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
        /// <param name="widgetName">Name of the widget that is being edited.</param>
        /// <param name="viewLocations">The locations where designer views could be found.</param>
        /// <param name="viewExtensions">The extensions of view files.</param>
        public DesignerModel(string widgetName, IEnumerable<string> viewLocations, IEnumerable<string> viewExtensions)
        {
            this.WidgetName = widgetName;
            this.viewLocations = viewLocations;
            this.viewExtensions = viewExtensions.Select(e => e.StartsWith(".") ? e : "." + e);
            this.resolverStrategy = ObjectFactory.Resolve<IResourceResolverStrategy>();
        }

        /// <inheritdoc />
        public virtual string WidgetName { get; private set; }

        /// <inheritdoc />
        public virtual IEnumerable<string> AvailableViews
        {
            get 
            {
                if (this.availableViews == null)
                {
                    this.availableViews = this.GetAvailableViews();
                }
                return this.availableViews;
            }
        }

        /// <summary>
        /// Determines whether the given file represents a designer view.
        /// </summary>
        /// <param name="filename">The filename.</param>
        protected virtual bool IsDesignerView(string filename)
        {
            return filename.StartsWith("DesignerView.") && viewExtensions.Any(e => filename.EndsWith(e));
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

        private IEnumerable<string> GetAvailableViews()
        {
            var designerFiles = this.GetAvailableViewsForAssembly(typeof(DesignerModel).Assembly);
            if (!this.WidgetName.IsNullOrEmpty())
            {
                var widgetAssembly = FrontendManager.ControllerFactory.ResolveControllerType(this.WidgetName).Assembly;
                var widgetFiles = this.GetAvailableViewsForAssembly(widgetAssembly);
                return designerFiles.Union(widgetFiles);
            }
            else
            {
                return designerFiles;
            }
        }

        private IEnumerable<string> GetAvailableViewsForAssembly(Assembly assembly)
        {
            var pathDef = FrontendManager.VirtualPathBuilder.GetPathDefinition(assembly);
            return this.viewLocations
                .SelectMany(l => this.GetAvailableViewsForPath(pathDef, l))
                .Distinct();
        }

        private IEnumerable<string> GetAvailableViewsForPath(PathDefinition definition, string path)
        {
            var availableFiles = this.resolverStrategy.GetAvailableFiles(definition, path);

            if (availableFiles != null)
            {
                return availableFiles
                    .Select(f => VirtualPathUtility.GetFileName(f))
                    .Where(this.IsDesignerView)
                    .Select(this.ExtractViewName);
            }
            else
            {
                return new string[] { };
            }
        }

        private IResourceResolverStrategy resolverStrategy;
        private IEnumerable<string> availableViews;
        private IEnumerable<string> viewLocations;
        private IEnumerable<string> viewExtensions;
    }
}
