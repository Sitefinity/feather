using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Comments;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Holds helpers related to Comments widget
    /// </summary>
    public static class CommentsHelpers
    {
        /// <summary>
        /// Returns the comments list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The item.</param>
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item)
        {
            string itemType = item.GetType().FullName;
            var itemThreadKey = ControlUtilities.GetLocalizedKey(item.Id, null, CommentsBehaviorUtilities.GetLocalizedKeySuffix(itemType));
            
            MvcHtmlString result;
            try
            {
                result = helper.Action(ActionName, ControllerName, new { threadKey = itemThreadKey, threadType = itemType });
            }
            catch (HttpException)
            {
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        /// <summary>
        /// Returns the CommentsCount widget if exist, else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <param name="threadKey">The thread key.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static MvcHtmlString CommentsCount(this HtmlHelper helper, string navigateUrl, string threadKey)
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

        private const string ActionName = "Index";
        private const string ControllerName = "Comments";
    }
}