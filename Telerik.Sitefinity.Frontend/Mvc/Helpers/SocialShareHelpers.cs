using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class SocialShareHelpers
    {
        /// <summary>
        /// Socials the share options. Redirect to the SocialShare control if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        public static System.Web.Mvc.MvcHtmlString SocialShareOptions(this HtmlHelper helper)
        {
            System.Web.Mvc.MvcHtmlString result;
            try
            {
                result = helper.Action(ActionName, ControllerName);
            }
            catch (HttpException)
            {
                result = new System.Web.Mvc.MvcHtmlString("The SocialShare widget could not be found.");
            }

            return result;
        }

        /// <summary>
        /// Socials the share options. Redirect to the SocialShare control if exist else render error message
        /// </summary>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="dataItem">The data item which we will be sharing</param>
        public static System.Web.Mvc.MvcHtmlString SocialShareOptions(this HtmlHelper helper, Telerik.Sitefinity.Model.IHasTitle dataItem)
        {
            System.Web.Mvc.MvcHtmlString result;
            try
            {
                RouteValueDictionary routeValues = new RouteValueDictionary();
                routeValues.Add(SocialShareHelpers.DataItemKey, dataItem);
                result = helper.Action(ActionName, ControllerName, routeValues);
            }
            catch (HttpException)
            {
                result = new System.Web.Mvc.MvcHtmlString("The SocialShare widget could not be found.");
            }

            return result;
        }

        public const string DataItemKey = "DataItem";

        private const string ActionName = "Index";
        private const string ControllerName = "SocialShare";
    }
}