using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.ResourcePackages
{
    /// <summary>
    /// This is test class for adding new layout files tests.
    /// </summary>
    [TestClass]
    public class NewLayoutFileDefaultPackage : FeatherTestCase
    {
        /// <summary>
        /// UI test AddNewLayoutFileToDefaultPackage.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void AddNewLayoutFileToDefaultPackage()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
            BAT.Arrange(this.TestName).ExecuteArrangement("AddNewLayoutFile");

            BAT.Macros().NavigateTo().Design().PageTemplates();
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().IsItemPresentInGridView(TemplateTitle);
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(TemplateTitle);

            bool isTextPresent = BATFrontend.Wrappers().Backend().PageTemplates().PageTemplateEditor().IsTextPresentInTemplateMainContainer(LayoutText);
            Assert.IsTrue(isTextPresent, "Layout template text was not found");

            bool isPlaceholderPresent = BATFrontend.Wrappers().Backend().PageTemplates().PageTemplateEditor().IsPlaceHolderPresent(PlaceHolderId);
            Assert.IsTrue(isPlaceholderPresent, "Placeholder not found");           
        }

        /// <summary>
        /// Performs clean up and clears all data created by the test.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(this.TestName).ExecuteTearDown();
        }

        private const string TemplateTitle = "TestLayout";
        private const string LayoutText = "Test Layout";
        private const string PlaceHolderId = "TestPlaceHolder";
    }
}
