using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class SocialShareHelpers
    {
        /// <summary>
        /// Socials the share options. Redirect to the SocialShare control if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        [Obsolete("Social sharing module has been removed. This helper will no longer work.")]
        [SuppressMessage("Microsoft.Design", "CA1801:ReviewUnusedParameters")]
        public static MvcHtmlString SocialShareOptions(this HtmlHelper helper)
        {
            return null;
        }

        /// <summary>
        /// Socials the share options. Redirect to the SocialShare control if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="dataItem">The data item which we will be sharing</param>
        [Obsolete("Social sharing module has been removed. This helper will no longer work.")]
        [SuppressMessage("Microsoft.Design", "CA1801:ReviewUnusedParameters")]
        public static MvcHtmlString SocialShareOptions(this HtmlHelper helper, Telerik.Sitefinity.Model.IHasTitle dataItem)
        {
            return null;
        }
    }
}