using System.Collections.Generic;
using System.Web.Mvc;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helepr method for HTML markup generation.
    /// </summary>
    public static class MarkupHelpers
    {
        /// <summary>
        /// Gets a unique element id with the given prefix.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper.</param>
        /// <param name="prefix">The prefix of the identifier.</param>
        /// <returns>Unique Id for an HTML element.</returns>
        public static MvcHtmlString UniqueId(this HtmlHelper htmlHelper, string prefix)
        {
            if (!htmlHelper.ViewData.ContainsKey(MarkupHelpers.ElementIdsKey))
            {
                htmlHelper.ViewData.Add(MarkupHelpers.ElementIdsKey, new Dictionary<string, string>());
            }

            var keysDictionary = (IDictionary<string, string>)htmlHelper.ViewData[MarkupHelpers.ElementIdsKey];
            if (!keysDictionary.ContainsKey(prefix))
            {
                if (!SystemManager.CurrentHttpContext.Items.Contains(MarkupHelpers.IdCountersKey))
                {
                    SystemManager.CurrentHttpContext.Items[MarkupHelpers.IdCountersKey] = new Dictionary<string, int>();
                }

                var counters = (IDictionary<string, int>)SystemManager.CurrentHttpContext.Items[MarkupHelpers.IdCountersKey];
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

        private const string ElementIdsKey = "sf-element-ids";
        private const string IdCountersKey = "sf-element-id-counters";
    }
}
