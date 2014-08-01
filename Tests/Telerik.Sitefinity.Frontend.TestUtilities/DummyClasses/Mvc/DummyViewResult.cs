using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc
{
    /// <summary>
    /// This class inherits <see cref="System.Web.Mvc.ViewResultBase"/> for test purposes.
    /// </summary>
    public class DummyViewResult : ViewResultBase
    {
        /// <summary>
        /// Returns the <see cref="T:System.Web.Mvc.ViewEngineResult" /> object that is used to render the view.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The view engine.
        /// </returns>
        protected override ViewEngineResult FindView(ControllerContext context)
        {
            return null;
        }
    }
}
