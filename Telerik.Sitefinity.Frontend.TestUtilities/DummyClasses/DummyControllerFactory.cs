using System;
using System.Collections.Generic;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// A dummy controller factory for unit tests.
    /// </summary>
    public class DummyControllerFactory : ISitefinityControllerFactory
    {
        /// <summary>
        /// The controller registry. Map ControllerName to ControllerType using this dictionary to emulate controller registration.
        /// </summary>
        public IDictionary<string, Type> ControllerRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        public void RegisterController(string controllerName, Type controllerType)
        {
            throw new NotImplementedException();
        }

        public string ResolveControllerName(Sitefinity.Mvc.Proxy.MvcProxyBase proxy)
        {
            throw new NotImplementedException();
        }

        public string ResolveControllerName(Type proxyType)
        {
            throw new NotImplementedException();
        }

        public Type ResolveControllerType(string controllerName)
        {
            if (this.ControllerRegistry.ContainsKey(controllerName))
                return this.ControllerRegistry[controllerName];
            else
                return null;
        }

        public void UnregisterController(string controllerName)
        {
            throw new NotImplementedException();
        }

        public System.Web.Mvc.IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            throw new NotImplementedException();
        }

        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            throw new NotImplementedException();
        }

        public void ReleaseController(System.Web.Mvc.IController controller)
        {
            throw new NotImplementedException();
        }
    }
}
