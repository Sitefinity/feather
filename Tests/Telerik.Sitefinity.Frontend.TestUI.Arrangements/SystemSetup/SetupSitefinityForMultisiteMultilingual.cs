using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestIntegration.Helpers;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.TestUI.Arrangements
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
            AuthenticationHelper.AuthenticateUser(Admin, Password);
            MultisiteHelper.CreateSite(SiteName, Url, Cultures, SiteName + "Provider");
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
        private static readonly List<string> Cultures = new List<string>() { "en", "bg-bg" };
        private const string Admin = "admin";
        private const string Password = "admin@2";
    }
}
