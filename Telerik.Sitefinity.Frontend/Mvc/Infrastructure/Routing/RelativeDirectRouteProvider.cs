using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Direct route provider that extracts relative routes.
    /// </summary>
    internal sealed class RelativeDirectRouteProvider : DefaultDirectRouteProvider
    {
        /// <summary>Gets a set of route factories for the given action descriptor.</summary>
        /// <returns>A set of route factories.</returns>
        /// <param name="actionDescriptor">The action descriptor.</param>
        protected override IReadOnlyList<IDirectRouteFactory> GetActionRouteFactories(ActionDescriptor actionDescriptor)
        {
            var methodInfoActionDescriptor = actionDescriptor as IMethodInfoActionDescriptor;
            if (methodInfoActionDescriptor != null && methodInfoActionDescriptor.MethodInfo != null && 
                actionDescriptor.ControllerDescriptor != null && methodInfoActionDescriptor.MethodInfo.DeclaringType != actionDescriptor.ControllerDescriptor.ControllerType)
            {
                return null;
            }

            var customAttributes = actionDescriptor.GetCustomAttributes(false);
            var relativeRouteAttributes = customAttributes.OfType<RelativeRouteAttribute>().Select(r => r.DirectRouteFactory).ToList();

            return relativeRouteAttributes;
        }

        /// <summary>Gets route factories for the given controller descriptor.</summary>
        /// <returns>A set of route factories.</returns>
        /// <param name="controllerDescriptor">The controller descriptor.</param>
        protected override IReadOnlyList<IDirectRouteFactory> GetControllerRouteFactories(ControllerDescriptor controllerDescriptor)
        {
            if (controllerDescriptor == null)
                throw new ArgumentNullException("controllerDescriptor");

            var customAttributes = controllerDescriptor.GetCustomAttributes(false);
            var directRouteFactories = customAttributes.OfType<RelativeRouteAttribute>().Select(r => r.DirectRouteFactory).ToList();
            return directRouteFactories;
        }
    }
}
