using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.LocalizationResources;

//In order to be properly resolved the controller must be in this namespace.
namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This class represents a dummy controller with Localization attribute.
    /// </summary>
    [Localization(typeof(DummyControllerResoruces))]
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
