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
    /// This is test class for apllying templates from different packages.
    /// </summary>
    [TestClass]
    public class ApplyTemplatesFromDifferentPackages : FeatherTestCase
    {
        /// <summary>
        /// UI test ApplyTwoDifferentTemplatesFromDifferentPackagesToPages.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void ApplyTwoDifferentTemplatesFromDifferentPackagesToPages()
        {
            BAT.Macros().NavigateTo().Design().PageTemplates();
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(Template1Title);
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(Template2Title);

            BAT.Arrange(this.TestName).ExecuteArrangement("CreatePages");

            BAT.Macros().NavigateTo().CustomPage("~/" + Page1Title, false);
            Assert.IsTrue(ActiveBrowser.ContainsText(Package1LayoutText), "Layout text is not correct");
            ActiveBrowser.Refresh();
            ActiveBrowser.WaitUntilReady();
            Assert.IsTrue(ActiveBrowser.ContainsText(Package1LayoutText), "Layout text is not correct");

            BAT.Macros().NavigateTo().CustomPage("~/" + Page2Title, false);
            Assert.IsTrue(ActiveBrowser.ContainsText(Package2LayoutText), "Layout text is not correct");
            ActiveBrowser.Refresh();
            ActiveBrowser.WaitUntilReady();
            Assert.IsTrue(ActiveBrowser.ContainsText(Package2LayoutText), "Layout text is not correct");
        }

        /// <summary>
        /// Performs Server Setup and prepare the system with needed data.
        /// </summary>
        protected override void ServerSetup()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
            BAT.Arrange(this.TestName).ExecuteSetUp();
        }

        /// <summary>
        /// Performs clean up and clears all data created by the test.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(this.TestName).ExecuteTearDown();
        }
          
        private const string Template1Title = "Package11.test-layout";
        private const string Template2Title = "Package22.test-layout";
        private const string Page1Title = "page1";
        private const string Page2Title = "page2";
        private const string Package1LayoutText = "Package1 - test layout";
        private const string Package2LayoutText = "Package2 - test layout";
    }
}
