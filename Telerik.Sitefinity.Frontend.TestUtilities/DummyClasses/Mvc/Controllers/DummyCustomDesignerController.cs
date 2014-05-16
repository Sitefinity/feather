using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Designers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    /// <summary>
    /// This class represents a dummy controller with custom designer URL.
    /// </summary>
    [DesignerUrl(DummyCustomDesignerController.CustomDesignerUrl)]
    public class DummyCustomDesignerController : Controller
    {
        /// <summary>
        /// The custom designer URL
        /// </summary>
        public const string CustomDesignerUrl = "~/Test/CustomDesigner/";
    }
}
