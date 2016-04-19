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
    /// This is test class for MVC widget that use master detail content container.
    /// </summary>
    [TestClass]
    public class MvcWidgetUseMasterDetailContentController_ : FeatherTestCase
    {
        /// <summary>
        /// UI test MvcWidgetUseMasterDetailContentController.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.SitefinityTeam2),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcWidgetUseMasterDetailContentController()
        {
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            Assert.IsTrue(ActiveBrowser.ContainsText(PageText), "Page text is not correct.");           
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
        private const string PageText = "Default";
    }
}
