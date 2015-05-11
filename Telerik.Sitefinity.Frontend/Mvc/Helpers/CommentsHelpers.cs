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

        private const string ActionName = "Index";
        private const string ControllerName = "Comments";
    }
}