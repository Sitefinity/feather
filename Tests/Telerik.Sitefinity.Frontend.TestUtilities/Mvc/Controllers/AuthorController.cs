using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers
{
    [ControllerToolboxItem(Name = "Author", Title = "Author", SectionName = "MvcWidgets")]
    public class AuthorController : Controller
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        [Category("String Properties")]
        public string Message { get; set; }

        /// <summary>
        /// This is the default Action.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "page")]
        public ActionResult Index(int? page)
        {
            return this.Content("Default");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "author")]
        public ActionResult Details(DynamicContent author)
        {
            return this.Content("Details");
        }
    }
}