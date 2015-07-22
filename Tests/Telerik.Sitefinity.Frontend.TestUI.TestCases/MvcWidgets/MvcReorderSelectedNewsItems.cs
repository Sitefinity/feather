using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// This is test class for MvcReorderSelectedNewsItems.
    /// </summary>
    [TestClass]
    public class MvcReorderSelectedNewsItems_ : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcReorderSelectedNewsItems()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("newsItemsMultipleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(this.selectedNewsNames);
            var countOfSelectedItems = this.selectedNewsNames.Count();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CheckNotificationInSelectedTab(countOfSelectedItems);            
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().OpenSelectedTab();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(4);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ReorderSelectedItems(this.expectedOrderOfNames, this.selectedNewsNames, this.reorderedIndexMapping);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(this.expectedOrderOfNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("newsItemsMultipleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInMultipleSelectors(this.expectedOrderOfNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
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
        private const string WidgetCaption = "SelectorWidget";
        private readonly string[] selectedNewsNames = { "News Item Title1", "News Item Title5", "News Item Title6", "News Item Title9" };
        private readonly string[] expectedOrderOfNames = { "News Item Title9", "News Item Title5", "News Item Title1", "News Item Title6" };
        private readonly Dictionary<int, int> reorderedIndexMapping = new Dictionary<int, int>()
        {
            { 3, 0 },
            { 1, 3 }
        };
    }
}
