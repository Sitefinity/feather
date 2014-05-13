using System.Web.UI;
using Telerik.Sitefinity.Frontend.Designers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// A dummy control with MVC designer.
    /// </summary>
    [DesignerUrl(DummyMvcDesignerControl.CustomDesignerUrl)]
    public class DummyMvcDesignerControl : Control
    {
        /// <summary>
        /// The custom designer URL
        /// </summary>
        public const string CustomDesignerUrl = "~/Test/CustomDesigner/";
    }
}
