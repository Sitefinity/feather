using System;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for work with Sitefinity layout templates.
    /// </summary>
    public static class LayoutsHelpers
    {
        /// <summary>
        /// HTML helper which adds the required content placeholder.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="containerName">Name of the container.</param>
        /// <returns></returns>
        public static MvcHtmlString SfPlaceHolder(this HtmlHelper helper, string containerName = "Body")
        {
            var htmlString = string.Format("<div class='sfPublicWrapper' id='PublicWrapper' runat='server'><asp:contentplaceholder id='{0}' runat='server' /></div>", containerName);

            return new MvcHtmlString(htmlString);
        }
    }
}
