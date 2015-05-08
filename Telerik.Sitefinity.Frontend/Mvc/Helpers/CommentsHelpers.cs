using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.Comments;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class CommentsHelpers
    {
        public static MvcHtmlString CommentsList(this HtmlHelper helper, IDataItem item)
        {
            MvcHtmlString result;
            var itemThreadKey = ControlUtilities.GetLocalizedKey(item.Id, null, CommentsBehaviorUtilities.GetLocalizedKeySuffix(item.GetType().FullName));

            try
            {
                result = helper.Action(ActionName, ControllerName, new { threadKey = itemThreadKey });
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