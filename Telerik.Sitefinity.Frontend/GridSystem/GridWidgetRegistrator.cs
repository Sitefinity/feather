using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.GridSystem
{
    /// <summary>
    /// This class register the grid template files in the PageLayout section of the toolbox.
    /// </summary>
    internal class GridWidgetRegistrator
    {
        /// <summary>
        /// Registers the grid widget in toolbox.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        public void RegisterInToolbox(string fileName, string packageName, string oldFileName = "")
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var sectionName = packageName + GridWidgetRegistrator.GridSectionNameSuffix;
                var sectionTitle = packageName + GridWidgetRegistrator.GridSectionTitleSuffix;
                var htmlLayoutsSection = this.GetOrCreateToolBoxSection(toolboxConfig, sectionName, sectionTitle);

                var layoutControl = this.CreateLayoutControlsData(fileName);
                this.AddOrRenameLayoutControl(htmlLayoutsSection.Tools, layoutControl, oldFileName);

                configManager.SaveSection(toolboxConfig);
            }
        }

        /// <summary>
        /// Gets existing or create a tool box section for the grid controls.
        /// </summary>
        /// <param name="toolboxConfig">The toolbox configuration.</param>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="sectionTitle">The section title.</param>
        /// <returns></returns>
        protected virtual ToolboxSection GetOrCreateToolBoxSection(ToolboxesConfig toolboxConfig, string sectionName, string sectionTitle)
        {
            var layoutsToolbox = toolboxConfig.Toolboxes["PageLayouts"];

            var htmlLayoutsSection = layoutsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == sectionName);
            if (htmlLayoutsSection == null)
            {
                htmlLayoutsSection = new ToolboxSection(layoutsToolbox.Sections);
                htmlLayoutsSection.Name = sectionName;

                /// TODO: Set resource class id and use resource keys for Title and Description.
                htmlLayoutsSection.Title = sectionTitle;

                layoutsToolbox.Sections.Add(htmlLayoutsSection);
            }

            return htmlLayoutsSection;
        }

        /// <summary>
        /// Adds or renames the layout control.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="data">The data.</param>
        /// <param name="oldFileName">Old name of the file.</param>
        /// <exception cref="System.ArgumentNullException">data</exception>
        protected virtual void AddOrRenameLayoutControl(ConfigElementList<ToolboxItem> parent, GridControlData data, string oldFileName = "")
        {
            if (data == null)
                throw new ArgumentNullException("data");

            string toolboxItemName;
            if (oldFileName.IsNullOrEmpty())
            {
                toolboxItemName = data.Name;
            }
            else
            {
                var extension = oldFileName.Split('.').LastOrDefault();
                var oldFileNameWithoutExtension = oldFileName.Substring(0, oldFileName.Length - (extension.Length + 1));
                toolboxItemName = oldFileNameWithoutExtension;
            }

            var control = parent.FirstOrDefault<ToolboxItem>(t => t.Name == toolboxItemName);

            if (control == null)
            {
                control = new ToolboxItem(parent);
                control.ControlType = typeof(GridControl).AssemblyQualifiedName;
                control.CssClass = data.CssClass;
                parent.Add(control);
            }

            control.Name = data.Name;
            control.Title = data.Title;
            control.LayoutTemplate = data.LayoutTemplatePath;
        }

        /// <summary>
        /// Creates the layout controls data.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        private GridControlData CreateLayoutControlsData(string fileName)
        {
            var baseTemplatePath = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                GridWidgetRegistrator.GridFolderPathStringTemplate,
                FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(FrontendService).Assembly));

            var extension = fileName.Split('.').LastOrDefault();
            var fileNameWithoutExtension = fileName.Substring(0, fileName.Length - (extension.Length + 1));

            var layoutData = new GridControlData() { Name = fileNameWithoutExtension, Title = fileNameWithoutExtension, LayoutTemplatePath = baseTemplatePath + fileName };

            return layoutData;
        }

        /// <summary>
        /// The grid folder path string template
        /// </summary>
        public const string GridFolderPathStringTemplate = "~/{0}GridSystem/Templates/";

        /// <summary>
        /// The grid section name suffix
        /// </summary>
        public const string GridSectionNameSuffix = "Grids";

        /// <summary>
        /// The grid section title suffix
        /// </summary>
        public const string GridSectionTitleSuffix = " grid widgets";
    }
}