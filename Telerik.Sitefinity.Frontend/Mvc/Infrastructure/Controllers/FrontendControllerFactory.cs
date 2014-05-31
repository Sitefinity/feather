using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class extends the <see cref="SitefinityControllerFactory"/> by adding additional virtual paths for controller view engines.
    /// </summary>
    public class FrontendControllerFactory : SitefinityControllerFactory
    {
        #region Overridden members

        /// <summary>
        /// Creates the specified controller by using the specified request context.
        /// </summary>
        /// <returns>The controller.</returns>
        /// <param name="requestContext">
        /// The context of the HTTP request, which includes the HTTP context and route data.
        /// </param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="requestContext"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// The <paramref name="controllerName"/> parameter is null or empty.
        /// </exception>
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            var baseController = base.CreateController(requestContext, controllerName);
            var controller = baseController as Controller;
            if (controller != null)
            {
                this.EnhanceViewEngines(controller);
            }
            
            return baseController;
        }

        #endregion

        #region Private members

        private void EnhanceViewEngines(Controller controller)
        {
            var enhanceAttr = this.GetEnhanceAttribute(controller.GetType());
            if (!enhanceAttr.Disabled)
            {
                controller.UpdateViewEnginesCollection(FrontendControllerFactory.GetControllerPathTransformations(controller, enhanceAttr.VirtualPath));
            }
        }

        private EnhanceViewEnginesAttribute GetEnhanceAttribute(Type controllerType)
        {
            EnhanceViewEnginesAttribute enhanceAttr = controllerType.GetCustomAttributes(typeof(EnhanceViewEnginesAttribute), true).FirstOrDefault() as EnhanceViewEnginesAttribute;
            if (enhanceAttr != null)
            {
                return enhanceAttr;
            }
            else
            {
                enhanceAttr = new EnhanceViewEnginesAttribute();
                enhanceAttr.Disabled = !this.IsInDefaultMvcNamespace(controllerType);
                enhanceAttr.VirtualPath = FrontendControllerFactory.AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType.Assembly));
                return enhanceAttr;
            }
        }

        private bool IsInDefaultMvcNamespace(Type controller)
        {
            var expectedTypeName = controller.Assembly.GetName().Name + ".Mvc.Controllers." + controller.Name;
            return string.Equals(expectedTypeName, controller.FullName, StringComparison.OrdinalIgnoreCase);
        }

        private static IList<Func<string, string>> GetControllerPathTransformations(Controller controller, string customPath)
        {
            var packagesManager = new PackageManager();
            var currentPackage = packagesManager.GetCurrentPackage();
            var pathTransformations = new List<Func<string, string>>();

            if (controller.RouteData != null && controller.RouteData.Values.ContainsKey("widgetName"))
            {
                var widgetName = (string)controller.RouteData.Values["widgetName"];
                var controllerType = FrontendManager.ControllerFactory.ResolveControllerType(widgetName);
                var widgetVp = FrontendControllerFactory.AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType));
                pathTransformations.Add(FrontendControllerFactory.GetPathTransformation(widgetVp, currentPackage, widgetName));
            }

            var controllerVp = customPath ?? FrontendControllerFactory.AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(controller.GetType().Assembly));
            pathTransformations.Add(FrontendControllerFactory.GetPathTransformation(controllerVp, currentPackage));

            return pathTransformations;
        }

        private static Func<string, string> GetPathTransformation(string controllerVirtualPath, string currentPackage, string widgetName = null)
        {
            return path =>
            {
                var result = path.Replace("~/", "~/" + controllerVirtualPath);

                if (!widgetName.IsNullOrEmpty())
                {
                    //{1} is the ControllerName argument in VirtualPathProviderViewEngines
                    result = result.Replace("{1}", widgetName);
                }

                if (!currentPackage.IsNullOrEmpty())
                    result = result + "#" + currentPackage + Path.GetExtension(path);

                return result;
            };
        }

        private static string AppendDefaultPath(string virtualPath)
        {
            return VirtualPathUtility.AppendTrailingSlash(virtualPath) + "Mvc/";
        }

        #endregion
    }
}
