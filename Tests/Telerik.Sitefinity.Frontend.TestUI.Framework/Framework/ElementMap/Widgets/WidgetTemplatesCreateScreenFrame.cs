using ArtOfTest.WebAii.Core;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    public class WidgetTemplatesCreateScreenFrame : WidgetTemplatesCreateEditBaseScreen
    {
        public WidgetTemplatesCreateScreenFrame(Find find)
            : base(find)
        {
        }

        public override FrameInfo GetHostedFrameInfo()
        {
            return new FrameInfo() { Name = "create" };
        }
    }
}
