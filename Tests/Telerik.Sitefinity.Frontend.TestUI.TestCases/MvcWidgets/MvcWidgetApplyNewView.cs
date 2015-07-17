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
    /// This is test class for MVC widget applying view from package.
    /// </summary>
    [TestClass]
    public class MvcWidgetApplyNewView : FeatherTestCase
    {
        /// <summary>
        /// UI test MvcWidgetUseViewFromLayoutFolderAndPackage.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcWidgetUseViewFromLayoutFolderAndPackage()
        {
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(DefaultViewText), "Default view text is not correct.");
            BAT.Arrange(this.TestName).ExecuteArrangement("AddNewViewToLayoutsFolder");
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(DefaultViewFromLayoutsFolderText), "Default view text from Layouts folder is not correct.");
            BAT.Arrange(this.TestName).ExecuteArrangement("AddNewViewToPackage");
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(DefaultViewFromPackageText), "Default view text from package is not correct.");
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

        private const string PageName = "FeatherPage";
        private const string DefaultViewText = "This is a test default view.";
        private const string DefaultViewFromPackageText = "This is a view from package.";
        private const string DefaultViewFromLayoutsFolderText = "This is a view from Layouts Folder.";
    }
}
