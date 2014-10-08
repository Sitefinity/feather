using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts
{
    /// <summary>
    /// This class fakes the <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts.LayoutVirtualPathBuilder"/> class members in fake context. Used for test purposes only.
    /// </summary>
    internal class DummyLayoutVirtualPathBuilder : LayoutVirtualPathBuilder
    {
        /// <inheritdoc />
        protected override string AddVariablesToPath(string layoutVirtualPath)
        {
            return layoutVirtualPath;
        }
    }
}
