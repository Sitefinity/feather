using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Telerik.Sitefinity.Frontend.Mvc.Models;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Comments;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Holds helpers related to Comments widget
    /// </summary>
    public static class CommentsHelpers
    {
        /// <summary>
        /// Returns the Comments list if exist, else render error message
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The item.</param>
        /// <param name="itemManagerName">Name of the item manager.</param>
        /// <param name="itemTitle">The item title.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item, string itemManagerName, string itemTitle)
        {
            if (item == null)
            {
                return MvcHtmlString.Empty;
            }

            var contentItem = item as Content;
            bool? allowComments = contentItem != null ? contentItem.AllowComments : null;
            return helper.CommentsList(item.Id, item.GetType().FullName, item.GetProviderName(), itemManagerName, itemTitle, allowComments);
        }

        /// <summary>
        /// Returns the Comments list if exist, else render error message
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="itemId">The item identifier.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="itemProviderName">Name of the item provider.</param>
        /// <param name="itemManagerName">Name of the item manager.</param>
        /// <param name="itemTitle">The item title.</param>
        /// <param name="allowComments">if not null this value will override the configuration for allowing comments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        public static MvcHtmlString CommentsList(this HtmlHelper helper, Guid itemId, string itemType, string itemProviderName, string itemManagerName, string itemTitle, bool? allowComments = null)
        {
            if (SystemManager.GetModule("Comments") == null)
                return MvcHtmlString.Empty;

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
                result = helper.Action(CommentsHelpers.IndexActionName, CommentsHelpers.ControllerName, routeDictionary);
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
        /// <param name="helper">The helper.</param>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <param name="item">The commented item.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static MvcHtmlString CommentsCount(this HtmlHelper helper, string navigateUrl, IDataItem item)
        {
            if (item == null)
                return MvcHtmlString.Empty;

            var contentItem = item as Content;
            bool? allowComments = contentItem != null ? contentItem.AllowComments : null;
            var threadKey = ControlUtilities.GetLocalizedKey(item.Id, null, CommentsBehaviorUtilities.GetLocalizedKeySuffix(item.GetType().FullName));
            return CommentsHelpers.CommentsCount(helper, navigateUrl, threadKey, allowComments);
        }

        /// <summary>
        /// Returns the CommentsCount widget if exist, else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <param name="threadKey">The thread key.</param>
        /// <param name="allowComments">if not null this value will override the configuration for allowing comments.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static MvcHtmlString CommentsCount(this HtmlHelper helper, string navigateUrl, string threadKey, bool? allowComments = null)
        {
            if (SystemManager.GetModule("Comments") == null)
                return MvcHtmlString.Empty;

            MvcHtmlString result;
            try
            {
                result = helper.Action(CommentsHelpers.CountActionName, CommentsHelpers.ControllerName, new { NavigateUrl = navigateUrl, ThreadKey = threadKey, AllowComments = allowComments });
            }
            catch (HttpException)
            {
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        private const string IndexActionName = "Index";
        private const string CountActionName = "Count";
        private const string ControllerName = "Comments";
    }
}