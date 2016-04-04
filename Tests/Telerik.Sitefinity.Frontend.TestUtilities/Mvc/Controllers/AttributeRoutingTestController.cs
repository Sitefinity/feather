using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    [RoutePrefix(AttributeRoutingTestController.RoutePrefix)]
    public class AttributeRoutingTestController : Controller
    {
        public ActionResult Index()
        {
            return this.Content("This is the Index.");
        }

        [RelativeRoute(AttributeRoutingTestController.RelativeRoute)]
        public ActionResult Test()
        {
            return this.Content(AttributeRoutingTestController.Content);
        }

        public const string RoutePrefix = "testme";
        public const string RelativeRoute = "route-me";
        public const string Content = "Routed successfully";
    }
}
