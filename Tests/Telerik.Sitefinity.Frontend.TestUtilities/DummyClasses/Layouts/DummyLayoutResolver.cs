using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts
{
    /// <summary>
    /// This class fakes the <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts.LayoutResolver"/> class members in fake context. Used for test purposes only.
    /// </summary>
    internal class DummyLayoutResolver : LayoutResolver
    {
        protected override LayoutVirtualPathBuilder CreateLayoutVirtualPathBuilder()
        {
            return new DummyLayoutVirtualPathBuilder();
        }
    }
}
