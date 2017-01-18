using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Pages.Model;
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

            if (!GlobalFilters.Filters.Any(f => f.Instance.GetType() == typeof(FrontendModuleFilter)))
                GlobalFilters.Filters.Add(new FrontendModuleFilter());
        }

        private static void Bootstrapper_Initialized(object sender, Data.ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                ObjectFactory.Container.RegisterType<IToolboxFilter, FeatherEnabledToolboxFilter>(typeof(FeatherEnabledToolboxFilter).FullName);
                ObjectFactory.Container.RegisterType<IToolboxFilter, GridControlToolboxFilter>(typeof(GridControlToolboxFilter).FullName, new InjectionConstructor(new Func<PageTemplateFramework>(Startup.ExtractFramework)));

                Startup.AddWidgetValidating();

                Startup.RegisterStringResources();
            }
        }

        private static void RegisterStringResources()
        {
            if (SystemManager.GetModule("Feather") == null)
                new ControllerContainerInitializer().RegisterStringResources();
        }

        private static void AddWidgetValidating()
        {
            var method = typeof(Startup).GetMethod("WidgetValidating", BindingFlags.NonPublic | BindingFlags.Static);
            var type = typeof(Func<string, Tuple<bool, string>>);
            Delegate del = Delegate.CreateDelegate(type, method);

            var processWidgetValidationInfo = typeof(ToolboxesConfig).GetField("ValidateWidgetState", BindingFlags.NonPublic | BindingFlags.Static);
            if (processWidgetValidationInfo != null && processWidgetValidationInfo.GetValue(del) == null)
                processWidgetValidationInfo.SetValue(null, del);
        }

        private static Tuple<bool, string> WidgetValidating(string widgetType)
        {
            Tuple<bool, string> widgetState = null;

            var isFeatherWidget = widgetType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal);
            if (isFeatherWidget)
            {
                var featherModule = SystemManager.GetModule("Feather");
                if (featherModule == null)
                {
                    var isActive = ControllerStore.Controllers().Any(c => c.ControllerType != null && c.ControllerType.FullName == widgetType);
                    if (!isActive)
                        widgetState = new Tuple<bool, string>(false, "Feather");
                }
            }

            return widgetState;
        }
        
        private static PageTemplateFramework ExtractFramework()
        {
            var contextItems = SystemManager.CurrentHttpContext.Items;
            var framework = (PageTemplateFramework)contextItems["PageTemplateFramework"];
            return framework;
        }
    }
}
