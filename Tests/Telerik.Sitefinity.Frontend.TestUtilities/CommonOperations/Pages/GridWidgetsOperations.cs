using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.Pages
{
    public class GridWidgetsOperations
    {
        public ToolboxItem GridControlToolboxItem(ToolboxSection section, string gridName, string css, string virtualPath)
        {
            var toolboxItem = new ToolboxItem(section.Tools)
            {
                Enabled = true,
                Name = gridName,
                Title = gridName,
                CssClass = css,
                ControlType = typeof(GridControl).AssemblyQualifiedName,
                LayoutTemplate = virtualPath
            };

            return toolboxItem;
        }

        public void AddGridControlToToolboxesConfig(string gridName, string css, string gridPath)
        {
            ConfigManager configurationManager = ConfigManager.GetManager();
            var toolboxesConfig = configurationManager.GetSection<ToolboxesConfig>();
            var pageControls = toolboxesConfig.Toolboxes["PageLayouts"];

            var section = pageControls.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "HtmlLayouts");

            var toolboxItem = this.GridControlToolboxItem(section, gridName, css, gridPath);

            section.Tools.Add(toolboxItem);

            configurationManager.SaveSection(toolboxesConfig);
        }

        public void RemoveGridControlFromToolboxesConfig(string gridName)
        {
            ConfigManager configurationManager = ConfigManager.GetManager();
            var toolboxesConfig = configurationManager.GetSection<ToolboxesConfig>();
            var pageControls = toolboxesConfig.Toolboxes["PageLayouts"];

            var section = pageControls.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == "HtmlLayouts");

            var itemToDelete = section.Tools.FirstOrDefault<ToolboxItem>(e => e.Name == gridName);
            section.Tools.Remove(itemToDelete);

            configurationManager.SaveSection(toolboxesConfig);
        }
    }
}
