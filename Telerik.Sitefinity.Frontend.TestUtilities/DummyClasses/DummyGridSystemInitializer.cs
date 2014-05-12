using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.GridSystem;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
     /// <summary>
    /// Inheritor of GridSystemInitializer used to test its protected methods.
    /// </summary>
    public class DummyGridSystemInitializer : GridSystemInitializer
    {
        /// <summary>
        /// Creates a dummy tool box section.
        /// </summary>
        /// <param name="toolboxConfig">The toolbox configuration.</param>
        /// <returns></returns>
        public Modules.Pages.Configuration.ToolboxSection PublicCreateToolBoxSection(Modules.Pages.Configuration.ToolboxesConfig toolboxConfig)
        {
            return this.GetOrCreateToolBoxSection(toolboxConfig);
        }

        /// <summary>
        /// Adds the dummy layout control.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="data">The data.</param>
        public void PublicAddLayoutControl(Configuration.ConfigElementList<Modules.Pages.Configuration.ToolboxItem> parent, GridControlData data)
        {
            this.AddLayoutControl(parent, data);
        }

        /// <summary>
        /// Creates the dummy layout controls data.
        /// </summary>
        /// <param name="baseTemplatePath">The base template path.</param>
        /// <returns></returns>
        public List<GridControlData> PublicCreateLayoutControlsData(string baseTemplatePath)
        {
            return this.CreateLayoutControlsData(baseTemplatePath);
        }
    }
}
