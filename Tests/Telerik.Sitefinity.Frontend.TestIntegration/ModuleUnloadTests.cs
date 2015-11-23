using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MbUnit.Framework;
using Newtonsoft.Json;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Modules.ControlTemplates;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Events;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration
{
    /// <summary>
    /// This class contains tests for unloading of the Feather module.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for unloading of the Feather module.")]
    public class ModuleUnloadTests
    {
        /// <summary>
        /// Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.
        /// </summary>
        /// <remarks>
        /// UnloadingFeather_Res_RegisterResource_ShouldBeUndone is not needed - Registering of resources is actually registering types in the Object factory
        /// UnloadingFeather_RouteTable_Routes_MapRoute_ShouldBeUndone is not needed - Routes are in one collection checked by UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone
        /// </remarks>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.")]
        public void DeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation()
        {
            //// var featherWasEnabled = this.IsFeatherEnabled();

            //// try
            //// {
                //// if (!featherWasEnabled)
                    //// this.ActivateFeather();

                //// this.DeactivateFeather();

                this.UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone();
                this.UnloadingFeather_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone();
                this.UnloadingFeather_ObjectFactory_Container_RegisterType_ShouldBeUndone();
                this.UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone();
                this.UnloadingFeather_EventHub_Subscribe_ShouldBeUndone();
                this.UnloadingFeather_IFileMonitor_Start_ShouldBeUndone();
                this.UnloadingFeather_GlobalFilters_Filters_Add_ShouldBeUndone();
                this.UnloadingFeather_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone();
                this.UnloadingFeather_ViewEngines_Engines_Remove_ShouldBeUndone();
                this.UnloadingFeather_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone();
                this.UnloadingFeather_ControllerStore_AddController_ShouldBeUndone();
                this.UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone();
            //// }
            //// finally
            //// {
                //// if (featherWasEnabled)
                    //// this.ActivateFeather();
            //// }
        }

        /// <summary>
        /// Checks whether after unloading Feather the VirtualPathManager AddVirtualFileResolver calls are undone.
        /// </summary>
        public void UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone()
        {
            var wildcardPaths = typeof(VirtualPathManager).GetField("wildcardPaths", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IList<PathDefinition>;

            // There are no changes on virtualPaths
            Assert.IsFalse(wildcardPaths.Any(wp => wp.ResolverName == "MvcFormsResolver"));
            Assert.IsFalse(wildcardPaths.Any(wp => wp.ResolverName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the SystemManager RegisterServiceStackPlugin calls are undone.
        /// </summary>
        public void UnloadingFeather_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone()
        {
            var pendingServiceStackPlugins = typeof(SystemManager).GetField("PendingServiceStackPlugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as IEnumerable<object>;
            Assert.IsFalse(pendingServiceStackPlugins.Any(pssp => pssp.GetType().FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the ObjectFactory Container RegisterType calls are undone.
        /// </summary>
        public void UnloadingFeather_ObjectFactory_Container_RegisterType_ShouldBeUndone()
        {
            Assert.IsFalse(ObjectFactory.Container.Registrations.Any(r =>
                (r.RegisteredType != null && !string.IsNullOrEmpty(r.RegisteredType.FullName) && r.RegisteredType.FullName.Contains(ModuleUnloadTests.FrontendAssemblyPrefix)) || 
                (r.MappedToType != null && !string.IsNullOrEmpty(r.MappedToType.FullName) && r.MappedToType.FullName.Contains(ModuleUnloadTests.FrontendAssemblyPrefix))));
        }

        /// <summary>
        /// Checks whether after unloading Feather the SystemManager RegisterRoute calls are undone.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "testo"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "routes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "routeRegistrations")]
        public void UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone()
        {
            // Routes are checked in RouteTable.Routes in UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone
            var routeRegistrationModuleName = Type.GetType("Telerik.Sitefinity.Abstractions.RouteRegistration, Telerik.Sitefinity").GetProperty("ModuleName", BindingFlags.NonPublic | BindingFlags.Instance);
            var routeRegistrations = Type.GetType("Telerik.Sitefinity.Services.RouteManager, Telerik.Sitefinity").GetField("routeRegistrations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as IEnumerable<object>;
            Assert.IsFalse(routeRegistrations.Any(r => (routeRegistrationModuleName.GetValue(r) as string).StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the EventHub Subscribe calls are undone.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "testo")]
        public void UnloadingFeather_EventHub_Subscribe_ShouldBeUndone()
        {
            var handlerLists = typeof(EventService).GetField("handlerLists", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ObjectFactory.Resolve<IEventService>());
            var allHandlersValues = handlerLists.GetType().GetProperty("Values").GetValue(handlerLists) as IEnumerable<object>;
            var innerHandlersProperty = typeof(EventService).GetNestedType("HandlerList", BindingFlags.Instance | BindingFlags.NonPublic).GetField("handlers", BindingFlags.Instance | BindingFlags.NonPublic);
            var allHandlers = allHandlersValues.SelectMany(h => innerHandlersProperty.GetValue(h) as IList<Delegate>);
            Assert.IsFalse(allHandlers.Any(h => h.Method.ReflectedType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the IFileMonitor Start calls are undone.
        /// </summary>
        public void UnloadingFeather_IFileMonitor_Start_ShouldBeUndone()
        {
            var moduleInstance = SystemManager.GetModule("Feather");
            if (moduleInstance != null)
            {
                var initializers = typeof(FrontendModule).GetField("initializers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(moduleInstance) as Lazy<IEnumerable<IInitializer>>;
                var fileInitializer = initializers.Value.FirstOrDefault(i => i is FileMonitoringInitializer);
                var fileMonitorInstance = typeof(FileMonitoringInitializer).GetField("fileMonitor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fileInitializer);
                Assert.IsNull(fileMonitorInstance);
            }
        }

        /// <summary>
        /// Checks whether after unloading Feather the GlobalFilters Filters Add calls are undone.
        /// </summary>
        public void UnloadingFeather_GlobalFilters_Filters_Add_ShouldBeUndone()
        {
            Assert.IsFalse(GlobalFilters.Filters.Any(f => f.Instance is CacheDependentAttribute));
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControlTemplates RegisterTemplatableControl calls are undone.
        /// </summary>
        public void UnloadingFeather_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone()
        {
            var controlTemplates = typeof(ControlTemplates).GetField("controlTemplates", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, IControlTemplateInfo>;
            Assert.IsFalse(controlTemplates.Values.Any(cti => cti.ControlType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the ViewEngines Engines Remove calls are undone.
        /// </summary>
        public void UnloadingFeather_ViewEngines_Engines_Remove_ShouldBeUndone()
        {
            Assert.IsTrue(ViewEngines.Engines.Any(ve => ve is SitefinityViewEngine));
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControllerBuilder Current SetControllerFactory calls are undone.
        /// </summary>
        public void UnloadingFeather_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone()
        {
            Assert.IsFalse(ControllerBuilder.Current.GetControllerFactory() is FrontendControllerFactory);
        }

        /// <summary>
        /// Checks whether after unloading Feather the ControllerStore AddController calls are undone.
        /// </summary>
        public void UnloadingFeather_ControllerStore_AddController_ShouldBeUndone()
        {
            Assert.IsFalse(ControllerStore.Controllers().Any(c => c.ControllerType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading Feather the RouteTable Routes Insert calls are undone.
        /// </summary>
        public void UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone()
        {
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url == "Telerik.Sitefinity.Frontend/{controller}/Master/{widgetName}"));
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url == "Telerik.Sitefinity.Frontend/{controller}/View/{widgetName}/{viewType}"));
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url.Contains(ModuleUnloadTests.FrontendAssemblyPrefix)));
            Assert.IsFalse(RouteTable.Routes.Any(r => r.GetType().FullName == "System.Web.Mvc.Routing.RouteCollectionRoute" && ((IEnumerable<RouteBase>)r).Any(rb => rb is Route && ((Route)rb).Url == "rest-api/login-status")));
            Assert.IsFalse(RouteTable.Routes.Any(r => r.GetType().FullName == "System.Web.Mvc.Routing.LinkGenerationRoute" && ((Route)r).Url == "rest-api/login-status"));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private string SerializeObject(object obj)
        {
            var jsonSerializerSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Serialize, PreserveReferencesHandling = PreserveReferencesHandling.Objects };
            return JsonConvert.SerializeObject(obj, Formatting.Indented, jsonSerializerSettings);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private Dictionary<string, object> GetState()
        {
            var state = new Dictionary<string, object>();

            var wildcardPaths = typeof(VirtualPathManager).GetField("wildcardPaths", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var virtualPaths = typeof(VirtualPathManager).GetField("virtualPaths", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            state.Add("UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone_wildcardPaths", wildcardPaths);
            state.Add("UnloadingFeather_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone_virtualPaths", virtualPaths);

            var pendingServiceStackPlugins = typeof(SystemManager).GetField("PendingServiceStackPlugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            state.Add("UnloadingFeather_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone_pendingServiceStackPlugins", pendingServiceStackPlugins);

            state.Add("UnloadingFeather_ObjectFactory_Container_RegisterType_ShouldBeUndone_registrations", ObjectFactory.Container.Registrations);

            var routes = Type.GetType("Telerik.Sitefinity.Services.RouteManager, Telerik.Sitefinity").GetField("routes", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as RouteCollection;
            var routeRegistrations = Type.GetType("Telerik.Sitefinity.Services.RouteManager, Telerik.Sitefinity").GetField("routeRegistrations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as List<object>;
            state.Add("UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone_routes", routes);
            state.Add("UnloadingFeather_SystemManager_RegisterRoute_ShouldBeUndone_routeRegistrations", routeRegistrations);

            var handlerLists = typeof(EventService).GetField("handlerLists", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ObjectFactory.Resolve<IEventService>());
            state.Add("UnloadingFeather_EventHub_Subscribe_ShouldBeUndone_handlerLists", handlerLists);

            var filters = Type.GetType("System.Web.Mvc.GlobalFilters, System.Web.Mvc").GetProperty("Filters", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
            state.Add("UnloadingFeather_GlobalFilters_Filters_Add_ShouldBeUndone_filters", filters);

            var controlTemplates = typeof(ControlTemplates).GetField("controlTemplates", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, IControlTemplateInfo>;
            state.Add("UnloadingFeather_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone_controlTemplates", controlTemplates);

            var viewEngines = Type.GetType("System.Web.Mvc.ViewEngines, System.Web.Mvc").GetProperty("Engines", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
            state.Add("UnloadingFeather_ViewEngines_Engines_Remove_ShouldBeUndone_viewEngines", viewEngines);

            var currentControllerBuilder = Type.GetType("System.Web.Mvc.ControllerBuilder, System.Web.Mvc").GetProperty("Current", BindingFlags.Static | BindingFlags.Public).GetValue(null, null);
            var controllerFactory = Type.GetType("System.Web.Mvc.ControllerBuilder, System.Web.Mvc").GetMethod("GetControllerFactory", BindingFlags.Public | BindingFlags.Instance).Invoke(currentControllerBuilder, null);
            state.Add("UnloadingFeather_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone_controllerFactory", controllerFactory);

            var controllers = Type.GetType("Telerik.Sitefinity.Mvc.Store.ControllerStore, Telerik.Sitefinity.Mvc").GetMethod("Controllers", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
            state.Add("UnloadingFeather_ControllerStore_AddController_ShouldBeUndone_controllers", controllers);

            var globalRoutes = RouteTable.Routes;
            state.Add("UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone_routes", globalRoutes);

            return state;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private bool IsFeatherEnabled()
        {
            return SystemManager.GetModule("Feather") != null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ActivateFeather()
        {
            if (this.IsFeatherEnabled())
                return;

            var installOperationEndpoint = UrlPath.ResolveUrl("/Sitefinity/Services/ModulesService/modules?operation=2", true);
            this.MakePutRequest(installOperationEndpoint, JsonRequestPayload);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void DeactivateFeather()
        {
            if (!this.IsFeatherEnabled())
                return;

            var uninstallOperationEndpoint = UrlPath.ResolveUrl("/Sitefinity/Services/ModulesService/modules?operation=3", true);
            this.MakePutRequest(uninstallOperationEndpoint, JsonRequestPayload);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void MakePutRequest(string url, string payload)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.CookieContainer = new CookieContainer();
            httpWebRequest.Headers["Cookie"] = HttpContext.Current.Request.Headers["Cookie"];
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "PUT";

            using (var writer = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                writer.Write(payload);
            }

            httpWebRequest.GetResponse();
        }

        private const string FrontendAssemblyPrefix = "Telerik.Sitefinity.Frontend";
        private const string JsonRequestPayload = "{\"ClientId\":\"Feather\",\"Description\":\"Modern, intuitive, convention based, mobile-first UI for Telerik Sitefinity\",\"ErrorMessage\":\"\",\"IsModuleLicensed\":true,\"IsSystemModule\":false,\"Key\":\"Feather\",\"ModuleId\":\"00000000-0000-0000-0000-000000000000\",\"ModuleType\":0,\"Name\":\"Feather\",\"ProviderName\":\"\",\"StartupType\":3,\"Status\":1,\"Title\":\"Feather\",\"Type\":\"Telerik.Sitefinity.Frontend.FrontendModule, Telerik.Sitefinity.Frontend\",\"Version\":{\"_Build\":390,\"_Major\":1,\"_Minor\":4,\"_Revision\":0}}";
    }
}
