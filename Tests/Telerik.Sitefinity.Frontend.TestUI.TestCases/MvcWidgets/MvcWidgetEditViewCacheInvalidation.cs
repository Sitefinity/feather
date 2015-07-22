using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// This is test class for MVC widget cache invalidation tests.
    /// </summary>
    [TestClass]  
    public class MvcWidgetEditViewCacheInvalidation : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcWidgetEditViewFromPackageCacheInvalidation()
        {
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(DefaultViewFromPackageText), "Default view text is not correct.");
            BAT.Arrange(this.TestName).ExecuteArrangement("EditViewFromPackage");
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(DefaultViewFromPackageEditedText), "Default view text after edit is not correct.");
        }

        /// <summary>
        /// Performs Server Setup and prepare the system with needed data.
        /// </summary>
        protected override void ServerSetup()
        {
            BAT.Arrange(this.TestName).ExecuteSetUp();
        }

        /// <summary>
        /// Performs clean up and clears all data created by the test.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(this.TestName).ExecuteTearDown();
        }

        private const string PageName = "FeatherPage";
        private const string DefaultViewFromPackageText = "This is a view from package.";
        private const string DefaultViewFromPackageEditedText = "This is a view from package after edit.";
    }
}
