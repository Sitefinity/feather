using ServiceStack.Text;
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
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;
using Telerik.Sitefinity.Web.UI.ContentUI.Enums;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Provides extension methods for MVC controllers.
    /// </summary>
    public static class ControllerExtensions
    {
        #region Public methods

        /// <summary>
        /// Determines whether the controller should return the details view.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="contentViewDisplayMode">Display mode of the view.</param>
        /// <param name="viewModel">A view model containing list of selected items.</param>
        /// <returns>A value indicating whether the controller should return the details view.</returns>
        public static bool ShouldReturnDetails(this Controller controller, ContentViewDisplayMode contentViewDisplayMode, ContentListViewModel viewModel)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");
            
            if (contentViewDisplayMode == ContentViewDisplayMode.Detail && viewModel != null && viewModel.Items.Count() == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates the view engines collection of the given <paramref name="controller"/> by making the engines aware of the controller's container virtual path.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="pathTransformationsFunc">Transformations func that have to be applied to each view engine search path.</param>
        /// <exception cref="System.ArgumentNullException">controller</exception>
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
        /// <param name="fullViewPaths">The full view paths.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fullViewPaths")]
        public static IEnumerable<string> GetPartialViews(this Controller controller, IList<string> fullViewPaths = null)
        {
            var viewLocations = ControllerExtensions.GetPartialViewLocations(controller);
            return ControllerExtensions.GetViews(controller, viewLocations);
        }

        /// <summary>
        /// Gets the views that are available to the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="fullViewPaths">The full view paths.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fullViewPaths")]
        public static IEnumerable<string> GetViews(this Controller controller, IList<string> fullViewPaths = null)
        {
            return ControllerExtensions.GetViews(controller, fullViewPaths, null);
        }

        /// <summary>
        /// Gets the views that are available to the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="fullViewPaths">The full view paths.</param>
        /// <param name="moduleName">The name of dynamic module (if any). Default is null.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fullViewPaths")]
        public static IEnumerable<string> GetViews(this Controller controller, IList<string> fullViewPaths, string moduleName)
        {
            var viewLocations = ControllerExtensions.GetViewLocations(controller);
            return ControllerExtensions.GetViews(controller, viewLocations, moduleName);
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
        /// Adds cache dependencies for the current response.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="contentType">The type of the content.</param>
        /// <param name="providerName">The name of the provider.</param>
        public static void AddCacheVariations(this Controller controller, Type contentType, string providerName = null)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            if (contentType == null)
                throw new ArgumentNullException("contentType");

            PageRouteHandler.RegisterContentListCacheVariation(contentType, providerName);
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

            var moduleName = controller.ViewBag.ModuleName as string;

            return ControllerExtensions.GetDynamicContentType(controllerName, moduleName);
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
            return ControllerExtensions.GetDynamicContentType(controllerName, null);
        }

        /// <summary>
        /// Gets the type of the dynamic content that is inferred for the given controller name.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="moduleName">The name of the module. If empty or null will search all dynamic modules. Default value is null.</param>
        /// <returns>
        /// The dynamic module type.
        /// </returns>
        public static DynamicModuleType GetDynamicContentType(string controllerName, string moduleName)
        {
            return ControllerExtensions.FindDynamicContentTypes(controllerName, moduleName).FirstOrDefault();
        }

        /// <summary>
        /// Determines whether this controller will produce output when rendered in-memory by the search engine.
        /// </summary>
        public static IndexRenderModes GetIndexRenderMode(this IController controller)
        {
            if (controller == null)
                throw new ArgumentNullException("controller");

            var searchableControl = controller as ISearchIndexBehavior;
            if (searchableControl != null)
            {
                var exclude = searchableControl.ExcludeFromSearchIndex;
                if (exclude)
                    return IndexRenderModes.NoOutput;
                else
                    return IndexRenderModes.Normal;
            }

            var attribute = controller.GetType().GetCustomAttributes(true).OfType<IndexRenderModeAttribute>().LastOrDefault();
            return attribute == null ? IndexRenderModes.Normal : attribute.Mode;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Gets the type of the dynamic content that is inferred for the given controller name.
        /// </summary>
        /// <param name="controllerName">Name of the controller.</param>
        /// <param name="moduleName">The name of the module. If empty or null will search all dynamic modules.</param>
        /// <returns>
        /// The dynamic module types.
        /// </returns>
        internal static IQueryable<DynamicModuleType> FindDynamicContentTypes(string controllerName, string moduleName)
        {
            if (controllerName == null)
                throw new ArgumentNullException("controllerName");

            if (SystemManager.GetModule("ModuleBuilder") == null)
                return Enumerable.Empty<DynamicModuleType>().AsQueryable();

            // TODO: use ModuleBuilderManager.GetModules()
            var moduleProvider = ModuleBuilderManager.GetManager().Provider;

            var dynamicModuleTypes = moduleProvider.GetDynamicModuleTypes().Where(t => t.TypeName == controllerName);
            if (!moduleName.IsNullOrWhitespace())
            {
                dynamicModuleTypes = dynamicModuleTypes.Where(t => t.ModuleName == moduleName);
            }

            var dynamicContentTypes = moduleProvider.GetDynamicModules()
                .Where(m => m.Status == DynamicModuleStatus.Active)
                .Join(dynamicModuleTypes, m => m.Id, t => t.ParentModuleId, (m, t) => t);

            return dynamicContentTypes;
        }

        /// <summary>
        /// Gets the partial views that are available to the controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="viewFilesMappings">The view files mappings.</param>
        /// <param name="moduleName">The name of dynamic module name.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fullViewPaths")]
        internal static IEnumerable<string> GetPartialViews(this Controller controller, ref Dictionary<string, string> viewFilesMappings, string moduleName)
        {
            var viewLocations = ControllerExtensions.GetPartialViewLocations(controller);
            return ControllerExtensions.GetViews(controller, viewLocations, ref viewFilesMappings, moduleName);
        }

        /// <summary>
        /// Resolves the widget name of the dynamic widget controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <returns>
        /// The widget name.
        /// </returns>
        internal static string ResolveDynamicControllerWidgetName(this Controller controller)
        {
            if (controller == null ||
                controller.Request == null ||
                controller.Request.QueryString == null ||
                controller.RouteData == null ||
                !controller.RouteData.Values.ContainsKey("widgetName") ||
                (string)controller.RouteData.Values["widgetName"] != "DynamicContent")
            {
                return null;
            }

            var controlId = controller.Request.QueryStringGet("controlId") as string;
            Guid controlIdGuid;
            if (string.IsNullOrEmpty(controlId) || !Guid.TryParse(controlId, out controlIdGuid))
                return null;

            var pageManager = Telerik.Sitefinity.Modules.Pages.PageManager.GetManager();
            var controlObjectData = pageManager.GetControl<Telerik.Sitefinity.Pages.Model.ObjectData>(controlIdGuid);
            if (controlObjectData == null || controlObjectData.Properties == null)
                return null;

            var controllerWidgetProperty = controlObjectData.Properties.FirstOrDefault(x => x.Name == "WidgetName");
            if (controllerWidgetProperty == null)
                return null;

            return controllerWidgetProperty.Value;
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

                var dynamicControllerWidgetName = controller.ResolveDynamicControllerWidgetName();
                if (!string.IsNullOrEmpty(dynamicControllerWidgetName))
                    key = string.Format("{0}-{1}", key, dynamicControllerWidgetName);
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
                if (newEngine != null)
                {
                    viewEngines.Add(newEngine);
                }
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
            VirtualPathProviderViewEngine newEngine;
            var precompiledEngine = viewEngine as CompositePrecompiledMvcEngineWrapper;
            if (precompiledEngine != null)
            {
                if (!precompiledEngine.PackageName.IsNullOrEmpty() &&
                    !string.Equals(precompiledEngine.PackageName, new PackageManager().GetCurrentPackage(), StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                newEngine = precompiledEngine.Clone();
            }
            else
            {
                var viewEngineType = viewEngine.GetType();
                var defaultCtor = viewEngineType.GetConstructor(Type.EmptyTypes);
                if (defaultCtor != null)
                    newEngine = (VirtualPathProviderViewEngine)Activator.CreateInstance(viewEngineType);
                else
                    return null;
            }

            newEngine.ViewLocationCache = DefaultViewLocationCache.Null;

            newEngine.AreaViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaViewLocationFormats, pathTransformations);
            newEngine.AreaMasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.AreaPartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.ViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.ViewLocationFormats, pathTransformations);
            newEngine.MasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.MasterLocationFormats, pathTransformations);
            newEngine.PartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.PartialViewLocationFormats, pathTransformations);

            if (precompiledEngine != null)
            {
                newEngine.AreaViewLocationFormats = newEngine.AreaViewLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
                newEngine.AreaMasterLocationFormats = newEngine.AreaMasterLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
                newEngine.AreaPartialViewLocationFormats = newEngine.AreaPartialViewLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
                newEngine.ViewLocationFormats = newEngine.ViewLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
                newEngine.MasterLocationFormats = newEngine.MasterLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
                newEngine.PartialViewLocationFormats = newEngine.PartialViewLocationFormats.Select(p => FrontendManager.VirtualPathBuilder.RemoveParams(p)).ToArray();
            }

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

        private static IEnumerable<string> GetViews(Controller controller, IEnumerable<string> viewLocations, string moduleName = null)
        {
            var viewExtensions = ControllerExtensions.GetViewFileExtensions(controller);
            var widgetName = controller.RouteData != null ? controller.RouteData.Values["widgetName"] as string : null;

            var assembly = controller.GetType().Assembly;
            var baseFiles = ControllerExtensions.GetViewsForAssembly(assembly, viewLocations, viewExtensions, moduleName).ToList();

            foreach (var viewEngine in controller.ViewEngineCollection)
            {
                var compositeViewEngine = viewEngine as CompositePrecompiledMvcEngineWrapper;
                if (compositeViewEngine != null)
                {
                    var files = compositeViewEngine.GetViews(controller.ControllerContext, viewLocations);
                    baseFiles.AddRange(files);
                }
            }
            var filesForBasePath = baseFiles.Distinct();

            if (!widgetName.IsNullOrEmpty())
            {
                var widgetAssembly = FrontendManager.ControllerFactory.ResolveControllerType(widgetName).Assembly;
                var widgetFiles = ControllerExtensions.GetViewsForAssembly(widgetAssembly, viewLocations, viewExtensions);
                return filesForBasePath.Union(widgetFiles);
            }

            return filesForBasePath;
        }

        private static IEnumerable<string> GetViews(Controller controller, IEnumerable<string> viewLocations, ref Dictionary<string, string> viewFilesMappings, string moduleName = null)
        {
            var viewExtensions = ControllerExtensions.GetViewFileExtensions(controller);
            var widgetName = controller.RouteData != null ? controller.RouteData.Values["widgetName"] as string : null;

            var baseFiles = ControllerExtensions.GetViewsForAssembly(controller.GetType().Assembly, viewLocations, viewExtensions, 
                                                                     ref viewFilesMappings, moduleName);
            if (!widgetName.IsNullOrEmpty())
            {
                var widgetAssembly = FrontendManager.ControllerFactory.ResolveControllerType(widgetName).Assembly;
                var widgetFiles = ControllerExtensions.GetViewsForAssembly(widgetAssembly, viewLocations, viewExtensions, 
                                                                           ref viewFilesMappings, moduleName);
                return baseFiles.Union(widgetFiles);
            }

            return baseFiles;
        }

        private static IEnumerable<string> GetViewsForAssembly(Assembly assembly, IEnumerable<string> viewLocations, IEnumerable<string> viewExtensions, string moduleName = null)
        {
            var pathDef = FrontendManager.VirtualPathBuilder.GetPathDefinition(assembly, moduleName);
            return viewLocations
                .SelectMany(l => ControllerExtensions.GetViewsForPath(pathDef, l, viewExtensions))
                .Distinct();
        }

        private static IEnumerable<string> GetViewsForAssembly(Assembly assembly, IEnumerable<string> viewLocations, IEnumerable<string> viewExtensions, 
                                                               ref Dictionary<string, string> viewFilesMappings, string moduleName = null)
        {
            var pathDef = FrontendManager.VirtualPathBuilder.GetPathDefinition(assembly, moduleName);
            var views = new List<string>();

            foreach (var viewLocation in viewLocations)
            {
                views.AddRange(ControllerExtensions.GetViewsForPath(pathDef, viewLocation, viewExtensions, ref viewFilesMappings));
            }

            return views.Distinct();
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

        private static IEnumerable<string> GetViewsForPath(PathDefinition definition, string path, IEnumerable<string> viewExtensions, ref Dictionary<string, string> viewFilesMappings)
        {
            var views = new List<string>();
            var files = ObjectFactory.Resolve<IResourceResolverStrategy>().GetFiles(definition, path);

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (viewExtensions.Any(e => file.EndsWith(e, StringComparison.Ordinal)))
                    {
                        var fileName = VirtualPathUtility.GetFileName(file);
                        var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        if (!viewFilesMappings.ContainsKey(fileNameWithoutExtension))
                        {
                            viewFilesMappings.Add(fileNameWithoutExtension, file);
                            views.Add(fileNameWithoutExtension);
                        }
                    }
                }
            }

            return views;
        }

        #endregion

        #region Private Fields

        private static readonly Dictionary<string, ViewEngineCollection> ViewEngineCollections = new Dictionary<string, ViewEngineCollection>();

        #endregion
    }
}
