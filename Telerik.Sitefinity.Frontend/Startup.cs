using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
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
            Bootstrapper.Initialized -= Bootstrapper_Initialized;
            Bootstrapper.Initialized += Bootstrapper_Initialized;
        }

        private static void Bootstrapper_Initialized(object sender, Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                ObjectFactory.Container.RegisterType<IToolboxFilter, FeatherEnabledToolboxFilter>(typeof(FeatherEnabledToolboxFilter).FullName);

                AddWidgetValidating();
            }
        }

        private static void AddWidgetValidating()
        {
            var method = typeof(Startup).GetMethod("WidgetValidating", BindingFlags.NonPublic | BindingFlags.Static);

            var type = typeof(Func<string, Tuple<bool, string>>);
            Delegate del = Delegate.CreateDelegate(type, method);

            var processWidgetValidationInfo = typeof(ToolboxesConfig).GetField("ValidateWidgetState", BindingFlags.NonPublic | BindingFlags.Static);

            if (processWidgetValidationInfo != null && processWidgetValidationInfo.GetValue(del) == null)
            {
                processWidgetValidationInfo.SetValue(null, del);
            }
        }

        private static Tuple<bool, string> WidgetValidating(string widgetType)
        {
            Tuple<bool, string> widgetState = null;
            var featherModule = SystemManager.GetModule("Feather");
            var isFeatherWidget = widgetType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal);

            if (isFeatherWidget && featherModule == null)
            {
                var isActive = ControllerStore.Controllers().Any(c => c.ControllerType != null && c.ControllerType.FullName == widgetType);

                if (!isActive)
                {
                    widgetState = new Tuple<bool, string>(false, "Feather");
                }
            }

            return widgetState;
        }
    }
}
