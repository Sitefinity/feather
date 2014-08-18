using System;
using System.Collections.Generic;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Mvc.ISitefinityControllerFactory"/> used for test purpsoes only.
    /// </summary>
    public class DummyControllerFactory : ISitefinityControllerFactory
    {
        public DummyControllerFactory()
        {
            this.ControllerRegistry = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// The controller registry. Map ControllerName to ControllerType using this dictionary to emulate controller registration.
        /// </summary>
        public IDictionary<string, Type> ControllerRegistry { get; set; }

        /// <inheritdoc />
        public void RegisterController(string controllerName, Type controllerType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string ResolveControllerName(Sitefinity.Mvc.Proxy.MvcProxyBase proxy)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string ResolveControllerName(Type proxyType)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Type ResolveControllerType(string controllerName)
        {
            if (this.ControllerRegistry.ContainsKey(controllerName))
                return this.ControllerRegistry[controllerName];
            else
                return null;
        }

        /// <inheritdoc />
        public void UnregisterController(string controllerName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public System.Web.Mvc.IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public System.Web.SessionState.SessionStateBehavior GetControllerSessionBehavior(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void ReleaseController(System.Web.Mvc.IController controller)
        {
            throw new NotImplementedException();
        }
    }
}
