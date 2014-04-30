using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// This class is used when a fake ToolboxesConfig is needed
    /// </summary>
    public class DummyToolboxesConfig : ToolboxesConfig
    {
         protected override void OnPropertiesInitialized()
        {
            // do nothing
        }
    }
}
