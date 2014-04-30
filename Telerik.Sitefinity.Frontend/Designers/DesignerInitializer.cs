using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Designers
{
    /// <summary>
    /// This class contains logic for initializing the MVC designer.
    /// </summary>
    public class DesignerInitializer
    {
        /// <summary>
        /// Initializes the MVC designer.
        /// </summary>
        public void Initialize()
        {
            RouteTable.Routes.MapRoute("MvcDesigner","Telerik.Sitefinity.Frontend/{controller}/{action}/{widgetName}", new { controller = "DesignerController", action = "Index" });

            ObjectFactory.Container.RegisterType<IDesignerResolver, DesignerResolver>(new ContainerControlledLifetimeManager());

            EventHub.Unsubscribe<IScriptsRegisteringEvent>(this.RegisteringScriptsHandler);
            EventHub.Subscribe<IScriptsRegisteringEvent>(this.RegisteringScriptsHandler);
        }

        /// <summary>
        /// Registering the scripts for ZoneEditor.
        /// </summary>
        /// <param name="event">The event.</param>
        private void RegisteringScriptsHandler(IScriptsRegisteringEvent @event)
        {
            var packagesManager = new PackagesManager();
            if (@event.Sender.GetType() == typeof(ZoneEditor))
            {
                var scriptRootPath = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(this.GetType().Assembly);

                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Angular/angular.min.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Angular/angular-route.min.js"));

                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Bootstrap/js/ui-bootstrap-tpls-0.10.0.min.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/ModalDialogModule.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/ControlPropertyServices.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Kendo/angular-kendo.js"));

                var currentPackage = packagesManager.GetCurrentPackage();
                if (!currentPackage.IsNullOrEmpty())
                {
                    var packageVar = "var sf_package = '{0}';".Arrange(currentPackage);
                    ((ZoneEditor)@event.Sender).Page.ClientScript.RegisterStartupScript(@event.Sender.GetType(), "sf_package",
                        packageVar + @"Sys.Net.WebRequestManager.add_invokingRequest(function (executor, args) { 
                            var url = args.get_webRequest().get_url();
                            if (url.indexOf('?') == -1) 
                                url += '?package=' + encodeURIComponent(sf_package); 
                            else 
                                url += '&package=' + encodeURIComponent(sf_package); 
                            args.get_webRequest().set_url(url); 
                        });",
                        addScriptTags: true);
                }
            }
        }
    }
}
