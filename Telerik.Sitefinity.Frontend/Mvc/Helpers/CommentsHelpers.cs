using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Telerik.Sitefinity.Frontend.Mvc.Models;
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
        /// Commentses the list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemManagerName">Name of the item manager.</param>
        /// <param name="itemTitle">The item title.</param>
        /// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item, string itemManagerName, string itemTitle, bool allowComments = true)
        {
            if (item == null)
            {
                return MvcHtmlString.Empty;
            }

            return helper.CommentsList(item.Id, item.GetType().FullName, item.GetProviderName(), itemManagerName, itemTitle, allowComments);
        }

        /// <summary>
        /// Commentses the list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="itemProviderName">Name of the item provider.</param>
        /// <param name="itemManagerName">Name of the item manager.</param>
        /// <param name="itemTitle">The item title.</param>
        /// <param name="allowComments">if set to <c>true</c> [allow comments].</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString CommentsList(this HtmlHelper helper, Guid itemId, string itemType, string itemProviderName, string itemManagerName, string itemTitle, bool allowComments = true)
        {
            var itemThreadKey = ControlUtilities.GetLocalizedKey(itemId, null, CommentsBehaviorUtilities.GetLocalizedKeySuffix(itemType));
            var itemGroupKey = ControlUtilities.GetUniqueProviderKey(itemManagerName, itemProviderName);

            var routeDictionary = new System.Web.Routing.RouteValueDictionary()
            {
                { "AllowComments", allowComments },
                { "ThreadKey", itemThreadKey },
                { "ThreadTitle", itemTitle },
                { "ThreadType", itemType },
                { "GroupKey", itemGroupKey },
                { "DataSource", itemProviderName }
            };
            
            MvcHtmlString result;
            try
            {
                result = helper.Action(ActionName, ControllerName, routeDictionary);
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