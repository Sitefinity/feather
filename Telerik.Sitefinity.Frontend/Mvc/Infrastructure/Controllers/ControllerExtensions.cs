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
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

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
        /// <param name="pathTransformationsFunc">Transformations func that have to be applied to each view engine search path.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static void UpdateViewEnginesCollection(this Controller controller, Func<IList<Func<string, string>>> pathTransformationsFunc)
        {
            if (pathTransformationsFunc == null)
                throw new ArgumentNullException("pathTransformationsFunc");

            if (controller == null)
                throw new ArgumentNullException("controller");

            controller.ViewEngineCollection = GetViewEngineCollection(controller, pathTransformationsFunc);
        }

        /// <summary>
        /// Updates the view engines collection of the given <paramref name="controller"/> by making the engines aware of the controller's container virtual path.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <exception cref="System.ArgumentNullException">controller</exception>
        /// <param name="pathTransformations">Transformations that have to be applied to each view engine search path.</param>
        [Obsolete("Use the UpdateViewEnginesCollection with the Func<IList<Func<string, string>>> overload")]
        public static void UpdateViewEnginesCollection(this Controller controller, IList<Func<string, string>> pathTransformations)
        {
            if (pathTransformations == null)
                throw new ArgumentNullException("pathTransformations");

            if (controller == null)
                throw new ArgumentNullException("controller");

            controller.ViewEngineCollection = GetViewEngineCollection(controller, () => pathTransformations);
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

            var controllerName = controller.ControllerContext.RouteData.Values["controller"] as string;
            return controller.GetDynamicContentType(controllerName);
        }

        /// <summary>
        /// Gets the type of the dynamic content that is inferred for the given controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns>
        /// The dynamic module type.
        /// </returns>
        public static DynamicModuleType GetDynamicContentType(this ControllerBase controller, string controllerName)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (controllerName == null)
                throw new ArgumentNullException("controllerName");

            return ControllerExtensions.GetDynamicContentType(controllerName);
        }

        /// <summary>
        /// Gets the type of the dynamic content that is inferred for the given controller name.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <returns>
        /// The dynamic module type.
        /// </returns>
        public static DynamicModuleType GetDynamicContentType(string controllerName)
        {
            if (controllerName == null)
                throw new ArgumentNullException("controllerName");

            if (SystemManager.GetModule("ModuleBuilder") == null)
                return null;

            var moduleProvider = ModuleBuilderManager.GetManager().Provider;
            var dynamicContentType = moduleProvider.GetDynamicModules()
                .Where(m => m.Status == DynamicModuleStatus.Active)
                .Join(moduleProvider.GetDynamicModuleTypes().Where(t => t.TypeName == controllerName), m => m.Id, t => t.ParentModuleId, (m, t) => t)
                .FirstOrDefault();

            return dynamicContentType;
        }

        /// <summary>
        /// Determines whether this controller will produce output when rendered in-memory by the search engine.
        /// </summary>
        public static IndexRenderModes GetIndexRenderMode(this IController controller)
        {
            if (controller == null) 
                throw new ArgumentNullException("controller");

            var attribute = controller.GetType().GetCustomAttributes(true).OfType<IndexRenderModeAttribute>().LastOrDefault();
            return attribute == null ? IndexRenderModes.Normal : attribute.Mode;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets an already cached view engine collection from the dictionary or builds a new one.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="pathTransformationsFunc">The path transformations function.</param>
        private static ViewEngineCollection GetViewEngineCollection(Controller controller, Func<IList<Func<string, string>>> pathTransformationsFunc)
        {
            var key = ControllerExtensions.GetKey(controller);

            if (!ControllerExtensions.ViewEngineCollections.ContainsKey(key))
            {
                lock (ControllerExtensions.ViewEngineCollections)
                {
                    if (!ControllerExtensions.ViewEngineCollections.ContainsKey(key))
                    {
                        var viewEngineCollection = ControllerExtensions.BuildViewEngineCollection(pathTransformationsFunc());
                        ControllerExtensions.ViewEngineCollections.Add(key, viewEngineCollection);
                    }
                }
            }

            return ControllerExtensions.ViewEngineCollections[key];
        }

        /// <summary>
        /// Gets the key to be used in the view engine collection dictionary based on the controller type and the widget name.
        /// </summary>
        /// <param name="controller">The controller.</param>
        private static string GetKey(Controller controller)
        {
            const string WidgetNameKey = "widgetName";

            var key = controller.GetType().FullName;

            if (controller.RouteData != null && controller.RouteData.Values != null && controller.RouteData.Values.ContainsKey(WidgetNameKey))
            {
                var widgetName = (string)controller.RouteData.Values[WidgetNameKey];
                key = string.Format("{0}-{1}", key, widgetName);
            }

            var currentPackage = new PackageManager().GetCurrentPackage();
            if (!currentPackage.IsNullOrEmpty())
            {
                key += "-" + currentPackage;
            }

            return key;
        }

        /// <summary>
        /// Builds the view engine collection by applying the passed path transformations to each view engine.
        /// </summary>
        /// <param name="pathTransformations">The path transformations.</param>
        private static ViewEngineCollection BuildViewEngineCollection(IList<Func<string, string>> pathTransformations)
        {
            var viewEngines = new ViewEngineCollection();

            foreach (var globalEngine in ViewEngines.Engines)
            {
                var vppEngine = globalEngine as VirtualPathProviderViewEngine;
                var newEngine = vppEngine != null ? ControllerExtensions.GetViewEngine(vppEngine, pathTransformations) : globalEngine;
                viewEngines.Add(newEngine);
            }

            return viewEngines;
        }

        /// <summary>
        /// Gets a view engine that is a clone of the given <paramref name="viewEngine"/> and has enhanced search locations.
        /// </summary>
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="pathTransformations">Transformations that have to be applied to each view engine search path.</param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        private static IViewEngine GetViewEngine(VirtualPathProviderViewEngine viewEngine, IList<Func<string, string>> pathTransformations)
        {
            var newEngine = (VirtualPathProviderViewEngine)Activator.CreateInstance(viewEngine.GetType());
            newEngine.AreaViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaViewLocationFormats, pathTransformations);
            newEngine.AreaMasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.AreaPartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.ViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.ViewLocationFormats, pathTransformations);
            newEngine.MasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.MasterLocationFormats, pathTransformations);
            newEngine.PartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.PartialViewLocationFormats, pathTransformations);

            newEngine.ViewLocationCache = DefaultViewLocationCache.Null;

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
            string controllerName;
            if (controller.RouteData != null && controller.RouteData.Values["controller"] as string != null)
                controllerName = controller.RouteData.Values["controller"] as string;
            else
                controllerName = FrontendManager.ControllerFactory.GetControllerName(controller.GetType());

            return controller.ViewEngineCollection.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(v => locationExtractor(v))
                .Distinct()
                .Select(v => v.Replace("{1}", controllerName))
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
                var returnResult = files
                    .Where(f => viewExtensions.Any(e => f.EndsWith(e, StringComparison.Ordinal)))
                    .Select(VirtualPathUtility.GetFileName)
                    .Select(System.IO.Path.GetFileNameWithoutExtension)
                    .Distinct();

                return returnResult;
            }

            return new string[] { };
        }

        #endregion

        #region Private Fields

        private static readonly Dictionary<string, ViewEngineCollection> ViewEngineCollections = new Dictionary<string, ViewEngineCollection>();

        #endregion
    }
}
