using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Builder.Model;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Provides extension methods for MVC controllers.
    /// </summary>
    public static class ControllerExtensions
    {
        #region Public methods

        /// <summary>
        /// Updates the view engines collection of the given <paramref name="controller"/> by making the engines aware of the controller's container virtual path.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <exception cref="System.ArgumentNullException">controller</exception>
        /// <param name="pathTransformations">Transformations that have to be applied to each view engine search path.</param>
        public static void UpdateViewEnginesCollection(this Controller controller, IList<Func<string, string>> pathTransformations)
        {
            if (pathTransformations == null)
                throw new ArgumentNullException("pathTransformations");

            if (controller == null)
                throw new ArgumentNullException("controller");

            var viewEngines = new ViewEngineCollection();

            foreach (var globalEngine in ViewEngines.Engines)
            {
                var vppEngine = globalEngine as VirtualPathProviderViewEngine;
                var newEngine = vppEngine != null ? ControllerExtensions.GetViewEngine(vppEngine, pathTransformations) : globalEngine;
                viewEngines.Add(newEngine);
            }

            controller.ViewEngineCollection = viewEngines;
        }

        /// <summary>
        /// Gets the partial views that are available to the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static IEnumerable<string> GetPartialViews(this Controller controller)
        {
            var viewLocations = ControllerExtensions.GetPartialViewLocations(controller);
            return ControllerExtensions.GetViews(controller, viewLocations);
        }

        /// <summary>
        /// Gets the views that are available to the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static IEnumerable<string> GetViews(this Controller controller)
        {
            var viewLocations = ControllerExtensions.GetViewLocations(controller);
            return ControllerExtensions.GetViews(controller, viewLocations);
        }

        /// <summary>
        /// Adds cache dependencies for the current response.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="keys">The cache dependency keys to be added.</param>
        public static void AddCacheDependencies(this Controller controller, IEnumerable<CacheDependencyKey> keys)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (keys == null)
                throw new ArgumentNullException("keys");

            if (SystemManager.CurrentHttpContext == null)
                throw new InvalidOperationException("Current HttpContext is null. Cannot add cache dependencies.");

            IList<CacheDependencyKey> dependencies = null;
            if (SystemManager.CurrentHttpContext.Items.Contains(PageCacheDependencyKeys.PageData))
            {
                dependencies = SystemManager.CurrentHttpContext.Items[PageCacheDependencyKeys.PageData] as IList<CacheDependencyKey>;
            }
            
            if (dependencies == null)
            {
                dependencies = new List<CacheDependencyKey>();
                SystemManager.CurrentHttpContext.Items.Add(PageCacheDependencyKeys.PageData, dependencies);
            }

            foreach (var key in keys)
                dependencies.Add(key);
        }

        /// <summary>
        /// Gets the partial view paths of the given controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static IEnumerable<string> GetPartialViewLocations(this Controller controller)
        {
            return ControllerExtensions.GetControllerViewEngineLocations(controller, v => v.PartialViewLocationFormats);
        }

        /// <summary>
        /// Gets the URL of the current page.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>URL of the currents page without a trailing slash.</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "controller")]
        [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings")]
        public static string GetCurrentPageUrl(this IController controller)
        {
            var currentSiteMap = (SiteMapBase)SitefinitySiteMap.GetCurrentProvider();
            var currentNode = currentSiteMap.CurrentNode;
            var url = string.Empty;

            if (currentNode != null)
            {
                url = UrlPath.ResolveUrl(currentNode.Url, absolute: false, removeTrailingSlash: true);
            }

            return url;
        }

        /// <summary>
        /// Gets the type of the dynamic content that is inferred for the given controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>The dynamic module type.</returns>
        public static DynamicModuleType GetDynamicContentType(this ControllerBase controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (controller.ControllerContext == null || controller.ControllerContext.RouteData == null)
                return null;

            var controllerName = controller.ControllerContext.RouteData.Values["controller"];
            var moduleProvider = ModuleBuilderManager.GetManager().Provider;
            var dynamicContentType = moduleProvider.GetDynamicModules()
                .Where(m => m.Status == DynamicModuleStatus.Active)
                .Join(moduleProvider.GetDynamicModuleTypes().Where(t => t.TypeName == controllerName), m => m.Id, t => t.ParentModuleId, (m, t) => t)
                .FirstOrDefault();

            return dynamicContentType;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets a view engine that is a clone of the given <paramref name="viewEngine"/> and has enhanced search locations.
        /// </summary>
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="pathTransformations">Transformations that have to be applied to each view engine search path.</param>
        private static IViewEngine GetViewEngine(VirtualPathProviderViewEngine viewEngine,  IList<Func<string, string>> pathTransformations)
        {
            var newEngine = (VirtualPathProviderViewEngine)Activator.CreateInstance(viewEngine.GetType());
            newEngine.AreaViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaViewLocationFormats, pathTransformations);
            newEngine.AreaMasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.AreaPartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.ViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.ViewLocationFormats, pathTransformations);
            newEngine.MasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.MasterLocationFormats, pathTransformations);
            newEngine.PartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.PartialViewLocationFormats, pathTransformations);

            newEngine.ViewLocationCache = new VoidViewLocationCache();

            return newEngine;
        }

        private static string[] AppendControllerVirtualPath(string[] originalPaths, IList<Func<string, string>> pathTransformations)
        {
            var result = originalPaths.ToList();
            foreach (var transform in pathTransformations)
            {
                result.AddRange(originalPaths.Select(transform));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Gets the view paths of the given controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private static IEnumerable<string> GetViewLocations(Controller controller)
        {
            return ControllerExtensions.GetControllerViewEngineLocations(controller, v => v.ViewLocationFormats);
        }

        private static IEnumerable<string> GetControllerViewEngineLocations(Controller controller, Func<VirtualPathProviderViewEngine, string[]> locationExtractor)
        {
            return controller.ViewEngineCollection.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(v => locationExtractor(v))
                .Distinct()
                .Select(v => v.Replace("{1}", FrontendManager.ControllerFactory.GetControllerName(controller.GetType())))
                .Select(VirtualPathUtility.GetDirectory)
                .Distinct();
        }

        /// <summary>
        /// Gets the file extensions that this controller will recognize when resolving view templates.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private static IEnumerable<string> GetViewFileExtensions(Controller controller)
        {
            return controller.ViewEngineCollection.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(v => v.FileExtensions.Select(e => "." + e))
                .Distinct();
        }

        private static IEnumerable<string> GetViews(Controller controller, IEnumerable<string> viewLocations)
        {
            var viewExtensions = ControllerExtensions.GetViewFileExtensions(controller);
            var widgetName = controller.RouteData != null ? controller.RouteData.Values["widgetName"] as string : null;

            var baseFiles = ControllerExtensions.GetViewsForAssembly(controller.GetType().Assembly, viewLocations, viewExtensions);
            if (!widgetName.IsNullOrEmpty())
            {
                var widgetAssembly = FrontendManager.ControllerFactory.ResolveControllerType(widgetName).Assembly;
                var widgetFiles = ControllerExtensions.GetViewsForAssembly(widgetAssembly, viewLocations, viewExtensions);
                return baseFiles.Union(widgetFiles);
            }

            return baseFiles;
        }

        private static IEnumerable<string> GetViewsForAssembly(Assembly assembly, IEnumerable<string> viewLocations, IEnumerable<string> viewExtensions)
        {
            var pathDef = FrontendManager.VirtualPathBuilder.GetPathDefinition(assembly);
            return viewLocations
                .SelectMany(l => ControllerExtensions.GetViewsForPath(pathDef, l, viewExtensions))
                .Distinct();
        }

        private static IEnumerable<string> GetViewsForPath(PathDefinition definition, string path, IEnumerable<string> viewExtensions)
        {
            var files = ObjectFactory.Resolve<IResourceResolverStrategy>().GetFiles(definition, path);

            if (files != null)
            {
                return files
                    .Where(f => viewExtensions.Any(e => f.EndsWith(e, StringComparison.Ordinal)))
                    .Select(VirtualPathUtility.GetFileName)
                    .Select(System.IO.Path.GetFileNameWithoutExtension)
                    .Distinct();
            }

            return new string[] { };
        }

        #endregion
    }
}
