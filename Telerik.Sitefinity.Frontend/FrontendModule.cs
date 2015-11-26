using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Ninject;
using Telerik.Microsoft.Practices.Unity;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Frontend.Designers;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.GridSystem;
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

            this.ninjectDependencyResolver = new StandardKernel();
            this.ninjectDependencyResolver.Load(ControllerContainerInitializer.ControllerContainerAssemblies);
        }

        /// <summary>
        /// This method is invoked during the unload process of an active module from Sitefinity, e.g. when a module is deactivated. For instance this method is also invoked for every active module during a restart of the application. 
        /// Typically you will use this method to unsubscribe the module from all events to which it has subscription.
        /// </summary>
        public override void Unload()
        {
            this.Uninitialize();
            base.Unload();
        }

        /// <summary>
        /// Uninstall the module from Sitefinity system. Deletes the module artifacts added with Install method.
        /// </summary>
        /// <param name="initializer">The site initializer instance.</param>
        public override void Uninstall(SiteInitializer initializer)
        {
            this.Uninitialize();
            this.Unistall(initializer);
            base.Uninstall(initializer);
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

            if (upgradeFrom <= new Version(1, 3, 320, 0))
            {
                this.UpdateGridWidgetsToolbox();
                this.UpdateGridWidgetPaths();
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
                foreach (var initializer in this.initializers.Value)
                {
                    initializer.Initialize();
                }

                ObjectFactory.Container.RegisterType<ICommentNotificationsStrategy, Telerik.Sitefinity.Frontend.Modules.Comments.ReviewNotificationStrategy>(new ContainerControlledLifetimeManager());
            }
        }

        private void Unistall(SiteInitializer initializer)
        {
            var featherWidgetTypes = new List<string>();
            var configManager = ConfigManager.GetManager();
            var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();
            var pageManager = initializer.PageManager;

            foreach (var toolbox in toolboxesConfig.Toolboxes.Values)
            {
                foreach (var section in toolbox.Sections)
                {
                    var featherWidgets = ((ICollection<ToolboxItem>)section.Tools).Where(i => !i.ControllerType.IsNullOrEmpty() && i.ControllerType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal));
                    featherWidgetTypes.AddRange(featherWidgets.Select(t => t.ControllerType));

                    var mvcToolsToDelete = featherWidgets.Select(i => i.GetKey());
                    foreach (var key in mvcToolsToDelete)
                    {
                        section.Tools.Remove(section.Tools.Elements.SingleOrDefault(e => e.GetKey() == key));
                    }
                }
            }

            // Delete widgets from pages
            this.DeleteControls(pageManager);

            configManager.SaveSection(toolboxesConfig);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private void DeleteControls(PageManager pageManager)
        {
            List<ControlData> controlsToDelete = new List<ControlData>();
            controlsToDelete.AddRange(this.GetControlsToDelete(pageManager));

            List<PageData> pagesToInvalidate = new List<PageData>();
            foreach (var control in controlsToDelete)
            {
                if (control is PageControl)
                {
                    var pageForInvalidation = ((PageControl)control).Page;
                    if (pageForInvalidation != null)
                        pagesToInvalidate.Add(pageForInvalidation);
                }
                else if (control is TemplateControl)
                {
                    pagesToInvalidate.AddRange(((TemplateControl)control).Page.Pages());
                }

                pageManager.Delete(control);
            }

            foreach (var page in pagesToInvalidate.Distinct())
                page.BuildStamp++;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private List<ControlData> GetControlsToDelete(PageManager pageManager)
        {
            List<ControlData> controlsToDelete;
            try
            {
                // Could fail if there are persisted records for not loaded types inherited from ControlData
                controlsToDelete = this.GetFeatherControlsToDelete<ControlData>(pageManager);
            }
            catch
            {
                controlsToDelete = new List<ControlData>();
                controlsToDelete.AddRange(this.GetFeatherControlsToDelete<PageControl>(pageManager));
                controlsToDelete.AddRange(this.GetFeatherControlsToDelete<PageDraftControl>(pageManager));
                controlsToDelete.AddRange(this.GetFeatherControlsToDelete<TemplateControl>(pageManager));
                controlsToDelete.AddRange(this.GetFeatherControlsToDelete<TemplateDraftControl>(pageManager));
            }

            return controlsToDelete;
        }

        private List<ControlData> GetFeatherControlsToDelete<TControlData>(PageManager pageManager) where TControlData : ControlData
        {
            List<ControlData> controlsToDelete = new List<ControlData>();

            controlsToDelete.AddRange(pageManager.GetControls<TControlData>().Where(c => c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend"))).ToList());

            return controlsToDelete;
        }

        private void Uninitialize()
        {
            if (this.ninjectDependencyResolver != null && !this.ninjectDependencyResolver.IsDisposed)
                this.ninjectDependencyResolver.Dispose();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;

            foreach (var initializer in this.initializers.Value)
            {
                initializer.Uninitialize();
            }

            // Force mvc initialization to run again after feather uninstalls
            typeof(SystemManager).GetField("mvcEnabled", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
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
                    layoutManager.CreateDefaultTemplates("Bootstrap", "default");
                else if (string.Equals(LayoutFileManager.FoundationDefaultTemplateName, template.Name, StringComparison.OrdinalIgnoreCase))
                    layoutManager.CreateDefaultTemplates("Foundation", "default");
                else if (string.Equals(LayoutFileManager.SemanticUIDefaultTemplateName, template.Name, StringComparison.OrdinalIgnoreCase))
                    layoutManager.CreateDefaultTemplates("SemanticUI", "default");
            }
        }

        private void UpdateGridWidgetPaths()
        {
            const int BATCH = 200;

            var pathPairs = new Dictionary<string, string>()
            {
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-11+5.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-8+4.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-12+4.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-9+3.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-5+11.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+8.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+12.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+9.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-five-cols.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-2+3+2+3+2.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-four-cols.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-3+3+3+3.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-one-col.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-12.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-three-cols.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-4+4+4.html" },
                { "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-two-cols.html", "~/Frontend-Assembly/Telerik.Sitefinity.Frontend/GridSystem/Templates/grid-6+6.html" }
            };

            var pageManager = PageManager.GetManager();
            var oldPaths = pathPairs.Keys.ToArray();

            var layoutControlIds = pageManager.GetControls<ControlData>().Where(c => c.IsLayoutControl).Select(c => c.Id).ToArray();
            var currentControl = 0;
            while (currentControl < layoutControlIds.Length)
            {
                var batchArray = layoutControlIds.Skip(currentControl).Take(BATCH).ToArray();

                var propertiesToUpdate = pageManager.GetProperties()
                    .Where(p => batchArray.Contains(p.Control.Id))
                    .Where(p => oldPaths.Contains(p.Value))
                    .ToArray();

                foreach (var property in propertiesToUpdate)
                    property.Value = pathPairs[property.Value];

                if (propertiesToUpdate.Length > 0)
                    pageManager.SaveChanges();

                currentControl += BATCH;
            }
        }

        private void UpdateGridWidgetsToolbox()
        {
            this.TransferGridWidgetSectionToDefault("BootstrapGrids");
            this.TransferGridWidgetSectionToDefault("FoundationGrids");
            this.TransferGridWidgetSectionToDefault("SemanticUIGrids");
        }

        private void TransferGridWidgetSectionToDefault(string sectionName)
        {
            var layoutConfig = Config.Get<ToolboxesConfig>().Toolboxes["PageLayouts"];
            var section = layoutConfig.Sections.FirstOrDefault<ToolboxSection>(e => e.Name == sectionName);
            if (section != null)
            {
                var registrator = new GridWidgetRegistrator();
                foreach (var tool in section.Tools)
                {
                    if (tool.LayoutTemplate.IsNullOrEmpty())
                        continue;

                    registrator.RegisterToolboxItem(System.Web.VirtualPathUtility.GetFileName(tool.LayoutTemplate));
                }

                var configurationManager = ConfigManager.GetManager();
                using (new ElevatedConfigModeRegion())
                {
                    var toolboxesConfig = configurationManager.GetSection<ToolboxesConfig>();
                    var pageControls = toolboxesConfig.Toolboxes["PageLayouts"];

                    var sectionToDelete = pageControls.Sections.FirstOrDefault<ToolboxSection>(e => e.Name == sectionName);
                    pageControls.Sections.Remove(sectionToDelete);

                    configurationManager.SaveSection(toolboxesConfig);
                }
            }
        }

        private IKernel ninjectDependencyResolver;
        private Lazy<IEnumerable<IInitializer>> initializers = new Lazy<IEnumerable<IInitializer>>(() =>
            typeof(FrontendModule).Assembly.GetTypes().Where(t => typeof(IInitializer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).Select(t => Activator.CreateInstance(t) as IInitializer).ToList());

        private const string FrontendServiceName = "Telerik.Sitefinity.Frontend";
    }
}
