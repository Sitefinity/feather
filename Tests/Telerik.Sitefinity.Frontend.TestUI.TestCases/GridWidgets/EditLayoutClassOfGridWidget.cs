using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Framework.Wrappers.Backend.PageEditor;
using Telerik.TestUI.Core.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.GridWidgets
{
    /// <summary>
    /// This is test class for edit of grid widget on page.
    /// </summary>
    [TestClass]
    public class EditLayoutClassOfGridWidget_ : FeatherTestCase
    {
        /// <summary>
        /// UI test EditLayoutClassOfGridWidget
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.SitefinityTeam2),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void EditLayoutClassOfGridWidget()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/pages", false, null, new HtmlFindExpression("class=~sfMain")));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () => BAT.Macros().User().EnsureAdminLoggedIn());
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);          
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidgetToPlaceholder(LayoutCaption);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(LayoutCaption, dropZoneIndex: 0);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().SetCustomCssToGridWidget(LayoutCaption);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            this.VerifyGridWidgetOnTheFrontend();
        }
        
        /// <summary>
        /// Verify grid widget on the frontend
        /// </summary>
        public void VerifyGridWidgetOnTheFrontend()
        {
            string[] layouts = new string[] { LayoutCaption };

            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            ActiveBrowser.WaitUntilReady();

            BATFrontend.Wrappers().Frontend().Widgets().GridWidgets().VerifyCustomGridWidgetOnTheFrontend(layouts);
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
        private const string LayoutCaption = "grid-customcss";
    }
}
