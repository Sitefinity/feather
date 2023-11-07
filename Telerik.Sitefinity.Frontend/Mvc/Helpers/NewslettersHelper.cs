using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.Newsletters;
using Telerik.Sitefinity.Modules.Newsletters.Configuration;
using Telerik.Sitefinity.Modules.Newsletters.Web;
using Telerik.Sitefinity.Newsletters.Model;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for email campaigns. 
    /// </summary>
    public static class NewslettersHelper
    {
        /// <summary>
        /// Indicates whether the current request is for newsletter.
        /// </summary>
        /// <returns><c>True</c> if the current request is for newsletter.</returns>
        public static bool IsNewsletter()
        {
            var httpContext = SystemManager.CurrentHttpContext;

            var newsLetterItem = httpContext.Items.Contains(NewslettersModule.IsNewsletter) ? (bool)httpContext.Items[NewslettersModule.IsNewsletter] : false;

            var isNewsletter = newsLetterItem
                    || (httpContext.Request.UrlReferrer != null
                        && httpContext.Request.UrlReferrer.AbsolutePath.ToLower().Contains("/sitefinity/sfnwslttrs/"))
                    || httpContext.Request.Url.AbsolutePath.ToLower().Contains("/sitefinity/sfnwslttrs/");

            return isNewsletter;
        }

        /// <summary>
        /// Indicates whether the current request is for pure mvc newsletter.
        /// </summary>
        /// <returns><c>True</c> if the current request is for mvc newsletter.</returns>
        internal static bool IsMvcNewsletter()
        {
            var currentHttpContext = SystemManager.CurrentHttpContext;

            return SystemManager.GetApplicationModule(NewslettersModule.ModuleName) != null
                    && !Config.Get<NewslettersConfig>().AllowHybridWidgets
                    && currentHttpContext != null
                    && currentHttpContext.Items.Contains(NewslettersRouteHandler.NewslettersMessageBodyType)
                    && (MessageBodyType)currentHttpContext.Items[NewslettersRouteHandler.NewslettersMessageBodyType] == MessageBodyType.InternalMvcPage;
        }
    }
}
