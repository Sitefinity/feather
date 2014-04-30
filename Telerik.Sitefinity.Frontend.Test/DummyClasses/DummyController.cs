using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Controllers
{
    /// <summary>
    /// A dummy controller with no custom attributes.
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
