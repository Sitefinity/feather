using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Handles upgrades for Feather module
    /// </summary>
    internal static class FrontendModuleUpgrader
    {
        /// <summary>
        /// Upgrades the specified upgrade from.
        /// </summary>
        /// <param name="upgradeFrom">The upgrade from.</param>
        public static void Upgrade(Version upgradeFrom)
        {
            if (upgradeFrom < new Version(1, 2, 140, 0))
            {
                FrontendModuleUpgrader.DeleteOldGridSection();
                FrontendModuleUpgrader.UpdateContentBlockTitle();
            }

            if (upgradeFrom <= new Version(1, 2, 180, 1))
            {
                FrontendModuleUpgrader.RemoveMvcWidgetToolboxItems();
                FrontendModuleUpgrader.RenameDynamicContentMvcToolboxItems();
            }

            if (upgradeFrom <= new Version(1, 2, 260, 1))
            {
                FrontendModuleUpgrader.RecategorizePageTemplates();
            }

            if (upgradeFrom <= new Version(1, 2, 270, 1))
            {
                FrontendModuleUpgrader.UpdatePageTemplates();
            }

            if (upgradeFrom <= new Version(1, 2, 280, 2))
            {
                FrontendModuleUpgrader.CreateDefaultTemplates();
            }

            if (upgradeFrom <= new Version(1, 3, 320, 0))
            {
                FrontendModuleUpgrader.UpdateGridWidgetsToolbox();
                FrontendModuleUpgrader.UpdateGridWidgetPaths();
            }
        }

        // 1, 2, 140, 0
        private static void DeleteOldGridSection()
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

        // 1, 2, 140, 0
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Telerik.Sitefinity.Pages.Model.ControlData.set_Caption(System.String)")]
        private static void UpdateContentBlockTitle()
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

        // 1, 2, 180, 1
        private static void RemoveMvcWidgetToolboxItems()
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

        // 1, 2, 180, 1
        private static void RenameDynamicContentMvcToolboxItems()
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

        // 1, 2, 260, 1
        private static void RecategorizePageTemplates()
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

        // 1, 2, 270, 1
        private static void UpdatePageTemplates()
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

        // 1, 2, 280, 2
        private static void CreateDefaultTemplates()
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

        // 1, 3, 320, 0
        private static void UpdateGridWidgetsToolbox()
        {
            FrontendModuleUpgrader.TransferGridWidgetSectionToDefault("BootstrapGrids");
            FrontendModuleUpgrader.TransferGridWidgetSectionToDefault("FoundationGrids");
            FrontendModuleUpgrader.TransferGridWidgetSectionToDefault("SemanticUIGrids");
        }

        private static void TransferGridWidgetSectionToDefault(string sectionName)
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

        // 1, 3, 320, 0
        private static void UpdateGridWidgetPaths()
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
    }
}
