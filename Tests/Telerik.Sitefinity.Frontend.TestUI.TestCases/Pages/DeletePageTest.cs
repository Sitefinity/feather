using System;
using System.Linq;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.Pages
{
    /// <summary>
    /// This is a sample test class.
    /// </summary>
    [TestClass]
    public class DeletePageTest : FeatherTestCase
    {
        /// <summary>
        /// Pefroms Server Setup and prepare the system with needed data.
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

        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void DeletePage()
        {
            NavigateToPages();
            SelectPageForDelete();
            ClickDeleteButton();
            HandleDeletePageDialog();
            CheckCreatePageLinkNotPresent();
        }

        /// <summary>
        /// Navigate to pages.
        /// </summary>
        public void NavigateToPages()
        {
            BAT.Macros().NavigateTo().Pages();
            ActiveBrowser.WaitUntilReady();
            ActiveBrowser.WaitForAsyncOperations();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Select a page for delete.
        /// </summary>
        public void SelectPageForDelete()
        {
            var tree = BAT.Wrappers().Backend().Pages().PagesWrapper().GetTree();
            Assert.IsNotNull(tree, "The delete button was not found");
            Assert.IsTrue(tree.IsVisible(), "The delete button was not visible");
            tree.Refresh();
            var checkBox = tree.Find.AllByAttributes<HtmlInputCheckBox>("class=rtChk", "type=checkbox").FirstOrDefault();
            checkBox.CheckJquery(true, true);
        }

        /// <summary>
        /// Click delete button if present.
        /// </summary>
        public void ClickDeleteButton()
        {
            var deleteButton = Manager.Current.ActiveBrowser.WaitForElement(Manager.Current.Settings.Web.GetWaitForElementTimeout(), "id=?_delete").As<HtmlAnchor>();
            Assert.IsNotNull(deleteButton, "The delete button was not found");
            Assert.IsTrue(deleteButton.IsVisible(), "The delete button was not visible");
            deleteButton.Click();
        }

        /// <summary>
        /// Handles delete page dialog.
        /// </summary>
        public void HandleDeletePageDialog()
        {
            var confirmDeleteButtons = ActiveBrowser.Find.AllByAttributes("class=sfLinkBtn sfDelete");
            var deleteSingleItemButton = confirmDeleteButtons.Where(c => c.InnerText == "Yes, Move to the Recycle Bin").FirstOrDefault().As<HtmlAnchor>();
            Assert.IsNotNull(deleteSingleItemButton, "The confirm button was not found");
            deleteSingleItemButton.Click();
        }

        /// <summary>
        /// Check if the create page link is not visble.
        /// </summary>
        public void CheckCreatePageLinkNotPresent()
        {
            var createPageLink = BAT.Wrappers().Backend().Pages().PagesWrapper().GetCreatePageFromDecisionScreen();
            Assert.IsNotNull(createPageLink, "The Create Page button was not found.");
        }
    }
}