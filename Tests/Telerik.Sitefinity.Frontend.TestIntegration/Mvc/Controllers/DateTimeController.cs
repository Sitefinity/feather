using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Mvc.Controllers
{
    /// <summary>
    /// This is dummy date time controller.
    /// </summary>
    public class DateTimeController : Controller
    {
        public static int Count { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        public ActionResult Index()
        {
            DateTimeController.Count++;

            return this.Content(DateTime.Now.ToString());
        }
    }
}