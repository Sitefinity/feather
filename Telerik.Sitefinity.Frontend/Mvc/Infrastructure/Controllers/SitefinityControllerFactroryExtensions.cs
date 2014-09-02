using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    public static class SitefinityControllerFactoryExtensions
    {
        /// <summary>
        /// Determines whether the given type represents an MVC controller.
        /// </summary>
        /// <param name="type">The type.</param>
        public static bool IsController(this ISitefinityControllerFactory factory, Type type)
        {
            if (type == null
                || !type.IsPublic
                || !type.Name.EndsWith(SitefinityControllerFactoryExtensions.ControllerSuffix, StringComparison.Ordinal)
                || type.IsAbstract)
            {
                return false;
            }

            return typeof(IController).IsAssignableFrom(type);
        }

        /// <summary>
        /// Gets the name of the controller. It will remove "Controller" suffix if present.
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns>
        /// The name of the controller. If present, "Controller" suffix will be removed.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">controllerType</exception>
        public static string GetControllerName(this ISitefinityControllerFactory factory, Type controllerType)
        {
            if (controllerType == null)
                throw new ArgumentNullException("controllerType");

            var controllerName = controllerType.Name;

            if (controllerName.EndsWith(SitefinityControllerFactoryExtensions.ControllerSuffix, StringComparison.Ordinal))
                controllerName = controllerName.Substring(0, controllerName.Length - SitefinityControllerFactoryExtensions.ControllerSuffix.Length);

            return controllerName;
        }

        /// <summary>
        /// Constant containing the suffix in the type name that is expected in Controller types.
        /// </summary>
        private const string ControllerSuffix = "Controller";
    }
}
