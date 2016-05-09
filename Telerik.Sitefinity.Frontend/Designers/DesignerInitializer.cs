using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Designers
{
    /// <summary>
    /// This class contains logic for initializing the MVC designer.
    /// </summary>
    internal class DesignerInitializer : IInitializer
    {
        /// <summary>
        /// Initializes the MVC designer.
        /// </summary>
        public void Initialize()
        {
            if (RouteTable.Routes[DesignerInitializer.MvcDesignerRouteName] == null)
            {
                RouteTable.Routes.MapRoute(DesignerInitializer.MvcDesignerRouteName, "Telerik.Sitefinity.Frontend/{controller}/Master/{widgetName}", new { controller = "DesignerController", action = "Master" });
            }

            if (RouteTable.Routes[DesignerInitializer.MvcDesignerViewRouteName] == null)
            {
                RouteTable.Routes.MapRoute(DesignerInitializer.MvcDesignerViewRouteName, "Telerik.Sitefinity.Frontend/{controller}/View/{widgetName}/{viewType}", new { controller = "DesignerController", action = "View", viewType = "PropertyGrid" });
            }

            ObjectFactory.Container.RegisterType<IDesignerResolver, DesignerResolver>(new ContainerControlledLifetimeManager());

            EventHub.Unsubscribe<IScriptsRegisteringEvent>(this.RegisteringScriptsHandler);
            EventHub.Subscribe<IScriptsRegisteringEvent>(this.RegisteringScriptsHandler);
        }

        /// <summary>
        /// Uninitializes the MVC designer.
        /// </summary>
        public void Uninitialize()
        {
            RouteTable.Routes.Remove(RouteTable.Routes[DesignerInitializer.MvcDesignerRouteName]);
            RouteTable.Routes.Remove(RouteTable.Routes[DesignerInitializer.MvcDesignerViewRouteName]);
        }

        /// <summary>
        /// Registering the scripts for ZoneEditor.
        /// </summary>
        /// <param name="event">The event.</param>
        private void RegisteringScriptsHandler(IScriptsRegisteringEvent @event)
        {
            if (@event.Sender.GetType() == typeof(ZoneEditor))
            {
                var scriptRootPath = "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(this.GetType().Assembly);
                
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Angular/angular.min.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Angular/angular-route.min.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/Bootstrap/js/ui-bootstrap-tpls.min.js"));
           
                ////var references = PageManager.GetScriptReferences(ScriptRef.KendoAll);
                ////foreach (var scriptRef in references)
                ////{
                ////    @event.Scripts.Add(scriptRef);
                ////}
     
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Designers/Scripts/page-editor-services.js"));
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Designers/Scripts/page-editor.js"));
                
                @event.Scripts.Add(new ScriptReference(scriptRootPath + "Mvc/Scripts/LABjs/LAB.min.js"));

                var currentPackage = new PackageManager().GetCurrentPackage();
                if (!currentPackage.IsNullOrEmpty())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(@"Sys.Net.WebRequestManager.add_invokingRequest(function (executor, args) {");
                    sb.AppendLine("var url = args.get_webRequest().get_url();");
                    sb.AppendLine("if (url.indexOf('?') == -1)");
                    sb.AppendLine(" url += '?package=' + encodeURIComponent(sf_package);");
                    sb.AppendLine("else");
                    sb.AppendLine(" url += '&package=' + encodeURIComponent(sf_package); ");    
                    sb.AppendLine("args.get_webRequest().set_url(url); ");    
                    sb.AppendLine("});");    

                    var packageVar = "var sf_package = '{0}';".Arrange(currentPackage);
                    ((ZoneEditor)@event.Sender).Page.ClientScript.RegisterStartupScript(
                        @event.Sender.GetType(), 
                        "sf_package",
                        packageVar + sb,
                        addScriptTags: true);
                }
            }
        }

        private const string MvcDesignerRouteName = "MvcDesigner";
        private const string MvcDesignerViewRouteName = "MvcDesignerView";
    }
}
