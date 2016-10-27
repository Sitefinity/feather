using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.HealthMonitoring;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Compilation
{
    /// <summary>
    /// This class represents a <see cref="RazorView"/> which allows measurement of its compilation performance.
    /// </summary>
    /// <seealso cref="System.Web.Mvc.RazorView" />
    internal class CompilationPerformanceRazorView : RazorView
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationPerformanceRazorView"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="layoutPath">The layout or master page.</param>
        /// <param name="runViewStartPages">A value that indicates whether view start files should be executed before the view.</param>
        /// <param name="viewStartFileExtensions">The set of extensions that will be used when looking up view start files.</param>
        public CompilationPerformanceRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions) :
            this(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationPerformanceRazorView"/> class.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="layoutPath">The layout or master page.</param>
        /// <param name="runViewStartPages">A value that indicates whether view start files should be executed before the view.</param>
        /// <param name="viewStartFileExtensions">The set of extensions that will be used when looking up view start files.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        public CompilationPerformanceRazorView(ControllerContext controllerContext, string viewPath, string layoutPath, bool runViewStartPages, IEnumerable<string> viewStartFileExtensions, IViewPageActivator viewPageActivator)
            : base(controllerContext, viewPath, layoutPath, runViewStartPages, viewStartFileExtensions, viewPageActivator)
        {
            this.controllerContext = controllerContext;
            this.viewPageActivator = viewPageActivator;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Renders the specified view context by using the specified the writer object.
        /// </summary>
        /// <param name="viewContext">Information related to rendering a view, such as view data, temporary data, and form context.</param>
        /// <param name="writer">The writer object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void Render(ViewContext viewContext, TextWriter writer)
        {
            try
            {
                if (this.ShouldMeasurePerformance(this.ViewPath))
                    this.RenderWithPerformanceMeasurement(viewContext, writer);
            }
            catch
            {
                base.Render(viewContext, writer);
            }
        }

        #endregion

        #region Private Methods

        private void RenderWithPerformanceMeasurement(ViewContext viewContext, TextWriter writer)
        {
            if (viewContext == null)
                throw new ArgumentNullException("viewContext");

            Type compiledType = null;

            using (this.GetMethodPerformanceRegion(viewContext))
                compiledType = System.Web.Compilation.BuildManager.GetCompiledType(this.ViewPath);

            object obj = null;
            if (compiledType != null)
                obj = this.viewPageActivator.Create(this.controllerContext, compiledType);

            if (obj == null)
            {
                string message = string.Format(CultureInfo.CurrentCulture, CompilationPerformanceRazorView.CshtmlViewCouldNotBeCreated, new[] { this.ViewPath });
                throw new InvalidOperationException(message);
            }

            this.RenderView(viewContext, writer, obj);
        }

        private bool ShouldMeasurePerformance(string virtualPath)
        {
            bool fileExists = HostingEnvironment.VirtualPathProvider.FileExists(virtualPath);
            bool isCompiled = System.Web.Compilation.BuildManager.GetCachedBuildDependencySet(System.Web.HttpContext.Current, virtualPath) != null;

            return fileExists && !isCompiled;
        }

        private MethodPerformanceRegion GetMethodPerformanceRegion(ViewContext viewContext)
        {
            PageSiteNode pageNode = null;
            if (SystemManager.HttpContextItems.Contains(SiteMapBase.CurrentNodeKey))
                pageNode = SystemManager.HttpContextItems[SiteMapBase.CurrentNodeKey] as PageSiteNode;

            int slashIndex = this.ViewPath.LastIndexOf('/');
            var fullViewName = this.ViewPath.Substring(slashIndex + 1);

            int hashIndex = fullViewName.IndexOf('#');
            var viewName = hashIndex < 0 ? fullViewName : fullViewName.Substring(0, hashIndex);

            var actionName = (string)viewContext.RequestContext.RouteData.Values["action"];
            var virtualPath = hashIndex < 0 ? this.ViewPath : this.ViewPath.Substring(0, slashIndex + hashIndex + 1);
            var packageManager = new PackageManager();
            var resourcePackage = packageManager.GetCurrentPackage();
            var controllerName = viewContext.Controller.GetType().FullName;
            var widgetName = this.GetWidgetName(this.controllerContext);

            var isBackendRequest = bool.Parse(SystemManager.CurrentHttpContext.Items[SystemManager.IsBackendRequestKey].ToString());
            var rootNodeId = isBackendRequest ? SiteInitializer.BackendRootNodeId : SiteInitializer.CurrentFrontendRootNodeId;
            var siteId = SystemManager.CurrentContext.CurrentSite.Id;
            var machineName = Environment.MachineName;

            var key = string.Format("Compile view \"{0}\" of widget \"{1}\"", fullViewName, widgetName);
            var data = new Dictionary<string, object>()
            {
                { CompilationPerformanceRazorView.ViewNameKey, viewName },
                { CompilationPerformanceRazorView.PageIdKey, pageNode.PageId },
                { CompilationPerformanceRazorView.ResourcePackageKey, resourcePackage ?? string.Empty },
                { CompilationPerformanceRazorView.ActionNameKey, actionName },
                { CompilationPerformanceRazorView.ControllerNameKey, controllerName },
                { CompilationPerformanceRazorView.WidgetNameKey, widgetName ?? string.Empty },
                { CompilationPerformanceRazorView.VirtualPathKey, this.ViewPath },
                { CompilationPerformanceRazorView.MachineNameKey, machineName },
                { CompilationPerformanceRazorView.SiteIdKey, siteId },
                { CompilationPerformanceRazorView.RootNodeIdKey, rootNodeId },
                { CompilationPerformanceRazorView.SourceKey, virtualPath }
            };

            return new MethodPerformanceRegion(key, CompilationPerformanceRazorView.ViewCompilationCategory, data);
        }

        private string GetWidgetName(ControllerContext context)
        {
            var controllerType = context.Controller.GetType();
            var controllerInfo = ControllerStore.Controllers()
                .SingleOrDefault(c => c.ControllerType == controllerType);

            if (controllerInfo == null)
                return string.Empty;

            return controllerInfo.DefaultToolboxItemTitle;
        }

        #endregion

        #region Fields and Constants

        private ControllerContext controllerContext;
        private IViewPageActivator viewPageActivator;

        internal const string ViewCompilationCategory = "ViewCompilation";

        internal const string ViewNameKey = "View";
        internal const string PageIdKey = "PageId";
        internal const string ResourcePackageKey = "ResourcePackage";
        internal const string ActionNameKey = "Action";
        internal const string ControllerNameKey = "Controller";
        internal const string WidgetNameKey = "Widget";
        internal const string VirtualPathKey = "VirtualPath";
        internal const string MachineNameKey = "MachineName";
        internal const string SiteIdKey = "SiteId";
        internal const string RootNodeIdKey = "RootNodeId";
        internal const string SourceKey = "Source";

        private const string CshtmlViewCouldNotBeCreated = "The view {0} could not be created.";

        #endregion
    }
}
