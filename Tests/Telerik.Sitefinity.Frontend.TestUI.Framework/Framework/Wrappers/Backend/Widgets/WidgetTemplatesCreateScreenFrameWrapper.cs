

using Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets;
namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Frame wrapper for CreateTemplateScreen screen
    /// </summary>
    public class WidgetTemplatesCreateScreenFrameWrapper : WidgetTemplatesCreateEditScreenWrapper<WidgetTemplatesCreateScreenFrame>
    {
        /// <summary>
        /// Gets the element map.
        /// </summary>
        /// <value>The element map.</value>
        protected override WidgetTemplatesCreateScreenFrame ActiveWindowEM
        {
            get
            {
                return new WidgetTemplatesCreateScreenFrame(ActiveBrowser.Find);
            }
        }
    }
}
