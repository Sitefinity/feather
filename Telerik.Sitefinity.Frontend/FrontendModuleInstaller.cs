using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Modules.Comments;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Services.FilesService;
using Telerik.Sitefinity.Frontend.Services.ListsService;
using Telerik.Sitefinity.Frontend.Services.ReviewsService;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments.Notifications;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Handles the logic for Feather install/initialize
    /// </summary>
    internal static class FrontendModuleInstaller
    {
        /// <summary>
        /// Installs the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public static void Install(SiteInitializer initializer)
        {
            if (FrontendModuleInstaller.FrontendServiceExists())
                FrontendModuleInstaller.InitialUpgrade(initializer);
        }

        /// <summary>
        /// Initializes the specified settings.
        /// </summary>
        /// <param name="ninjectDependencyResolver">The ninject dependency resolver.</param>
        public static void Initialize(IKernel ninjectDependencyResolver)
        {
            SystemManager.RegisterServiceStackPlugin(new ListsServiceStackPlugin());
            SystemManager.RegisterServiceStackPlugin(new FilesServiceStackPlugin());
            SystemManager.RegisterServiceStackPlugin(new ReviewsServiceStackPlugin());

            var controllerContainerInitializer = new ControllerContainerInitializer();
            ninjectDependencyResolver.Load(controllerContainerInitializer.ControllerContainerAssemblies);
            controllerContainerInitializer.RegisterStringResources();
        }

        /// <summary>
        /// Bootstrapper_s the initialized.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        public static void Bootstrapper_Initialized(IEnumerable<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
                initializer.Initialize();

            ObjectFactory.Container.RegisterType<ICommentNotificationsStrategy, ReviewNotificationStrategy>(new ContainerControlledLifetimeManager());
        }
        
        #region Install

        private static bool FrontendServiceExists()
        {
            var systemConfig = Config.Get<SystemConfig>();
            return systemConfig.SystemServices.ContainsKey(FrontendModuleInstaller.FrontendServiceName);
        }

        private static void InitialUpgrade(SiteInitializer initializer)
        {
            FrontendModuleInstaller.RemoveFrontendService();
            FrontendModuleInstaller.RenameControllers(initializer);
            FrontendModuleInstaller.ClearToolboxItems();
        }

        private static void RemoveFrontendService()
        {
            var configManager = ConfigManager.GetManager();
            var systemConfig = configManager.GetSection<SystemConfig>();
            systemConfig.SystemServices.Remove(FrontendModuleInstaller.FrontendServiceName);
            configManager.SaveSection(systemConfig);
        }

        private static void RenameControllers(SiteInitializer initializer)
        {
            var manager = initializer.PageManager;
            var controllerNameProperties = manager.GetControls<ControlData>()
                .Where(c => c.ObjectType == "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy" || c.ObjectType == "Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.MvcWidgetProxy, Telerik.Sitefinity.Frontend")
                .SelectMany(c => c.Properties.Where(p => p.Name == "ControllerName"));

            foreach (var property in controllerNameProperties)
            {
                if (property.Value == "News.Mvc.Controllers.NewsController")
                    property.Value = "Telerik.Sitefinity.Frontend.News.Mvc.Controllers.NewsController";
                else if (property.Value == "ContentBlock.Mvc.Controllers.ContentBlockController")
                    property.Value = "Telerik.Sitefinity.Frontend.ContentBlock.Mvc.Controllers.ContentBlockController";
                else if (property.Value == "Navigation.Mvc.Controllers.NavigationController")
                    property.Value = "Telerik.Sitefinity.Frontend.Navigation.Mvc.Controllers.NavigationController";
                else if (property.Value == "SocialShare.Mvc.Controllers.SocialShareController")
                    property.Value = "Telerik.Sitefinity.Frontend.SocialShare.Mvc.Controllers.SocialShareController";
                else if (property.Value == "DynamicContent.Mvc.Controllers.DynamicContentController")
                    property.Value = "Telerik.Sitefinity.Frontend.DynamicContent.Mvc.Controllers.DynamicContentController";
            }
        }

        private static void ClearToolboxItems()
        {
            var configManager = ConfigManager.GetManager();
            var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();

            var pagesToolbox = toolboxesConfig.Toolboxes["PageControls"];
            if (pagesToolbox == null)
                return;

            var mvcWidgetsSection = pagesToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "MvcWidgets");
            if (mvcWidgetsSection != null)
            {
                FrontendModuleInstaller.RemoveToolboxItemIfExists(mvcWidgetsSection, "ContentBlock");
                FrontendModuleInstaller.RemoveToolboxItemIfExists(mvcWidgetsSection, "Navigation");
                FrontendModuleInstaller.RemoveToolboxItemIfExists(mvcWidgetsSection, "News");

                configManager.SaveSection(toolboxesConfig);
            }
        }

        private static void RemoveToolboxItemIfExists(ToolboxSection mvcWidgetsSection, string toolName)
        {
            var tool = mvcWidgetsSection.Tools.FirstOrDefault<ToolboxItem>(t => t.Name == toolName);
            if (tool != null)
                mvcWidgetsSection.Tools.Remove(tool);
        }

        private const string FrontendServiceName = "Telerik.Sitefinity.Frontend";

        #endregion
    }
}
