using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This class represents a test MVC widget which has an action that returns the Default View.
    /// </summary>
    public class MvcTestController : Controller
    {
        /// <summary>
        /// This is the default Action.
        /// </summary>
        /// <returns>The Default View.</returns>
        public ActionResult Index()
        {
            return this.View("Default");
        }
    }
}
