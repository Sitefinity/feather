using System;
using System.Reflection;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class represents controller for pager widget.
    /// </summary>
    [ControllerMetadata(IsTemplatableControl = false)]
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
            int startIndex = 1;
            if (model.CurrentPage > model.DisplayCount)
            {
                if (model.CurrentPage <= 0)
                    model.CurrentPage = 1;

                startIndex = ((int)Math.Floor((double)(model.CurrentPage - 1) / model.DisplayCount) * model.DisplayCount) + 1;
            }

            int endIndex = Math.Min(model.TotalPagesCount, (startIndex + model.DisplayCount) - 1);

            // Check to see if we need a Previous Button Node
            if (startIndex > model.DisplayCount)
            {
                model.PreviousNode = new Pager.PagerNumericItem(Math.Max(startIndex - 1, 1));
            }
            else
            {
                model.PreviousNode = null;
            }

            for (int i = startIndex; i <= endIndex; i++)
            {
                var text = string.Format(model.RedirectUrlTemplate, i);
                Pager.PagerNumericItem pagerNumericItem = new Pager.PagerNumericItem(i, text);
                model.PagerNodes.Add(pagerNumericItem);
            }

            // Check to see if we need a Next Button Node
            if (endIndex < model.TotalPagesCount)
            {
                model.NextNode = new Pager.PagerNumericItem(endIndex + 1);
            }
            else
            {
                model.NextNode = null;
            }

            this.TryStorePaginationUrls(model);

            return this.PartialView("Pager", model);
        }

        internal static object GetPaginationUrls(string nextUrl, string previousUrl)
        {
            var paginationUrlsType = Type.GetType("Telerik.Sitefinity.ContentLocations.PaginationUrls, Telerik.Sitefinity");
            var instance = Activator.CreateInstance(paginationUrlsType, nonPublic: true);
            paginationUrlsType.GetProperty("PreviousUrl").SetValue(instance, previousUrl);
            paginationUrlsType.GetProperty("NextUrl").SetValue(instance, nextUrl);

            return instance;
        }

        internal static MethodInfo GetTryStorePaginationUrlsMethod()
        {
            var canonicalUrlExtensions = Type.GetType("Telerik.Sitefinity.ContentLocations.CanonicalUrlPageExtensions, Telerik.Sitefinity");
            return canonicalUrlExtensions.GetMethod("TryStorePaginationUrls", BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static string PageNodeUrl(Pager.PagerNumericItem node, string template)
        {
            if (node == null)
                return null;
            else
                return RouteHelper.ResolveUrl(string.Format(template, node.PageNumber), UrlResolveOptions.Absolute);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "paginationUrls")]
        private void TryStorePaginationUrls(PagerViewModel model)
        {
            string nextUrl;
            if (model.CurrentPage > 0 && model.CurrentPage < model.PagerNodes.Count)
                nextUrl = ContentPagerController.PageNodeUrl(model.PagerNodes[model.CurrentPage], model.RedirectUrlTemplate);
            else
                nextUrl = null;

            string previousUrl;
            if (model.CurrentPage > 1)
                previousUrl = ContentPagerController.PageNodeUrl(model.PagerNodes[model.CurrentPage - 2], model.RedirectUrlTemplate);
            else
                previousUrl = null;

            var paginationUrls = ContentPagerController.GetPaginationUrls(nextUrl, previousUrl);

            var page = this.HttpContext.Handler.GetPageHandler();
            ContentPagerController.GetTryStorePaginationUrlsMethod().Invoke(null, new object[] { page, paginationUrls });
        }
    }
}
