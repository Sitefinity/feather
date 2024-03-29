﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.GridSystem
{
    /// <summary>
    /// This class register the grid template files in the PageLayout section of the toolbox.
    /// </summary>
    internal class GridWidgetRegistrator
    {
        #region Public methods

        /// <summary>
        /// Registers the grid widget in toolbox.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        public void RegisterToolboxItem(string fileName, string oldFileName = "")
        {
            ConfigManager configManager = null;
            var toolboxConfig = Config.Get<ToolboxesConfig>();
            var layoutsToolbox = toolboxConfig.Toolboxes[ToolboxesConfig.LayoutsToolboxName];
            var htmlLayoutsSection = layoutsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == GridWidgetRegistrator.GridSectionName);
            if (htmlLayoutsSection == null)
            {
                configManager = ConfigManager.GetManager();
                toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                layoutsToolbox = toolboxConfig.Toolboxes[ToolboxesConfig.LayoutsToolboxName];
                htmlLayoutsSection = this.CreateToolBoxSection(layoutsToolbox, GridWidgetRegistrator.GridSectionName, GridWidgetRegistrator.GridSectionTitle);
            }

            var layoutControl = this.CreateGridControlsData(fileName);

            string toolboxItemName;
            if (oldFileName.IsNullOrEmpty())
            {
                toolboxItemName = layoutControl.Name;
            }
            else
            {
                var oldFileNameWithoutExtension = this.GetFileNameWithoutExtension(oldFileName);
                toolboxItemName = oldFileNameWithoutExtension;
            }

            ToolboxItem toolboxItem = null;
            if (configManager == null)
                toolboxItem = htmlLayoutsSection.Tools.FirstOrDefault<ToolboxItem>(t => t.Name == toolboxItemName);

            if (toolboxItem == null || (toolboxItem.Name != layoutControl.Name || toolboxItem.Title != layoutControl.Title || toolboxItem.LayoutTemplate != layoutControl.LayoutTemplatePath))
            {
                if (configManager == null)
                {
                    configManager = ConfigManager.GetManager();
                    toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                    layoutsToolbox = toolboxConfig.Toolboxes[ToolboxesConfig.LayoutsToolboxName];
                    htmlLayoutsSection = layoutsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == GridWidgetRegistrator.GridSectionName);
                }

                this.AddOrRenameGridControl(htmlLayoutsSection, layoutControl, toolboxItemName);
            }

            if (configManager != null)
            {
                using (new ElevatedConfigModeRegion())
                    configManager.SaveSection(toolboxConfig);
            }
        }

        /// <summary>
        /// Unregisters the toolbox item.
        /// </summary>
        /// <param name="fileName">Name of the content type.</param>
        public virtual void UnregisterToolboxItem(string fileName)
        {
            var configurationManager = ConfigManager.GetManager();
            var toolboxesConfig = configurationManager.GetSection<ToolboxesConfig>();
            var pageControls = toolboxesConfig.Toolboxes["PageLayouts"];
            var sectionName = GridWidgetRegistrator.GridSectionName;

            var section = pageControls.Sections.Where<ToolboxSection>(e => e.Name == sectionName).FirstOrDefault();
            if (section != null)
            {
                var fileNameWithoutExtension = this.GetFileNameWithoutExtension(fileName);
                var itemToDelete = section.Tools.FirstOrDefault<ToolboxItem>(e => e.Name == fileNameWithoutExtension);

                if (itemToDelete != null && itemToDelete.Source == ConfigSource.Database)
                {
                    section.Tools.Remove(itemToDelete);
                }

                if (!section.Tools.Any<ToolboxItem>() && section.Source == ConfigSource.Database)
                {
                    pageControls.Sections.Remove(section);
                }
            }

            configurationManager.SaveSection(toolboxesConfig);
        }

        /// <summary>
        /// Updates the control data.
        /// </summary>
        /// <param name="newFileName">New name of the file.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        public void UpdateControlData(string newFileName, string oldFileName)
        {
            var pageManger = PageManager.GetManager();

            var baseTemplatePath = string.Format(
            CultureInfo.InvariantCulture,
            GridWidgetRegistrator.GridFolderPathStringTemplate,
            FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(FrontendModule).Assembly));

            var properties = pageManger.GetProperties().Where(prop => prop.Name == "Layout" && prop.Value == baseTemplatePath + oldFileName).ToList();
            var newCaption = this.GetFileNameWithoutExtension(newFileName);
            foreach (var property in properties)
            {
                property.Value = baseTemplatePath + newFileName;
                ((ControlData)property.Control).Caption = newCaption;
            }

            pageManger.SaveChanges();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Gets existing or create a tool box section for the grid controls.
        /// </summary>
        /// <param name="toolbox">The toolbox configuration.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="sectionTitle">The section title.</param>
        /// <returns>The section.</returns>
        protected virtual ToolboxSection CreateToolBoxSection(Toolbox toolbox, string sectionName, string sectionTitle)
        {
            var htmlLayoutsSection = new ToolboxSection(toolbox.Sections);
            htmlLayoutsSection.Name = sectionName;

            /// TODO: Set resource class id and use resource keys for Title and Description.
            htmlLayoutsSection.Title = sectionTitle;

            toolbox.Sections.Add(htmlLayoutsSection);

            return htmlLayoutsSection;
        }

        /// <summary>
        /// Adds or renames the grid control.
        /// </summary>
        /// <param name="section">The parent toolbox section.</param>
        /// <param name="data">The data.</param>
        /// <param name="toolboxItemName">Old name of the file.</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        protected virtual void AddOrRenameGridControl(ToolboxSection section, GridControlData data, string toolboxItemName = default(string))
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var tools = section.Tools;

            var control = tools.FirstOrDefault<ToolboxItem>(t => t.Name == toolboxItemName);

            if (control == null)
            {
                control = new ToolboxItem(tools);
                control.ControlType = typeof(GridControl).AssemblyQualifiedName;
                control.CssClass = data.CssClass;
                tools.Add(control);
            }

            control.Name = data.Name;
            control.Title = data.Title;
            control.LayoutTemplate = data.LayoutTemplatePath;
        }

        /// <summary>
        /// Creates the grid controls data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        protected GridControlData CreateGridControlsData(string fileName)
        {
            var baseTemplatePath = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                GridWidgetRegistrator.GridFolderPathStringTemplate,
                FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(FrontendModule).Assembly));

            var fileNameWithoutExtension = this.GetFileNameWithoutExtension(fileName);
            var cssClass = this.GetInferredCssClass(fileNameWithoutExtension);

            var layoutData = new GridControlData() 
            {
                Name = fileNameWithoutExtension, 
                Title = fileNameWithoutExtension, 
                LayoutTemplatePath = baseTemplatePath + fileName,
                CssClass = cssClass
            };

            return layoutData;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private string GetFileNameWithoutExtension(string fileName)
        {
            var extension = fileName.Split('.').LastOrDefault();
            var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - (extension.Length + 1));

            return fileNameWithoutExtension;
        }

        private string GetInferredCssClass(string fileNameWithoutExtension)
        {
            if (this.cssClassMapping.ContainsKey(fileNameWithoutExtension))
            {
                return this.cssClassMapping[fileNameWithoutExtension];
            }
            else
            {
                return null;
            }
        }

        private readonly Dictionary<string, string> cssClassMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "grid-12", "sfL100" },
            { "grid-3+9", "sfL25_75" },
            { "grid-4+8", "sfL33_67" },
            { "grid-6+6", "sfL50_50" },
            { "grid-8+4", "sfL67_33" },
            { "grid-9+3", "sfL75_25" },
            { "grid-4+4+4", "sfL33_34_33" },
            { "grid-3+6+3", "sfL25_50_25" },
            { "grid-3+3+3+3", "sfL25_25_25_25" },
            { "grid-2+3+2+3+2", "sfL20_20_20_20_20" },
            { "container", "sfL100" }
        };

        #endregion

        #region Constants

        /// <summary>
        /// The grid folder path string template
        /// </summary>
        public const string GridFolderPathStringTemplate = "~/{0}GridSystem/Templates/";

        /// <summary>
        /// The grid section name.
        /// </summary>
        public const string GridSectionName = "Grids";

        /// <summary>
        /// The grid section title.
        /// </summary>
        public const string GridSectionTitle = "Grid widgets";

        #endregion
    }
}