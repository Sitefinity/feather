using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.GridSystem
{
    /// <summary>
    /// This class contains logic for registering and initializing the grid controls.
    /// </summary>
    internal class GridSystemInitializer
    {
        /// <summary>
        /// Registers the grid controls.
        /// </summary>
        public virtual void Initialize()
        {
            var configManager = ConfigManager.GetManager();
            using (new ElevatedConfigModeRegion())
            {
                var toolboxConfig = configManager.GetSection<ToolboxesConfig>();
                var htmlLayoutsSection = this.GetOrCreateToolBoxSection(toolboxConfig);

                var baseTemplatePath = string.Format(
                                                    System.Globalization.CultureInfo.InvariantCulture,
                                                    GridSystemInitializer.GridFolderPathStringTemplate,
                                                    FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(FrontendService).Assembly));
                
                var layoutControls = this.CreateLayoutControlsData(baseTemplatePath);

                foreach (var layoutControl in layoutControls)
                {
                    this.AddLayoutControl(htmlLayoutsSection.Tools, layoutControl);
                }

                configManager.SaveSection(toolboxConfig);
            }
        }

        /// <summary>
        /// Gets existing or create a tool box section for the grid controls.
        /// </summary>
        /// <param name="toolboxConfig">The toolbox configuration.</param>
        /// <returns></returns>
        protected virtual ToolboxSection GetOrCreateToolBoxSection(ToolboxesConfig toolboxConfig)
        {
            var layoutsToolbox = toolboxConfig.Toolboxes["PageLayouts"];

            var htmlLayoutsSection = layoutsToolbox.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "HtmlLayouts");
            if (htmlLayoutsSection == null)
            {
                htmlLayoutsSection = new ToolboxSection(layoutsToolbox.Sections);
                htmlLayoutsSection.Name = "HtmlLayouts";

                /// TODO: Set resource class id and use resource keys for Title and Description.
                htmlLayoutsSection.Title = "Bootstrap grid widgets";

                layoutsToolbox.Sections.Add(htmlLayoutsSection);
            }

            return htmlLayoutsSection;
        }

        /// <summary>
        /// Creates the layout controls data.
        /// </summary>
        /// <param name="baseTemplatePath">The base template path.</param>
        /// <returns></returns>
        protected virtual List<GridControlData> CreateLayoutControlsData(string baseTemplatePath)
        {
            var layoutControls = new List<GridControlData>()
            {
                 new GridControlData() { Name = "Col1", Title = "12", LayoutTemplatePath = baseTemplatePath + "grid-12.html", CssClass = "sfL100" },
                        new GridControlData() { Name = "Col2T1", Title = "3 + 9", LayoutTemplatePath = baseTemplatePath + "grid-3+9.html", CssClass = "sfL25_75" },
                        new GridControlData() { Name = "Col2T2", Title = "4 + 8", LayoutTemplatePath = baseTemplatePath + "grid-4+8.html", CssClass = "sfL33_67" },
                        new GridControlData() { Name = "Col2T3", Title = "6 + 6", LayoutTemplatePath = baseTemplatePath + "grid-6+6.html", CssClass = "sfL50_50" },
                        new GridControlData() { Name = "Col2T4", Title = "8 + 4", LayoutTemplatePath = baseTemplatePath + "grid-8+4.html", CssClass = "sfL67_33" },
                        new GridControlData() { Name = "Col2T5", Title = "9 + 3", LayoutTemplatePath = baseTemplatePath + "grid-9+3.html", CssClass = "sfL75_25" },
                        new GridControlData() { Name = "Col3T1", Title = "4 + 4 + 4", LayoutTemplatePath = baseTemplatePath + "grid-4+4+4.html", CssClass = "sfL33_34_33" },
                        new GridControlData() { Name = "Col3T2", Title = "3 + 6 + 3", LayoutTemplatePath = baseTemplatePath + "grid-3+6+3.html", CssClass = "sfL25_50_25" },
                        new GridControlData() { Name = "Col4T1", Title = "3 + 3 + 3 + 3", LayoutTemplatePath = baseTemplatePath + "grid-3+3+3+3.html", CssClass = "sfL25_25_25_25" },
                        new GridControlData() { Name = "Col5T1", Title = "2 + 3 + 2 + 3 + 2", LayoutTemplatePath = baseTemplatePath + "grid-2+3+2+3+2.html", CssClass = "sfL20_20_20_20_20" },
                        new GridControlData() { Name = "Col1T1", Title = "container", LayoutTemplatePath = baseTemplatePath + "container.html", CssClass = "sfL100" }
            };

            return layoutControls;
        }

        /// <summary>
        /// Adds the layout control.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="data">The data.</param>
        protected virtual void AddLayoutControl(ConfigElementList<ToolboxItem> parent, GridControlData data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            var control = parent.FirstOrDefault<ToolboxItem>(t => t.Name == data.Name);

            if (control == null)
            {
                control = new ToolboxItem(parent);
                control.ControlType = typeof(GridControl).AssemblyQualifiedName;
                control.Name = data.Name;
                control.Title = data.Title;
                control.LayoutTemplate = data.LayoutTemplatePath;
                control.CssClass = data.CssClass;
                parent.Add(control);
            }
        }

        /// <summary>
        /// The grid folder path string template
        /// </summary>
        public const string GridFolderPathStringTemplate = "~/{0}GridSystem/Templates/";
    }
}
