using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.ResourcePackages
{
    /// <summary>
    /// This is test class for testing cache invalidation when edit a layout file.
    /// </summary>
    [TestClass]
    public class EditLayoutFileCacheInvalidation : FeatherTestCase
    {
        /// <summary>
        /// UI test EditLayoutFileFromPackageCacheInvalidation.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void EditLayoutFileFromPackageCacheInvalidation()
        {
            BAT.Macros().NavigateTo().CustomPage("~/" + PageTitle.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(LayoutFileText), "Layout text is not correct.");

            BAT.Arrange(this.TestName).ExecuteArrangement("EditLayoutFile");

            BAT.Macros().NavigateTo().CustomPage("~/" + PageTitle.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(LayoutFileNewText), "Layout text after edit is not correct.");
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

        private const string PageTitle = "FeatherPage";
        private const string LayoutFileText = "Test Layout";
        private const string LayoutFileNewText = "New Text";
    }
}
