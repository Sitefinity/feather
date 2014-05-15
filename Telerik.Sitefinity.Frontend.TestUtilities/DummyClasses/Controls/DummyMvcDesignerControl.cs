using System.Web.UI;
using Telerik.Sitefinity.Frontend.Designers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls
{
    /// <summary>
    /// This class represents dummy <see cref="System.Web.UI.Control"/> with MVC designer.
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
