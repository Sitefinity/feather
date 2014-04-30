using System;
using System.Collections.Generic;
using System.Web.UI;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Packages;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Security;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Service
{
    /// <summary>
    /// A service that will be invoked by Sitefinity.
    /// </summary>
    public class SitefinityService : ServiceBase, ISitefinityService
    {
        /// <summary>
        /// Initializes the service with specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            this.RegisterTypes();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
            Bootstrapper.Initialized += this.Bootstrapper_Initialized;
        }

        /// <summary>
        /// Handles the Initialized event of the Bootstrapper.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sitefinity.Data.ExecutedEventArgs"/> instance containing the event data.</param>
        protected virtual void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                this.RegisterFileObservers();

                var controllerInitializer = new ControllerInitializer();
                controllerInitializer.Initialize();

                var layoutsInitializer = new LayoutInitializer();
                layoutsInitializer.Initialize();

                var gridSystemInitializer = new GridSystemInitializer();
                gridSystemInitializer.Initialize();

                var designerInitializer = new DesignerInitializer();
                designerInitializer.Initialize();

                this.RegisterScripts();
            }
        }

        /// <summary>
        /// Registers the file observers.
        /// </summary>
        private void RegisterFileObservers()
        {
            var fileObserver = ObjectFactory.Resolve<IFilesMonitor>();

            var direcotoriesInfo = new Dictionary<string, bool>();
            direcotoriesInfo.Add("~/" + PackagesManager.PackagesFolder, true);
            direcotoriesInfo.Add("~/Mvc/Views/Layouts", false);

            fileObserver.Start(direcotoriesInfo);
        }

        /// <summary>
        /// Registers the scripts.
        /// </summary>
        private void RegisterScripts()
        {
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
                var scriptRootPath = "~/" + MvcIntegrationManager.GetVirtualPathBuilder().GetVirtualPath(this.GetType().Assembly);

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

        /// <summary>
        /// Gets the types of the service used to register and resolve the service from the Service Bus.
        /// </summary>
        /// <value>The type of the service.</value>
        public override Type[] Interfaces
        {
            get 
            { 
                return this.interfaces;
            }
        }

        /// <summary>
        /// Registers the types in the ObjectFactory.
        /// </summary>
        protected virtual void RegisterTypes()
        {
            ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());

            ObjectFactory.Container.RegisterType<IFilesMonitor, FileMonitor>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IFileManager, LayoutFilesManager>(ResourceTypes.Layouts.ToString(), new ContainerControlledLifetimeManager());

            ObjectFactory.Container.RegisterType<ISitefinityControllerFactory, MultiplePathsControllerFactory>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IAuthenticationEvaluator, AuthenticationEvaluator>(new ContainerControlledLifetimeManager());

            //Raise event to allow injection.
        }

        
        private readonly Type[] interfaces = new Type[] { typeof(ISitefinityService) };
    }
}
