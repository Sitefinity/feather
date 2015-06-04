using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules.Builder;
using Telerik.Sitefinity.DynamicModules.Builder.Model;
using Telerik.Sitefinity.DynamicModules.Model;
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
            var itemTypeFullName = item.GetType().FullName;

            return CommentsHelpers.CommentsCount(helper, navigateUrl, threadKey, itemTypeFullName, allowComments);
        }

        /// <summary>
        /// Returns the CommentsCount widget if exist, else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="navigateUrl">The navigate URL.</param>
        /// <param name="threadKey">The thread key.</param>
        /// <param name="threadType">Type of the thread.</param>
        /// <param name="allowComments">if not null this value will override the configuration for allowing comments.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public static MvcHtmlString CommentsCount(this HtmlHelper helper, string navigateUrl, string threadKey, string threadType, bool? allowComments = null)
        {
            if (SystemManager.GetModule("Comments") == null || string.IsNullOrEmpty(threadKey))
                return MvcHtmlString.Empty;

            if (string.IsNullOrEmpty(navigateUrl))
            {
                navigateUrl = "#comments-" + threadKey;
            }

            var controllerName = threadKey.EndsWith(ReviewsSuffix, StringComparison.Ordinal) ? CommentsHelpers.ReviewsControllerName : CommentsHelpers.CommentsControllerName;

            MvcHtmlString result;
            try
            {
                result = helper.Action(CommentsHelpers.CountActionName, controllerName, new { NavigateUrl = navigateUrl, ThreadKey = threadKey, ThreadType = threadType, AllowComments = allowComments });
            }
            catch (HttpException)
            {
                result = MvcHtmlString.Empty;
            }
            catch (NullReferenceException)
            {
                //// Telerik.Sitefinity.Mvc.SitefinityMvcRoute GetOrderedParameters() on line 116 controllerType.GetMethods() throws null reference exception (controllerType is null).
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        /// <summary>
        /// Renders comments list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The item.</param>
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item)
        {
            var title = CommentsHelpers.GetTitle(item);
            return CommentsHelpers.GetCommentsList(helper, item, title);
        }

        /// <summary>
        /// Renders comments list.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="item">The item.</param>
        /// <param name="title">The title.</param>
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item, string title)
        {
            return CommentsHelpers.GetCommentsList(helper, item, title);
        }

        private static MvcHtmlString GetCommentsList(HtmlHelper helper, IDataItem item, string title)
        {
            if (SystemManager.GetModule("Comments") == null || item == null)
            {
                return MvcHtmlString.Empty;
            }

            var itemTypeFullName = item.GetType().FullName;
            var itemProviderName = item.GetProviderName();

            var itemThreadKey = ControlUtilities.GetLocalizedKey(item.Id, null, CommentsBehaviorUtilities.GetLocalizedKeySuffix(itemTypeFullName));
            var itemGroupKey = ControlUtilities.GetUniqueProviderKey(GetDataSourceName(item), itemProviderName);

            var routeDictionary = new System.Web.Routing.RouteValueDictionary()
            {
                { "AllowComments", GetAllowComments(item) },
                { "ThreadKey", itemThreadKey },
                { "ThreadTitle", title },
                { "ThreadType", itemTypeFullName },
                { "GroupKey", itemGroupKey },
                { "DataSource", itemProviderName }
            };

            var controllerName = itemThreadKey.EndsWith(ReviewsSuffix, StringComparison.Ordinal) ? CommentsHelpers.ReviewsControllerName : CommentsHelpers.CommentsControllerName;

            MvcHtmlString result;
            try
            {
                result = helper.Action(CommentsHelpers.IndexActionName, controllerName, routeDictionary);
            }
            catch (HttpException)
            {
                result = MvcHtmlString.Empty;
            }
            catch (NullReferenceException)
            {
                //// Telerik.Sitefinity.Mvc.SitefinityMvcRoute GetOrderedParameters() on line 116 controllerType.GetMethods() throws null reference exception (controllerType is null).
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        private static string GetDataSourceName(IDataItem item)
        {
            string dataSourceName = null;

            if (item != null && item is DynamicContent)
            {
                var moduleProvider = ModuleBuilderManager.GetManager().Provider;
                var itemType = item.GetType().FullName;

                var dynamicContentType = moduleProvider.GetDynamicModules()
                    .Where(m => m.Status == DynamicModuleStatus.Active)
                    .Join(moduleProvider.GetDynamicModuleTypes().Where(t => string.Concat(t.TypeNamespace, ".", t.TypeName) == itemType), m => m.Id, t => t.ParentModuleId, (m, t) => t)
                    .FirstOrDefault();

                if (dynamicContentType != null)
                {
                    dataSourceName = dynamicContentType.ModuleName;
                }
            }
            else if (item != null)
            {
                Type managerType;
                if (ManagerBase.TryGetMappedManagerType(item.GetType(), out managerType))
                {
                    dataSourceName = managerType.FullName;
                }
            }

            return dataSourceName;
        }

        private static string GetTitle(IDataItem item)
        {
            string title = null;

            if (item != null)
            {
                var hasTitleItem = item as IHasTitle;
                if (hasTitleItem != null)
                {
                    title = hasTitleItem.GetTitle();
                }
            }

            return title;
        }

        private static bool? GetAllowComments(IDataItem item)
        {
            bool? allowComments = null;

            if (item != null)
            {
                var contentItem = item as Content;
                if (contentItem != null)
                {
                    allowComments = contentItem.AllowComments;
                }
            }

            return allowComments;
        }

        private const string ReviewsSuffix = "_review";

        private const string CommentsControllerName = "Comments";
        private const string ReviewsControllerName = "Reviews";
        private const string IndexActionName = "Index";
        private const string CountActionName = "Count";
    }
}