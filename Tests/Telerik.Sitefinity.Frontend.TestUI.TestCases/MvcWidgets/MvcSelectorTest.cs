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
    /// This is test class for MvcSelectorTest.
    /// </summary>
    [TestClass]
    public class MvcSelectorTest_ : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcSelectorTest()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetTitle(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetSaveButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCancelButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("newsItemsSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(3);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetSearchText("Title1");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedNewsName1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInFlatSelectors(SelectedNewsName1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("tagSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(4);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(TagTitle);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInFlatSelectors(TagTitle);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(TagTitle);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(TagTitle);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);

            for (int i = 0; i < 2; i++)
            {
                BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickAdvancedButton();
                BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSelectorButton();
            }

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("newsItemsSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(3);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedNewsName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItemInFlatSelectors(SelectedNewsName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(SelectedNewsName2);
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
        private const string SelectedNewsName1 = "News Item Title1";
        private const string SelectedNewsName2 = "News Item Title2"; 
        private const string TagTitle = "Tag Title3";
    }
}
