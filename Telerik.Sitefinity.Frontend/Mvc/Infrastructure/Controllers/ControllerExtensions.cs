using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            if (controller == null)
                throw new ArgumentNullException("controller");

            var viewEngines = new ViewEngineCollection();
            foreach (var globalEngine in ViewEngines.Engines)
            {
                var vppEngine = globalEngine as VirtualPathProviderViewEngine;
                var newEngine = vppEngine != null ? ControllerExtensions.GetViewEngine(vppEngine, controller, pathTransformations) : globalEngine;
                viewEngines.Add(newEngine);
            }
            controller.ViewEngineCollection = viewEngines;
        }

        /// <summary>
        /// Disables the view location cache.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static void DisableViewLocationCache(this Controller controller)
        {
            foreach (var engine in controller.ViewEngineCollection)
            {
                var vppEngine = engine as VirtualPathProviderViewEngine;
                if (vppEngine != null)
                {
                    vppEngine.ViewLocationCache = new VoidViewLocationCache();
                }
            }
        }

        /// <summary>
        /// Gets the partial view paths of the given controller.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static IEnumerable<string> GetPartialViewLocations(this Controller controller)
        {
            return controller.ViewEngineCollection.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(v => v.PartialViewLocationFormats)
                .Distinct()
                .Select(v => v.Replace("{1}", FrontendManager.ControllerFactory.GetControllerName(controller.GetType())))
                .Select(VirtualPathUtility.GetDirectory)
                .Distinct();
        }

        /// <summary>
        /// Gets the file extensions that this controller will recognize when resolving view templates.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static IEnumerable<string> GetViewFileExtensions(this Controller controller)
        {
            return controller.ViewEngineCollection.OfType<VirtualPathProviderViewEngine>()
                .SelectMany(v => v.FileExtensions)
                .Distinct();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets a view engine that is a clone of the given <paramref name="viewEngine"/> and has enhanced search locations.
        /// </summary>
        /// <param name="viewEngine">The view engine.</param>
        /// <param name="controller">The controller.</param>
        /// <param name="pathTransformations">Transformations that have to be applied to each view engine search path.</param>
        private static IViewEngine GetViewEngine(VirtualPathProviderViewEngine viewEngine, Controller controller, IList<Func<string, string>> pathTransformations)
        {
            var newEngine = (VirtualPathProviderViewEngine)Activator.CreateInstance(viewEngine.GetType());
            newEngine.AreaViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaViewLocationFormats, pathTransformations);
            newEngine.AreaMasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.AreaPartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.AreaPartialViewLocationFormats, pathTransformations);
            newEngine.ViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.ViewLocationFormats, pathTransformations);
            newEngine.MasterLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.MasterLocationFormats, pathTransformations);
            newEngine.PartialViewLocationFormats = ControllerExtensions.AppendControllerVirtualPath(viewEngine.PartialViewLocationFormats, pathTransformations);
            
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

        #endregion
    }
}
