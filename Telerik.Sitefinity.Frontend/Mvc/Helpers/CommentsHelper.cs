using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class CommentsHelper
    {
        /// <summary>
        /// Socials the share options. Redirect to the CommentsCount widget if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="navigateUrl">The navigation URL.</param>
        /// <param name="threadKey">The thread key.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static System.Web.Mvc.MvcHtmlString CommentsCount(this HtmlHelper helper, string navigateUrl, string threadKey)
        {
            System.Web.Mvc.MvcHtmlString result;
            try
            {
                result = helper.Action("Count", ControllerName, new { navigateUrl = navigateUrl, threadKey = threadKey });
            }
            catch (HttpException)
            {
                result = new System.Web.Mvc.MvcHtmlString("The CommentsCount widget could not be found.");
            }

            return result;
        }

        private const string ControllerName = "Comments";
    }
}