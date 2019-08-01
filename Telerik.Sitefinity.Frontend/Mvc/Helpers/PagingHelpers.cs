using System;
using System.Web;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for widget paging.
    /// </summary>
    public static class PagingHelpers
    {
        /// <summary>
        /// Generate url template that suports paging.
        /// </summary>
        /// <param name="controller">Target controller.</param>
        /// <param name="pageUrl">Base url for paging.</param>
        /// <param name="urlKeyPrefix">Query parameter sufix.</param>
        /// <returns>Url suffix.</returns>
        public static string GeneratePagingTemplate(this ContentBaseController controller, string pageUrl, string urlKeyPrefix)
        {
            if (urlKeyPrefix.IsNullOrWhitespace())
            {
                return string.Concat(pageUrl, UrlHelpers.GetRedirectPagingUrl(), SystemManager.CurrentHttpContext.Request.QueryString.ToQueryString(true));
            }

            var key = string.Format(KeyFormat, urlKeyPrefix);
            var queryParams = HttpUtility.ParseQueryString(SystemManager.CurrentHttpContext.Request.Url.Query);
            queryParams[key] = "{0}";

            var queryParamsRaw = HttpUtility.UrlDecode(queryParams.ToQueryString(true));
            return string.Concat(SystemManager.CurrentHttpContext.Request.Url.LocalPath, queryParamsRaw);
        }

        /// <summary>
        /// Check query params and update page if applicable.
        /// </summary>
        /// <param name="controller">Target controller.</param>
        /// <param name="page">The page.</param>
        /// <param name="urlKeyPrefix">Prefix of page query parameter.</param>
        public static void UpdatePageFromQuery(this ContentBaseController controller, ref int? page, string urlKeyPrefix)
        {
            if (urlKeyPrefix.IsNullOrWhitespace()) return;

            var key = string.Format(KeyFormat, urlKeyPrefix);
            var pageString = SystemManager.CurrentHttpContext.Request.QueryStringGet(key);

            int queryPage;
            if (!int.TryParse(pageString, out queryPage))
            {
                queryPage = 1;
            }
            page = queryPage;
        }

        /// <summary>
        /// Extract a valid page number.
        /// </summary>
        /// <param name="controller">Target controller.</param>
        /// <param name="page">Nullable page number.</param>
        /// <returns>One (1) if page is null or less than one, otherwise page value.</returns>
        public static int ExtractValidPage(this ContentBaseController controller, int? page)
        {
            if (!page.HasValue || page.Value < 1) return 1;

            return page.Value;
        }

        private const string KeyFormat = "{0}page";
    }
}
