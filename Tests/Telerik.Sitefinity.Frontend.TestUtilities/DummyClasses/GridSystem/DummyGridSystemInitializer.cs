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
    /// This class is used to fake the functionality of <see cref="Telerik.Sitefinity.Frontend.GridSystem.GridWidgetRegistrator"/> class to expose its protected methods for testing purposes.
    /// </summary>
    internal class DummyGridWidgetRegistrator : GridWidgetRegistrator
    {
        /// <inheritdoc />
        public ToolboxSection PublicCreateToolBoxSection(ToolboxesConfig toolboxConfig, string sectionName, string sectionTitle)
        {
            return this.GetOrCreateToolBoxSection(toolboxConfig, sectionName, sectionTitle);
        }

        /// <inheritdoc />
        public void PublicAddGridControl(ConfigElementList<ToolboxItem> parent, GridControlData data, string oldFileName = "")
        {
            this.AddOrRenameGridControl(parent, data, oldFileName);
        }

        /// <inheritdoc />
        public GridControlData PublicCreateGridControlsData(string fileName)
        {
            return this.CreateGridControlsData(fileName);
        }
    }
}
