﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class extends the <see cref="SitefinityControllerFactory"/> by adding additional virtual paths for controller view engines.
    /// </summary>
    public class FrontendControllerFactory : SitefinityControllerFactory, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontendControllerFactory"/> class.
        /// </summary>
        public FrontendControllerFactory()
        {
            this.ninjectKernel = new StandardKernel();
        }

        #endregion

        #region Public members

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

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="cleanManagedResources">If <value>false</value> cleans up native resources, otherwise cleans up both managed and native resources.</param>
        protected virtual void Dispose(bool cleanManagedResources)
        {
            if (cleanManagedResources)
                this.ninjectKernel.Dispose();
        }

        #endregion

        #region Protected members

        /// <inheritdoc />
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return (IController)this.ninjectKernel.Get(controllerType);
        }

        #endregion

        #region Private members

        private static IList<Func<string, string>> GetControllerPathTransformations(Controller controller, string customPath)
        {
            var packagesManager = new PackageManager();
            var currentPackage = packagesManager.GetCurrentPackage();
            var pathTransformations = new List<Func<string, string>>();

            if (controller.RouteData != null && controller.RouteData.Values.ContainsKey("widgetName"))
            {
                var widgetName = (string)controller.RouteData.Values["widgetName"];
                var controllerType = FrontendManager.ControllerFactory.ResolveControllerType(widgetName);
                var widgetVp = AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(controllerType));
                pathTransformations.Add(FrontendControllerFactory.GetPathTransformation(widgetVp, currentPackage, widgetName));
            }

            var controllerVp = customPath ?? AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(controller.GetType().Assembly));
            pathTransformations.Add(FrontendControllerFactory.GetPathTransformation(controllerVp, currentPackage));

            var frontendVp = AppendDefaultPath(FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(FrontendControllerFactory).Assembly));
            if (!string.Equals(controllerVp, frontendVp, StringComparison.OrdinalIgnoreCase))
            {
                pathTransformations.Add(FrontendControllerFactory.GetPathTransformation(frontendVp, currentPackage));
            }

            return pathTransformations;
        }

        private static Func<string, string> GetPathTransformation(string controllerVirtualPath, string currentPackage, string widgetName = null)
        {
            return path =>
            {
                var result = path.Replace("~/", "~/" + controllerVirtualPath);

                if (!widgetName.IsNullOrEmpty())
                {
                    // {1} is the ControllerName argument in VirtualPathProviderViewEngines
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

        private void EnhanceViewEngines(Controller controller)
        {
            var enhanceAttr = this.GetEnhanceAttribute(controller.GetType());
            if (!enhanceAttr.Disabled)
            {
                controller.UpdateViewEnginesCollection(GetControllerPathTransformations(controller, enhanceAttr.VirtualPath));
            }
        }

        private EnhanceViewEnginesAttribute GetEnhanceAttribute(Type controllerType)
        {
            var enhanceAttr = controllerType.GetCustomAttributes(typeof(EnhanceViewEnginesAttribute), true).FirstOrDefault() as EnhanceViewEnginesAttribute;
            if (enhanceAttr != null)
            {
                return enhanceAttr;
            }

            enhanceAttr = new EnhanceViewEnginesAttribute
                              {
                                  Disabled = !this.IsInDefaultMvcNamespace(controllerType),
                                  VirtualPath =
                                      AppendDefaultPath(
                                          FrontendManager.VirtualPathBuilder.GetVirtualPath(
                                              controllerType.Assembly))
                              };
            return enhanceAttr;
        }

        private bool IsInDefaultMvcNamespace(Type controller)
        {
            var expectedTypeName = controller.Assembly.GetName().Name + ".Mvc.Controllers." + controller.Name;
            return string.Equals(expectedTypeName, controller.FullName, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Fields

        private IKernel ninjectKernel;

        #endregion
    }
}
