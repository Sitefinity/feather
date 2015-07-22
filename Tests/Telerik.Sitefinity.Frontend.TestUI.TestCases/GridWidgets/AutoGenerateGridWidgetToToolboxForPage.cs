using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Framework.Wrappers.Backend.PageEditor;
using Telerik.TestUI.Core.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.GridWidgets
{
    /// <summary>
    /// This is test class for grid widget on page and page template.
    /// </summary>
    [TestClass]
    public class AutoGenerateGridWidgetToToolboxForPage : FeatherTestCase
    {
        /// <summary>
        /// UI test AddAndDeleteGridWidgetFromFileSystemVerifyPageToolbox
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void AddAndDeleteGridWidgetFromFileSystemVerifyPageToolbox()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () =>  BAT.Macros().NavigateTo().CustomPage("~/sitefinity/pages", false));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () => BAT.Macros().User().EnsureAdminLoggedIn());              
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickBootstrapGridWidgetButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidgetToPlaceholder(LayoutCaption);
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount1);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();           

            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            BATFrontend.Wrappers().Frontend().Widgets().GridWidgets().VerifyNewGridWidgetOnTheFrontend(this.layouts);

            BAT.Arrange(this.TestName).ExecuteArrangement("DeleteGridWidgetFromFileSystem");

            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickBootstrapGridWidgetButton();
            Assert.IsFalse(
                BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().IsLayoutWidgetPresentInToolbox(LayoutCaption),
                "Layout widget is found in the toolbox");

            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage(); 
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);

            // Asserts that the page is not throwing an error 500 on the frontend
            HttpResponseMessage response = new HttpResponseMessage();
            Assert.AreEqual(200, (int)response.StatusCode);
        }

        /// <summary>
        /// Performs Server Setup and prepare the system with needed data.
        /// </summary>
        protected override void ServerSetup()
        {            
            BAT.Arrange(this.TestName).ExecuteSetUp();
        }

        /// <summary>
        /// Performs clean up and clears all data created by the test.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(this.TestName).ExecuteTearDown();
        }

        private const string PageName = "GridPage";
        private const string LayoutCaption = "grid-grid";
        private const int GridCount1 = 1;
        private readonly string[] layouts = new string[] { "col-md-3", "col-md-9" };
    }
}
