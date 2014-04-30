using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Security;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// This class manages instances of classes that are that are used to integrate the web application with the MVC framework.
    /// </summary>
    public static class FrontendManager
    {
        /// <summary>
        /// Gets an instance of the virtual path builder.
        /// </summary>
        public static VirtualPathBuilder VirtualPathBuilder
        {
            get
            {
                if (virtualPathBuilder == null)
                    virtualPathBuilder = new VirtualPathBuilder();

                return virtualPathBuilder;
            }
        }

        /// <summary>
        /// Gets an instance of the controller registry.
        /// </summary>
        public static ISitefinityControllerFactory ControllerFactory
        {
            get
            {
                if (controllerRegistry == null)
                    controllerRegistry = ControllerBuilder.Current.GetControllerFactory() as ISitefinityControllerFactory;

                return controllerRegistry;
            }
        }

        /// <summary>
        /// Gets or sets an instance of authentication evaluator
        /// </summary>
        public static AuthenticationEvaluator AuthenticationEvaluator
        {
            get
            {
                if (authenticationEvaluator == null)
                    authenticationEvaluator = new AuthenticationEvaluator();

                return authenticationEvaluator;
            }
            set
            {
                authenticationEvaluator = value;
            }
        }

        private static VirtualPathBuilder virtualPathBuilder;
        private static ISitefinityControllerFactory controllerRegistry;
        private static AuthenticationEvaluator authenticationEvaluator;
    }
}
