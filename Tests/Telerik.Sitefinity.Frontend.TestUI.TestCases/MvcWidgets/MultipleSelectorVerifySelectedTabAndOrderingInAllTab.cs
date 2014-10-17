using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// Verifies selected tab in multi selector, ordering in all tab of selected items.
    /// </summary>
    [TestClass]
    public class MultipleSelectorVerifySelectedTabAndOrderingInAllTab_ : FeatherTestCase
    {
        /// <summary>
        /// Verifies selected tab in multi selector, ordering in all tab of selected items.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MultipleSelectorVerifySelectedTabAndOrderingInAllTab()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppear);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedTab("Selected");

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().OpenAllTab();
            System.Threading.Thread.Sleep(10000);
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
            //// BAT.Arrange(this.TestName).ExecuteTearDown();
        }

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";

        private const string TagSelectorName = "TagsMultipleSelector";
        private const int TagItemsToAppear = 10;

        private readonly string[] selectedTagNames = { "Tag Title1", "Tag Title2", "Tag Title6" };
    }
}
