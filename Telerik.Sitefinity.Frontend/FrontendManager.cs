﻿using System;
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
                if (FrontendManager.virtualPathBuilder == null)
                    FrontendManager.virtualPathBuilder = new VirtualPathBuilder();

                return FrontendManager.virtualPathBuilder;
            }
        }

        /// <summary>
        /// Gets an instance of the controller registry.
        /// </summary>
        public static ISitefinityControllerFactory ControllerFactory
        {
            get
            {
                return ControllerBuilder.Current.GetControllerFactory() as ISitefinityControllerFactory;;
            }
        }

        /// <summary>
        /// Gets or sets an instance of authentication evaluator
        /// </summary>
        public static AuthenticationEvaluator AuthenticationEvaluator
        {
            get
            {
                if (FrontendManager.authenticationEvaluator == null)
                    FrontendManager.authenticationEvaluator = new AuthenticationEvaluator();

                return FrontendManager.authenticationEvaluator;
            }
            set
            {
                FrontendManager.authenticationEvaluator = value;
            }
        }

        private static VirtualPathBuilder virtualPathBuilder;
        private static AuthenticationEvaluator authenticationEvaluator;
    }
}
