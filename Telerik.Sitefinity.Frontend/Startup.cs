using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Contains the application startup event handles related to the Feather functionality of Sitefinity.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Called before the Asp.Net application is started.
        /// </summary>
        public static void OnPreApplicationStart()
        {
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        private static void Bootstrapper_Initialized(object sender, Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                EventHub.Subscribe<IWidgetValidatingEvent>(WidgetValidating);
            }
        }

        private static void WidgetValidating(IWidgetValidatingEvent evt)
        {
            var featherModule = SystemManager.GetModule("Feather");
            if (featherModule == null)
            {
                evt.IsValid = ControllerStore.Controllers().Any(c => c.ControllerType.FullName == evt.WidgetTypeFullName);
            }
        }
    }
}
