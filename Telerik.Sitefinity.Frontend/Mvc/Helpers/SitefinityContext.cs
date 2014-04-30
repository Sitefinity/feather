using System;
using Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Security;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Security.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class creates helper which provides commonly used data to the Mvc views. 
    /// </summary>
    public static class SitefinityContext
    {
        #region Public Properties

        public static PageDataViewModel Page
        {
            get
            {
                return SitefinityContext.GetPage();
            }
        }

        public static SiteViewModel Site
        {
            get
            {
                return SitefinityContext.GetSite();
            }
        }

        public static ProfileViewModel Profile
        {
            get
            {
                return SitefinityContext.GetCurrentSitefinityUserProfile();
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
                var result = new PageDataViewModel()
                {
                    Title = pageNode.Page.Title,
                    Keywords = pageNode.Page.Keywords
                };
                return result;
            }
            else
            {
                return new PageDataViewModel();
            }
        }

        private static ProfileViewModel GetCurrentSitefinityUserProfile()
        {
            var identity = ClaimsManager.GetCurrentIdentity();
            var currentUserGuid = identity.UserId;
            if (currentUserGuid != Guid.Empty)
            {
                UserProfileManager profileManager = UserProfileManager.GetManager();
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
