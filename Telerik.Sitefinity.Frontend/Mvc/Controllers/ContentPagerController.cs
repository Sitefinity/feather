using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Web.UI;

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
        [OutputCache(Duration = 1)]
        public PartialViewResult Index(int currentPage, int totalPagesCount, string redirectUrlTemplate)
        {
            var model = new PagerViewModel(currentPage, totalPagesCount, redirectUrlTemplate);

            // Build the pager
            int num = 1;
            if (model.CurrentPage > model.DisplayCount)
            {
                if (model.CurrentPage <= 0)
                    model.CurrentPage = 1;

                num = ((int)Math.Floor(Convert.ToDecimal(model.CurrentPage - 1) / model.DisplayCount) * model.DisplayCount) + 1;
            }

            int num1 = Math.Min(model.TotalPagesCount, (num + model.DisplayCount) - 1);

            // Check to see if we need a Previous Button Node
            if (num > model.DisplayCount)
            {
                model.PreviousNode = new Pager.PagerNumericItem(Math.Max(num - 1, 1));
            }
            else
            {
                model.PreviousNode = null;
            }

            int num2 = num;
            int num3 = num1;

            while (num2 <= num3)
            {
                string str = string.Format(model.RedirectUrlTemplate, num2);
                Pager.PagerNumericItem pagerNumericItem1 = new Pager.PagerNumericItem(num2, str);
                model.PagerNodes.Add(pagerNumericItem1);
                num2++;
            }

            // Check to see if we need a Next Button Node
            if (num1 < model.TotalPagesCount)
            {
                model.NextNode = new Pager.PagerNumericItem(num1 + 1);
            }
            else
            {
                model.NextNode = null;
            }

            return this.PartialView("Pager", model);
        }
    }
}
