using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestArrangementService.Core;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUtilities.Helpers.Models;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// SetupSitefinityForMultisiteMultilingual Arrangement
    /// </summary>
    public class SetupSitefinityForMultisiteMultilingual : ITestArrangement
    {
        /// <summary>
        /// Creates website with localization.
        /// </summary>
        [ServerArrangement]
        public void CreateMultilingualSite()
        {
            AuthenticationHelper.AuthenticateUser(AdminEmail, AdminPass);
            var siteName = ArrangementConfig.GetArrangementSite();
            var siteUrl = ArrangementConfig.GetArrangementSiteUrl();
            var siteCultures = ArrangementConfig.GetArrangementSiteCultures();
            var site = new SiteModel(siteName, siteUrl, siteName + "Provider", true) { Cultures = siteCultures };
            MultisiteHelper.CreateSite(site);
        }

        /// <summary>
        /// Deletes the additionally created site.
        /// </summary>
        [ServerArrangement]
        public void DeleteSite()
        {
            ServerOperations.MultiSite().DeleteSite(SiteName);
        }

        private const string SiteName = "SecondSite";
        private const string Url = "http://localhost:83/";
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private static readonly List<string> Cultures = new List<string>() { "en", "bg-bg" };
        private const string AdminEmail = "admin@test.test";
        private const string AdminPass = "admin@2";
    }
}
