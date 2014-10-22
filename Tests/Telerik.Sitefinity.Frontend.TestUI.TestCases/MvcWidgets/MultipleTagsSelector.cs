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
    /// Verifies multiple tags selector.
    /// </summary>
    [TestClass]
    public class MultipleTagsSelector : FeatherTestCase
    {
        /// <summary>
        /// Verifies selected tab in multiple selector and ordering in all tab of selected items.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Sitefinity Team 7"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MultipleSelectorVerifySelectedTabAndOrderingInAllTab()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedTab(SelectedTabAfterSelection);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().OpenAllTab();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemsInAllTab(TagPrefixName, selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
        }

        /// <summary>
        /// Verifies items selection after switching to Advanced settings.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Sitefinity Team 7"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MultipleSelectorVerifySelectionAfterSwitchingToAdvancedSettings()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(selectedTagNames);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickAdvancedButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSelectorButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
        }

        /// <summary>
        /// Creates a couple of tags and a page with dummy widget.
        /// </summary>
        protected override void ServerSetup()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
            BAT.Arrange(ArrangementClassName).ExecuteSetUp();
        }

        /// <summary>
        /// Deletes all tags and pages.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(ArrangementClassName).ExecuteTearDown();
        }

        private const string ArrangementClassName = "MultipleTagsSelector";

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";

        private const string SelectedTabAfterSelection = "Selected";
        private const string TagSelectorName = "TagsMultipleSelector";
        private const string TagPrefixName = "Tag Title";
        private const int TagItemsToAppearCount = 10;

        private readonly string[] selectedTagNames = { "Tag Title1", "Tag Title2", "Tag Title6" };
    }
}
