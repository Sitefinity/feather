using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class represents controller for pager widget.
    /// </summary>
    [ControllerMetadataAttribute(IsTemplatableControl = false)]
    public class ContentPagerController : Controller
    {
        /// <summary>
        /// Returns view with pager.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="totalPagesCount">The total pages count.</param>
        /// <param name="redirectUrlTemplate">The template of the URL used for redirecting.</param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public PartialViewResult Index(int currentPage, int totalPagesCount, string redirectUrlTemplate)
        {
            var model = new PagerViewModel(currentPage, totalPagesCount, redirectUrlTemplate);

            return this.PartialView("Pager", model);
        }
    }
}
