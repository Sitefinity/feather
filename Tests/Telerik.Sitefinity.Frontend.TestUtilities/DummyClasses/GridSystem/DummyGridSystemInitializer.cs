using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.GridSystem
{
    /// <summary>
    /// This class is used to fake the functionality of <see cref="Telerik.Sitefinity.Frontend.GridSystem.GridSystemInitializer"/> class to expose its protected methods for testing purposes.
    /// </summary>
    public class DummyGridSystemInitializer : GridSystemInitializer
    {
        /// <inheritdoc />
        public ToolboxSection PublicCreateToolBoxSection(ToolboxesConfig toolboxConfig)
        {
            return this.GetOrCreateToolBoxSection(toolboxConfig);
        }

        /// <inheritdoc />
        public void PublicAddLayoutControl(ConfigElementList<ToolboxItem> parent, GridControlData data)
        {
            this.AddLayoutControl(parent, data);
        }

        /// <inheritdoc />
        public ICollection<GridControlData> PublicCreateLayoutControlsData(string baseTemplatePath)
        {
            return this.CreateLayoutControlsData(baseTemplatePath);
        }
    }
}
