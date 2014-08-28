using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.LocalizationResources;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This class represents a dummy controller with Localization attribute.
    /// </summary>
    /// <remarks>
    /// In order to be properly resolved the controller should be 
    /// in the <see cref="Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers"/> namespace.
    /// </remarks>
    [Localization(typeof(DummyControllerResources))]
    public class DummyController : Controller
    {
        /// <summary>
        /// A dummy action.
        /// </summary>
        /// <returns></returns>
        public ViewResult DummyAction()
        { 
            return new ViewResult();
        }
    }
}
