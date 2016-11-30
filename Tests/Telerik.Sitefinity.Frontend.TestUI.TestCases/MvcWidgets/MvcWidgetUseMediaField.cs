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
    /// This is test class for MVC widget that use media field.
    /// </summary>
    [TestClass]
    public class MvcWidgetUseMediaField_ : FeatherTestCase
    {
        /// <summary>
        /// This is test class for MVC widget that use media field.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.SitefinityTeam2),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcWidgetUseMediaField()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsMessageAppear(false);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ChangeDocumentButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectMediaFile(DocumentTitle, true);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelectingOfDocument();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();

            BAT.Macros().NavigateTo().Modules().Documents();
            BAT.Wrappers().Backend().DocumentsAndFiles().DocumentsAndFilesDashboardWrapper().OpenDefaultLibrary();
            BAT.Wrappers().Backend().Images().ImagesDashboard().SelectItemsInGrid(this.itemToUnPublish);
            BAT.Wrappers().Backend().Images().ImagesDashboard().ClickMoreActionsMenuAndSelectOption(ActionNameUnpublish);

            ActiveBrowser.Refresh();
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsMessageAppear(true);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();


            BAT.Macros().NavigateTo().Modules().Documents();
            BAT.Wrappers().Backend().DocumentsAndFiles().DocumentsAndFilesDashboardWrapper().OpenDefaultLibrary();
            BAT.Wrappers().Backend().Images().ImagesDashboard().SelectItemsInGrid(this.itemToUnPublish);
            BAT.Wrappers().Backend().Images().ImagesDashboard().ClickMoreActionsMenuAndSelectOption(ActionNamePublish);

            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsMessageAppear(false);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();

            BAT.Macros().NavigateTo().Modules().Documents();
            BAT.Wrappers().Backend().DocumentsAndFiles().DocumentsAndFilesDashboardWrapper().OpenDefaultLibrary();
            BAT.Wrappers().Backend().Images().ImagesDashboard().SelectItemsInGrid(this.itemToUnPublish);
            BAT.Wrappers().Backend().DocumentsAndFiles().DocumentsAndFilesDashboardWrapper().PerformDeleteOfCheckedFile();
            ActiveBrowser.Refresh();
            ActiveBrowser.WaitUntilReady();

            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsMessageAppear(true);
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
        private const string WidgetCaption = "MediaWidget";
        private const string DocumentTitle = "Document2";
        private readonly string[] itemToUnPublish = new string[] { "Document2" };
        private const string ActionNameUnpublish = "Unpublish documents";
        private const string ActionNamePublish = "Publish documents";
    }
}
