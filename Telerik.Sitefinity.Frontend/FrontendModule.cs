using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Ninject;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.Resources;
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
    /// A module that will be invoked by Sitefinity.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Field is disposed on Unload.")]
    public class FrontendModule : ModuleBase
    {
        /// <summary>
        /// Gets the current instance of the module.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static FrontendModule Current
        {
            get
            {
                return (FrontendModule)SystemManager.GetModule("Feather");
            }
        }

        /// <summary>
        /// Gets the landing page id for each module inherit from <see cref="SecuredModuleBase"/> class.
        /// </summary>
        /// <value>The landing page id.</value>
        public override Guid LandingPageId
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// Gets the CLR types of all data managers provided by this module.
        /// </summary>
        /// <value>An array of <see cref="Type"/> objects.</value>
        public override Type[] Managers
        {
            get { return new Type[0]; }
        }

        /// <summary>
        /// Gets the dependency resolver. Can be used for overriding the default implementations of some interfaces.
        /// </summary>
        /// <value>
        /// The dependency resolver.
        /// </value>
        public IKernel DependencyResolver
        {
            get
            {
                return this.ninjectDependencyResolver;
            }
        }

        /// <summary>
        /// Installs the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public override void Install(SiteInitializer initializer)
        {
            if (this.FrontendServiceExists())
            {
                this.InitialUpgrade(initializer);
            }
        }

        /// <summary>
        /// Initializes the service with specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
            Bootstrapper.Initialized += this.Bootstrapper_Initialized;

            SystemManager.RegisterServiceStackPlugin(new ListsServiceStackPlugin());
            SystemManager.RegisterServiceStackPlugin(new FilesServiceStackPlugin());
            SystemManager.RegisterServiceStackPlugin(new ReviewsServiceStackPlugin());

            this.controllerAssemblies = new ControllerContainerInitializer().RetrieveAssemblies();

            this.ninjectDependencyResolver = new StandardKernel();
            this.ninjectDependencyResolver.Load(this.controllerAssemblies);
        }

        /// <summary>
        /// This method is invoked during the unload process of an active module from Sitefinity, e.g. when a module is deactivated. For instance this method is also invoked for every active module during a restart of the application. 
        /// Typically you will use this method to unsubscribe the module from all events to which it has subscription.
        /// </summary>
        public override void Unload()
        {
            if (this.ninjectDependencyResolver != null && !this.ninjectDependencyResolver.IsDisposed)
                this.ninjectDependencyResolver.Dispose();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;

            base.Unload();
        }

        /// <summary>
        /// Upgrades this module from the specified version.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        /// <param name="upgradeFrom">The version this module us upgrading from.</param>
        public override void Upgrade(SiteInitializer initializer, Version upgradeFrom)
        {
            base.Upgrade(initializer, upgradeFrom);

            if (upgradeFrom < new Version(1, 2, 140, 0))
            {
                this.DeleteOldGridSection();
                this.UpdateContentBlockTitle();
            }

            if (upgradeFrom <= new Version(1, 2, 180, 1))
            {
                this.RemoveMvcWidgetToolboxItems();
                this.RenameDynamicContentMvcToolboxItems();
            }

            if (upgradeFrom <= new Version(1, 2, 260, 1))
            {
                this.RecategorizePageTemplates();
            }

            if (upgradeFrom <= new Version(1, 2, 270, 1))
            {
                this.UpdatePageTemplates();
            }

            if (upgradeFrom <= new Version(1, 2, 280, 2))
            {
                this.CreateDefaultTemplates();
            }
        }

        /// <summary>
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            return null;
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
                var resourcesInitializer = new ResourcesInitializer();
                resourcesInitializer.Initialize();

                var fileMonitoringInitilizer = new FileMonitoringInitializer();
                fileMonitoringInitilizer.Initialize();

                var controllerContainerInitializer = new ControllerContainerInitializer();
                controllerContainerInitializer.Initialize(this.controllerAssemblies);
                this.controllerAssemblies = null; // We won't be needing those anymore. Set them free.

                var layoutsInitializer = new LayoutInitializer();
                layoutsInitializer.Initialize();

                var designerInitializer = new DesignerInitializer();
                designerInitializer.Initialize();

                ObjectFactory.Container.RegisterType<ICommentNotificationsStrategy, Telerik.Sitefinity.Frontend.Modules.Comments.ReviewNotificationStrategy>(new ContainerControlledLifetimeManager());
            }
        }

        private void InitialUpgrade(SiteInitializer initializer)
        {
            this.RemoveFrontendService();
            this.RenameControllers(initializer);
            this.ClearToolboxItems();
        }
        
        private void ClearToolboxItems()
        {
            var configManager = ConfigManager.GetManager();
            var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();

            var pagesToolbox = toolboxesConfig.Toolboxes["PageControls"];
            if (pagesToolbox == null)
                return;

            var mvcWidgetsSection = pagesToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "MvcWidgets");
            if (mvcWidgetsSection != null)
            {
                this.RemoveToolboxItemIfExists(mvcWidgetsSection, "ContentBlock");
                this.RemoveToolboxItemIfExists(mvcWidgetsSection, "Navigation");
                this.RemoveToolboxItemIfExists(mvcWidgetsSection, "News");

                configManager.SaveSection(toolboxesConfig);
            }
        }

        private void RemoveToolboxItemIfExists(ToolboxSection mvcWidgetsSection, string toolName)
        {
            var tool = mvcWidgetsSection.Tools.FirstOrDefault<ToolboxItem>(t => t.Name == toolName);
            if (tool != null)
                mvcWidgetsSection.Tools.Remove(tool);
        }

        private void RenameControllers(SiteInitializer initializer)
        {
            var manager = initializer.PageManager;
            var controllerNameProperties = manager.GetControls<ControlData>()
                .Where(c => c.ObjectType == "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy" || c.ObjectType == "Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.MvcWidgetProxy, Telerik.Sitefinity.Frontend")
                .SelectMany(c => c.Properties.Where(p => p.Name == "ControllerName"));

            foreach (var property in controllerNameProperties)
            {
                if (property.Value == "News.Mvc.Controllers.NewsController")
                {
                    property.Value = "Telerik.Sitefinity.Frontend.News.Mvc.Controllers.NewsController";
                }
                else if (property.Value == "ContentBlock.Mvc.Controllers.ContentBlockController")
                {
                    property.Value = "Telerik.Sitefinity.Frontend.ContentBlock.Mvc.Controllers.ContentBlockController";
                }
                else if (property.Value == "Navigation.Mvc.Controllers.NavigationController")
                {
                    property.Value = "Telerik.Sitefinity.Frontend.Navigation.Mvc.Controllers.NavigationController";
                }
                else if (property.Value == "SocialShare.Mvc.Controllers.SocialShareController")
                {
                    property.Value = "Telerik.Sitefinity.Frontend.SocialShare.Mvc.Controllers.SocialShareController";
                }
                else if (property.Value == "DynamicContent.Mvc.Controllers.DynamicContentController")
                {
                    property.Value = "Telerik.Sitefinity.Frontend.DynamicContent.Mvc.Controllers.DynamicContentController";
                }
            }
        }

        private void RemoveMvcWidgetToolboxItems()
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var pageControls = toolboxConfig.Toolboxes["PageControls"];

                foreach (var section in pageControls.Sections)
                {
                    var widgets = section.Tools.Where<ToolboxItem>(t => t.ControllerType.StartsWith("Telerik.Sitefinity.Frontend.", StringComparison.Ordinal) && !t.ControllerType.StartsWith("Telerik.Sitefinity.Frontend.DynamicContent", StringComparison.Ordinal)).ToArray();
                    foreach (ToolboxItem tool in widgets)
                    {
                        section.Tools.Remove(tool);
                    }
                }

                var mvcWidgetsSection = pageControls.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "MvcWidgets");
                if (mvcWidgetsSection != null)
                {
                    pageControls.Sections.Remove(mvcWidgetsSection);
                }

                configManager.SaveSection(toolboxConfig);
            }
        }

        private void RenameDynamicContentMvcToolboxItems()
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var pageControls = toolboxConfig.Toolboxes["PageControls"];

                foreach (var section in pageControls.Sections)
                {
                    var widgets = section.Tools.Where<ToolboxItem>(t => t.ControllerType.StartsWith("Telerik.Sitefinity.Frontend.DynamicContent", StringComparison.Ordinal)).ToArray();
                    foreach (ToolboxItem tool in widgets)
                    {
                        tool.CssClass = "sfNewsViewIcn sfMvcIcn";
                        var indexOfMvcSuffix = tool.Title.LastIndexOf(" MVC", StringComparison.Ordinal);
                        tool.Title = tool.Title.Substring(0, indexOfMvcSuffix);
                    }
                }

                configManager.SaveSection(toolboxConfig);
            }
        }

        private void RemoveFrontendService()
        {
            var configManager = ConfigManager.GetManager();
            var systemConfig = configManager.GetSection<SystemConfig>();
            systemConfig.SystemServices.Remove(FrontendModule.FrontendServiceName);
            configManager.SaveSection(systemConfig);
        }

        private bool FrontendServiceExists()
        {
            var systemConfig = Config.Get<SystemConfig>();
            return systemConfig.SystemServices.ContainsKey(FrontendModule.FrontendServiceName);
        }

        private void DeleteOldGridSection()
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var layoutsToolbox = toolboxConfig.Toolboxes["PageLayouts"];
                if (layoutsToolbox != null)
                {
                    var htmlLayoutsSection = layoutsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "HtmlLayouts");
                    if (htmlLayoutsSection != null)
                    {
                        layoutsToolbox.Sections.Remove(htmlLayoutsSection);
                        configManager.SaveSection(toolboxConfig);
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Sitefinity.Pages.Model.ControlData.set_Caption(System.String)")]
        private void UpdateContentBlockTitle()
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var controlsToolbox = toolboxConfig.Toolboxes["PageControls"];
                if (controlsToolbox != null)
                {
                    var mvcWidgetsSection = controlsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "MvcWidgets");
                    if (mvcWidgetsSection != null)
                    {
                        var contentBlockTool = mvcWidgetsSection.Tools.FirstOrDefault<ToolboxItem>(t => t.Name == "ContentBlock");
                        if (contentBlockTool != null)
                        {
                            contentBlockTool.Title = "Content Block";
                            configManager.SaveSection(toolboxConfig);
                        }
                    }
                }
            }

            var pageManager = PageManager.GetManager();
            using (new ElevatedModeRegion(pageManager))
            {
                var contentBlocks = pageManager.GetControls<ControlData>().Where(c => c.ObjectType == "Telerik.Sitefinity.Mvc.Proxy.MvcControllerProxy" && c.Caption == "ContentBlock").ToArray();
                foreach (var contentBlock in contentBlocks)
                {
                    contentBlock.Caption = "Content Block";
                }

                pageManager.SaveChanges();
            }
        }

        private void RecategorizePageTemplates()
        {
            var pageManager = PageManager.GetManager();
            var layoutManager = new LayoutFileManager();

            var customPageTemplates = pageManager.GetTemplates().Where(pt => pt.Category == SiteInitializer.CustomTemplatesCategoryId).ToArray();
            foreach (var customPageTemplate in customPageTemplates)
            {
                var titleTokens = customPageTemplate.Title.ToString().Split('.');
                if (titleTokens.Length > 1 && (new PackageManager()).PackageExists(titleTokens[0]))
                {
                    customPageTemplate.Category = layoutManager.GetOrCreateTemplateCategoryId(titleTokens[0]);
                }
            }

            pageManager.SaveChanges();
        }

        private void UpdatePageTemplates()
        {
            var pageManager = PageManager.GetManager();
            var layoutFileManager = new LayoutFileManager();

            var defaultPageTemplates = pageManager.GetTemplates().Where(pt => LayoutFileManager.DefaultTemplateNames.Contains(pt.Name)).ToArray();
            foreach (var defaultPageTemplate in defaultPageTemplates)
            {
                // Renaming template title
                var titleParts = defaultPageTemplate.Title.ToString().Split('.');
                if (titleParts.Length > 1)
                {
                    defaultPageTemplate.Title = titleParts[1];
                }

                // Adding icon to title
                layoutFileManager.AttachImageToTemplate(defaultPageTemplate, pageManager, "default");
            }

            pageManager.SaveChanges();
        }

        private void CreateDefaultTemplates()
        {
            var layoutManager = new LayoutFileManager();

            var pageManager = PageManager.GetManager();
            var defaultPageTemplates = pageManager.GetTemplates().Where(pt => LayoutFileManager.DefaultTemplateNames.Contains(pt.Name));
            foreach (var template in defaultPageTemplates)
            {
                if (string.Equals(LayoutFileManager.BootstrapDefaultTemplateName, template.Name, StringComparison.OrdinalIgnoreCase))
                    layoutManager.CreateDefaultBootstrapTemplates();
                else if (string.Equals(LayoutFileManager.FoundationDefaultTemplateName, template.Name, StringComparison.OrdinalIgnoreCase))
                    layoutManager.CreateDefaultFoundationTemplates();
                else if (string.Equals(LayoutFileManager.SemanticUIDefaultTemplateName, template.Name, StringComparison.OrdinalIgnoreCase))
                    layoutManager.CreateDefaultSemanticUiTemplates();
            }
        }

        private IKernel ninjectDependencyResolver;
        private IEnumerable<Assembly> controllerAssemblies;

        private const string FrontendServiceName = "Telerik.Sitefinity.Frontend";
    }
}
