namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Sitefinity.Security.Sanitizers;
    using Telerik.Sitefinity.Services;
    using Telerik.Sitefinity.Web.UI;

    /// <summary>
    /// Helper method for HTML markup generation.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Gets a unique element id with the given prefix.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="prefix">The prefix of the identifier.</param>
        /// <returns>Unique Id for an HTML element.</returns>
        public static MvcHtmlString UniqueId(this HtmlHelper htmlHelper, string prefix)
        {
            if (!htmlHelper.ViewData.ContainsKey(HtmlHelperExtensions.ElementIdsKey))
            {
                htmlHelper.ViewData.Add(HtmlHelperExtensions.ElementIdsKey, new Dictionary<string, string>());
            }

            var keysDictionary = (IDictionary<string, string>)htmlHelper.ViewData[HtmlHelperExtensions.ElementIdsKey];
            if (!keysDictionary.ContainsKey(prefix))
            {
                if (!SystemManager.CurrentHttpContext.Items.Contains(HtmlHelperExtensions.IdCountersKey))
                {
                    SystemManager.CurrentHttpContext.Items[HtmlHelperExtensions.IdCountersKey] = new Dictionary<string, int>();
                }

                var counters = (IDictionary<string, int>)SystemManager.CurrentHttpContext.Items[HtmlHelperExtensions.IdCountersKey];
                if (!counters.ContainsKey(prefix))
                {
                    counters[prefix] = 0;
                }

                var counter = counters[prefix] + 1;
                counters[prefix] = counter;
                keysDictionary[prefix] = prefix + "-" + counter.ToString();
            }

            var uniqueId = keysDictionary[prefix];
            return MvcHtmlString.Create(uniqueId);
        }

        /// <summary>
        /// Sanitizes html string using the registered <see cref="IHtmlSanitizer"/>. This method should be used when you want to handle text that is supposed to be HTML, but comes from untrusted source.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper which this method is extending.</param>
        /// <param name="html">The HTML string to be sanitized.</param>
        /// <returns></returns>
        public static MvcHtmlString HtmlSanitize(this HtmlHelper htmlHelper, string html)
        {
            var sanitizedHtml = ControlUtilities.Sanitize(html);

            return MvcHtmlString.Create(sanitizedHtml);
        }

        /// <summary>
        /// Sanitizes url string using the registered <see cref="IHtmlSanitizer"/>. It will also attribute encode the string.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper which this method is extending.</param>
        /// <param name="url">The url string to be sanitized.</param>
        /// <returns></returns>
        public static MvcHtmlString UrlSanitize(this HtmlHelper htmlHelper, string url)
        {
            var sanitizedUrl = ControlUtilities.SanitizeUrl(url);

            return MvcHtmlString.Create(sanitizedUrl);
        }

        private const string ElementIdsKey = "sf-element-ids";
        private const string IdCountersKey = "sf-element-id-counters";
    }
}