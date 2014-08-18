using Telerik.Sitefinity.Modules.Pages.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs
{
    /// <summary>
    /// This class fakes <see cref="Telerik.Sitefinity.Modules.Pages.Configuration.ToolboxesConfig"/> for test purposes.
    /// </summary>
    public class DummyToolboxesConfig : ToolboxesConfig
    {
        /// <inheritdoc />
        protected override void OnPropertiesInitialized()
        {
            // do nothing
        }
    }
}
