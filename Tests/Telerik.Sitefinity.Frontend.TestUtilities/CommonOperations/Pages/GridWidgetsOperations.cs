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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public void RemoveGridControlFromToolboxesConfig(string gridName, string sectionName = "BootstrapGrids")
        {
            ConfigManager configurationManager = ConfigManager.GetManager();
            var toolboxesConfig = configurationManager.GetSection<ToolboxesConfig>();
            var pageControls = toolboxesConfig.Toolboxes["PageLayouts"];

            var section = pageControls.Sections.FirstOrDefault<ToolboxSection>(s => s.Name == sectionName);

            var itemToDelete = section.Tools.FirstOrDefault<ToolboxItem>(e => e.Name == gridName);
            section.Tools.Remove(itemToDelete);

            configurationManager.SaveSection(toolboxesConfig);
        }
    }
}
