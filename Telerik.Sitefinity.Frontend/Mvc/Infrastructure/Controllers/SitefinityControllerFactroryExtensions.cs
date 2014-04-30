using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Mvc;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Mvc.Store;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    public static class SitefinityControllerFactroryExtensions
    {
        /// <summary>
        /// Determines whether the given type represents an MVC controller.
        /// </summary>
        /// <param name="type">The type.</param>
        public static bool IsController(this ISitefinityControllerFactory factory, Type type)
        {
            if (type == null
                || !type.IsPublic
                || !type.Name.EndsWith(SitefinityControllerFactroryExtensions.controllerSuffix, StringComparison.OrdinalIgnoreCase)
                || type.IsAbstract)
            {
                return false;
            }
            else
            {
                return typeof(IController).IsAssignableFrom(type);
            }
        }

        /// <summary>
        /// Gets the name of the controller. It will remove "Controller" suffix if present.
        /// </summary>
        /// <param name="controllerType">Type of the controller.</param>
        /// <returns>
        /// The name of the controller. If present, "Controller" suffix will be removed.
        /// </returns>
        public static string GetControllerName(this ISitefinityControllerFactory factory, Type controllerType)
        {
            if (controllerType == null)
                throw new ArgumentNullException("controllerType");

            var controllerName = controllerType.Name;

            if (controllerName.EndsWith(SitefinityControllerFactroryExtensions.controllerSuffix, StringComparison.OrdinalIgnoreCase))
                controllerName = controllerName.Substring(0, controllerName.Length - SitefinityControllerFactroryExtensions.controllerSuffix.Length);

            return controllerName;
        }
        
        /// <summary>
        /// Constant containing the suffix in the type name that is expected in Controller types.
        /// </summary>
        private const string controllerSuffix = "Controller";
    }
}
