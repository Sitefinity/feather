using System.Web.Mvc;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This class reperesents a controller that should not be rendered in indexing mode. Used for testing purposes.
    /// </summary>
    [IndexRenderMode(IndexRenderModes.NoOutput)]
    public class DummyNoOutputInIndexingController : Controller
    {
        /// <summary>
        /// The default action of this controller.
        /// </summary>
        /// <returns>Static content.</returns>
        public ActionResult Index()
        {
            return this.Content(DummyNoOutputInIndexingController.Output);
        }

        /// <summary>
        /// The output of the Index action.
        /// </summary>
        public const string Output = "I was rendered.";
    }
}
