using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;
using Telerik.Sitefinity.Web.Events;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class creates helper which provides commonly used data to the Mvc views. 
    /// </summary>
    public static class SitefinityContext
    {
        #region Public Properties

        /// <summary>
        /// Gets the page.
        /// </summary>
        /// <value>
        /// The page.
        /// </value>
        public static PageDataViewModel Page
        {
            get
            {
                return SitefinityContext.GetPage();
            }
        }

        /// <summary>
        /// Gets the site.
        /// </summary>
        /// <value>
        /// The site.
        /// </value>
        public static SiteViewModel Site
        {
            get
            {
                return SitefinityContext.GetSite();
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <value>
        /// The profile.
        /// </value>
        public static ProfileViewModel Profile
        {
            get
            {
                return SitefinityContext.GetCurrentSitefinityUserProfile();
            }
        }

        /// <summary>
        /// Determines whether this method is invoked from Sitefinity's backend.
        /// </summary>
        /// <returns></returns>
        public static bool IsBackend
        {
            get
            {
                return ControlExtensions.IsBackend();
            }
        }

        /// <summary>
        /// Determines whether this method is invoked from Sitefinity's preview mode.
        /// </summary>
        public static bool IsPreviewMode
        {
            get
            {
                return SystemManager.IsPreviewMode;
            }
        }

        /// <summary>
        /// Gets the frontend login URL for the current site as configured in the backend.
        /// </summary>
        /// <value>
        /// The frontend login URL.
        /// </value>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
        public static string FrontendLoginUrl
        {
            get
            {
                var redirectStrategy = RedirectStrategyType.None;
                var wrapper = new HttpContextWrapper(HttpContext.Current);
                var pageUrl = RouteHelper.GetFrontEndLogin(wrapper, out redirectStrategy);

                return pageUrl;
            }
        }

        #endregion

        #region Private Members

        private static SiteViewModel GetSite()
        {
            return new SiteViewModel()
            {
                Title = SystemManager.CurrentContext.CurrentSite.Name
            };
        }

        private static PageDataViewModel GetPage()
        {
            var siteMapProvider = SiteMapBase.GetCurrentProvider();
            if (siteMapProvider != null && siteMapProvider.CurrentNode != null)
            {
                var pm = new PageManager();
                var pageNode = pm.GetPageNode(new Guid(siteMapProvider.CurrentNode.Key));
                var result = new PageDataViewModel
                {
                    Title = pageNode.Page.Title,
                    Keywords = pageNode.Page.Keywords
                };
                return result;
            }

            return new PageDataViewModel();
        }

        private static ProfileViewModel GetCurrentSitefinityUserProfile()
        {
            var identity = ClaimsManager.GetCurrentIdentity();
            var currentUserGuid = identity.UserId;
            if (currentUserGuid != Guid.Empty)
            {
                var profileManager = UserProfileManager.GetManager();
                var user = SecurityManager.GetUser(currentUserGuid);
                if (user != null)
                {
                    var profile = profileManager.GetUserProfile<SitefinityProfile>(user);
                    if (profile != null)
                    {
                        var result = new ProfileViewModel();
                        result.FirstName = profile.FirstName;
                        return result;
                    }
                }
            }

            return new ProfileViewModel();
        }

        #endregion
    }
}