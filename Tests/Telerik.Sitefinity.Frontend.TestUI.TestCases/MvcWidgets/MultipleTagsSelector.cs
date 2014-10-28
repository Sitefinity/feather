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
        public void MultipleTagsSelectorVerifySelectedTabAndOrderingInAllTab()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(selectedTagNames);
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
        TestCategory(FeatherTestCategories.PagesAndContent),
        Ignore]
        public void MultipleTagsSelectorVerifySelectionAfterSwitchingToAdvancedSettings()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(selectedTagNames);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickAdvancedButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSelectorButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(selectedTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
        }

        /// <summary>
        /// Verifies selected items reordering from top to bottom for tag items.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Sitefinity Team 7"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MultipleTagsSelectorVerifySelectedItemsReorderingFromTopToBottom()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            var countOfSelectedItems = selectedTagNames.Count();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CheckNotificationInSelectedTab(countOfSelectedItems);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().OpenSelectedTab();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppearInSelectedTab(countOfSelectedItems);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ReorderSelectedItems(expectedOrderOfTagNames, selectedTagNames, reorderedIndexMapping);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(expectedOrderOfTagNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(expectedOrderOfTagNames);
        }

        /// <summary>
        /// Verifies search in Selected tab.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Sitefinity Team 7"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MultipleTagsSelectorVerifySearchInSelectedTab()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(TagSelectorName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(TagItemsToAppearCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(selectedTagNames);
            var countOfSelectedItems = selectedTagNames.Count();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CheckNotificationInSelectedTab(countOfSelectedItems);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().OpenSelectedTab();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppearInSelectedTab(countOfSelectedItems);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyReorderingIconVisibility(SelectedTagItemsCount, false);

            //// filter selected results and verify correct span css class is applied
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetSearchText(SearchText);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppearInSelectedTab(1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyReorderingIconVisibility(FilteredTagItemsCount, true);

            //// clear search and verify that correct span css class is applied
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetSearchText("");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyReorderingIconVisibility(SelectedTagItemsCount, false);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(selectedTagNames);
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
        private const string SearchText = "Title2";
        private const int TagItemsToAppearCount = 10;
        private const int SelectedTagItemsCount = 5;
        private const int FilteredTagItemsCount = 1;

        private readonly string[] selectedTagNames = { "Tag Title1", "Tag Title2", "Tag Title6", "Tag Title7", "Tag Title9" };
        private readonly string[] expectedOrderOfTagNames = { "Tag Title2", "Tag Title1", "Tag Title7", "Tag Title6", "Tag Title9" };

        private readonly Dictionary<int, int> reorderedIndexMapping = new Dictionary<int, int>() { {0, 2}, {2, 4} };
    }
}