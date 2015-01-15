using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    /// <summary>
    /// This class represents a controller that has one action and it always throws exception. Used for testing purposes.
    /// </summary>
    public class DummyFailingController : Controller
    {
        /// <summary>
        /// The default action of the controller.
        /// </summary>
        /// <returns>Returns nothing. Always throws exception.</returns>
        /// <exception cref="System.Exception">Always fails.</exception>
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        public ViewResult Index()
        {
            throw new Exception("Always fails.");
        }
    }
}
