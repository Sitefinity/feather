using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.HealthMonitoring;
using Telerik.Sitefinity.Modules.Forms;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Pages.Model;
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
        public override void Render(ViewContext viewContext, TextWriter writer)
        {
            if (!this.ShouldMeasurePerformance(this.ViewPath) || !this.RenderWithPerformanceMeasurement(viewContext, writer))
                base.Render(viewContext, writer);
        }

        #endregion

        #region Private Methods

        private bool RenderWithPerformanceMeasurement(ViewContext viewContext, TextWriter writer)
        {
            if (viewContext == null)
                return false;

            var region = this.GetMethodPerformanceRegion(viewContext);
            if (region == null)
                return false;

            Type compiledType = null;
            try
            {
                compiledType = System.Web.Compilation.BuildManager.GetCompiledType(this.ViewPath);
            }
            finally
            {
                region.Dispose();
            }

            if (compiledType == null)
                return false;

            var obj = this.viewPageActivator.Create(this.controllerContext, compiledType);
            if (obj == null)
                return false;

            this.RenderView(viewContext, writer, obj);

            return true;
        }

        private bool ShouldMeasurePerformance(string virtualPath)
        {
            if (virtualPath == null || HttpContext.Current == null)
                return false;

            return HostingEnvironment.VirtualPathProvider.FileExists(virtualPath) && BuildManager.GetCachedBuildDependencySet(HttpContext.Current, virtualPath) == null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The type of the thrown exception is not of interest in this scenario. We are only interested in marking the start of the performance measurement as failed.")]
        private IDisposable GetMethodPerformanceRegion(ViewContext viewContext)
        {
            if (SystemManager.HttpContextItems == null || !SystemManager.HttpContextItems.Contains(SiteMapBase.CurrentNodeKey) || !(SystemManager.HttpContextItems[SiteMapBase.CurrentNodeKey] is PageSiteNode))
                return null;

            var pageNode = (PageSiteNode)SystemManager.HttpContextItems[SiteMapBase.CurrentNodeKey];

            try
            {
                int slashIndex = this.ViewPath.LastIndexOf('/');
                var fullViewName = this.ViewPath.Substring(slashIndex + 1);

                int hashIndex = fullViewName.IndexOf('#');
                var viewName = hashIndex < 0 ? fullViewName : fullViewName.Substring(0, hashIndex);

                var actionName = (string)viewContext.RequestContext.RouteData.Values["action"];
                var virtualPath = hashIndex < 0 ? this.ViewPath : this.ViewPath.Substring(0, slashIndex + hashIndex + 1);
                var source = this.GetSource(this.ViewPath);
                var packageManager = new PackageManager();
                var resourcePackage = packageManager.GetCurrentPackage();
                var controllerName = viewContext.Controller.GetType().FullName;
                var widgetName = this.GetWidgetName(this.controllerContext);
                if (this.ViewPath.StartsWith(CompilationPerformanceRazorView.FormsResolverPath, StringComparison.OrdinalIgnoreCase) && viewName.EndsWith(CompilationPerformanceRazorView.RazorViewSuffix, StringComparison.OrdinalIgnoreCase))
                {
                    Guid formId;
                    var formIdString = viewName.Left(viewName.Length - CompilationPerformanceRazorView.RazorViewSuffix.Length);
                    if (Guid.TryParse(formIdString, out formId))
                    {
                        var form = FormsManager.GetManager().GetForms().FirstOrDefault(f => f.Id == formId);
                        if (form != null)
                        {
                            viewName = form.Title + " (" + formIdString + ")";
                        }
                    }
                }

                var isBackendRequest = bool.Parse(SystemManager.CurrentHttpContext.Items[SystemManager.IsBackendRequestKey].ToString());
                var rootNodeId = isBackendRequest ? SiteInitializer.BackendRootNodeId : SiteInitializer.CurrentFrontendRootNodeId;
                var siteId = SystemManager.CurrentContext.CurrentSite.Id;
                var machineName = Environment.MachineName;

                var key = string.Format("Compile view \"{0}\" of controller \"{1}\"", fullViewName, controllerName);
                var data = new Dictionary<string, object>()
                {
                    { CompilationPerformanceRazorView.ViewNameKey, viewName },
                    { CompilationPerformanceRazorView.PageIdKey, pageNode.PageId },
                    { CompilationPerformanceRazorView.PageTitleKey, pageNode.Title },
                    { CompilationPerformanceRazorView.ResourcePackageKey, resourcePackage ?? string.Empty },
                    { CompilationPerformanceRazorView.ActionNameKey, actionName },
                    { CompilationPerformanceRazorView.ControllerNameKey, controllerName },
                    { CompilationPerformanceRazorView.WidgetNameKey, widgetName ?? string.Empty },
                    { CompilationPerformanceRazorView.VirtualPathKey, this.ViewPath },
                    { CompilationPerformanceRazorView.MachineNameKey, machineName },
                    { CompilationPerformanceRazorView.SiteIdKey, siteId },
                    { CompilationPerformanceRazorView.RootNodeIdKey, rootNodeId },
                    { CompilationPerformanceRazorView.SourceKey, virtualPath },
                    { CompilationPerformanceRazorView.ViewSourceKey, source }
                };

                return (IDisposable)Activator.CreateInstance(CompilationPerformanceRazorView.methodPerformanceRegionType, new object[] { key, CompilationPerformanceRazorView.ViewCompilationCategory, data });
            }
            catch
            {
                return null;
            }
        }

        private string GetSource(string viewPath)
        {
            var cacheDep = HostingEnvironment.VirtualPathProvider.GetCacheDependency(viewPath, new object[0], DateTime.UtcNow);
            var getFileDependencies = typeof(CacheDependency).GetMethod("GetFileDependencies", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var getDependencyArray = typeof(AggregateCacheDependency).GetMethod("GetDependencyArray", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            while (cacheDep is AggregateCacheDependency)
            {
                var deps = getDependencyArray.Invoke(cacheDep, null) as CacheDependency[];
                if (deps == null || deps.Length == 0)
                    break;

                cacheDep = deps[deps.Length - 1];
            }

            if (cacheDep is Abstractions.VirtualPath.ControlPresentationCacheDependency)
            {
                var title = this.ExtractTemplateTitle((Abstractions.VirtualPath.ControlPresentationCacheDependency)cacheDep);
                if (title != null)
                {
                    return "Widget template: " + title + " (DB)";
                }
            }

            var files = getFileDependencies.Invoke(cacheDep, null) as string[];
            if (files != null && files.Length > 0)
                return files[files.Length - 1].Replace(HostingEnvironment.ApplicationPhysicalPath, "~\\");
            else
                return viewPath;
        }

        private string ExtractTemplateTitle(Abstractions.VirtualPath.ControlPresentationCacheDependency cacheDep)
        {
            var cacheDepId = cacheDep.GetUniqueID().Right(cacheDep.GetUniqueID().Length - cacheDep.UniquePrefix.Length - 1);
            Guid id;
            if (!Guid.TryParse(cacheDepId, out id))
                return null;

            var manager = PageManager.GetManager();
            string title;
            using (new Data.ElevatedModeRegion(manager))
            {
                title = manager.GetPresentationItem<ControlPresentation>(id).Name;
            }

            return title;
        }

        private string GetWidgetName(ControllerContext context)
        {
            var controllerType = context.Controller.GetType();

            // Check in controller store
            var controllerInfo = ControllerStore.Controllers()
                .FirstOrDefault(c => c.ControllerType == controllerType);

            if (controllerInfo != null && !controllerInfo.DefaultToolboxItemTitle.IsNullOrEmpty())
                return controllerInfo.DefaultToolboxItemTitle;

            // Check for dynamic type
            var modelProperty = controllerType.GetProperty("Model");
            if (modelProperty != null)
            {
                var model = modelProperty.GetValue(context.Controller) as ContentModelBase;
                if (model != null)
                {
                    var contentType = model.ContentType;
                    var manager = ModuleBuilderManager.GetManager().Provider;
                    var dynamicType = manager.GetDynamicModuleTypes().FirstOrDefault(t => t.TypeName == contentType.Name && t.TypeNamespace == contentType.Namespace);
                    if (dynamicType != null)
                        return dynamicType.DisplayName;
                }
            }

            if (this.ViewPath.StartsWith(CompilationPerformanceRazorView.LayoutVirtualPathBeginning, StringComparison.OrdinalIgnoreCase))
                return CompilationPerformanceRazorView.Layout;

            var controllerName = FrontendManager.ControllerFactory.GetControllerName(controllerType);

            return controllerName;
        }

        #endregion

        #region Fields and Constants

        private ControllerContext controllerContext;
        private IViewPageActivator viewPageActivator;

        internal const string ViewCompilationCategory = "ViewCompilation";

        internal const string ViewNameKey = "View";
        internal const string PageIdKey = "PageId";
        internal const string PageTitleKey = "PageTitle";
        internal const string ResourcePackageKey = "ResourcePackage";
        internal const string ActionNameKey = "Action";
        internal const string ControllerNameKey = "Controller";
        internal const string WidgetNameKey = "Widget";
        internal const string VirtualPathKey = "VirtualPath";
        internal const string MachineNameKey = "MachineName";
        internal const string SiteIdKey = "SiteId";
        internal const string RootNodeIdKey = "RootNodeId";
        internal const string SourceKey = "Source";
        internal const string ViewSourceKey = "ViewSource";

        private const string LayoutVirtualPathBeginning = "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/Mvc/Views/Layouts";
        private const string Layout = "Layout";
        private const string FormsResolverPath = "~/Mvc-Form-View/";
        private const string RazorViewSuffix = ".cshtml";

        private static Type methodPerformanceRegionType = Assembly.GetAssembly(typeof(SystemManager)).GetType("Telerik.Sitefinity.HealthMonitoring.MethodPerformanceRegion", false);

        #endregion
    }
}
