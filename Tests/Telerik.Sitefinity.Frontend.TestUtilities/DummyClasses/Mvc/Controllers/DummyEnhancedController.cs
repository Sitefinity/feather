using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    /// <summary>
    /// This class represents dummy controller with EnchanceViewEngines attribute.
    /// </summary>
    [EnhanceViewEngines(VirtualPath = DummyEnhancedController.CustomControllerPath)]
    public class DummyEnhancedController : Controller
    {
        /// <summary>
        /// The custom controller path.
        /// </summary>
        public const string CustomControllerPath = "MyCustomPath/MoreDepth/";
    }
}
