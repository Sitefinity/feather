using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.TestUI.Framework.Wrappers.Backend.PageEditor;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.GridWidgets
{
    /// <summary>
    /// This is test class for grid widget on page and page template.
    /// </summary>
    [TestClass]
    public class AddGridWidgetToToolboxForPageTemplate_ : FeatherTestCase
    {
        /// <summary>
        /// UI test AddGridWidgetToToolboxForPageTemplate
        /// </summary>
        [TestMethod,
       Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
       TestCategory(FeatherTestCategories.PagesAndContent)]
        public void AddGridWidgetToToolboxForPageTemplate()
        {
            BAT.Macros().NavigateTo().Design().PageTemplates();
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(PageTemplateName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickBootstrapGridWidgetButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidget(LayoutCaption);
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount1);
            BAT.Wrappers().Backend().PageTemplates().PageTemplateModifyScreen().PublishTemplate();
            this.VerifyGridWidgetOnTheFrontend();
        }

        /// <summary>
        /// Verify grid widget on the frontend
        /// </summary>
        public void VerifyGridWidgetOnTheFrontend()
        {
            string[] layouts = new string[] { LayouClass1, LayouClass2 };

            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            ActiveBrowser.WaitUntilReady();

            BATFrontend.Wrappers().Frontend().Widgets().GridWidgets().VerifyNewGridWidgetOnTheFrontend(layouts);
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

        private const string PageTemplateName = "Bootstrap.defaultNew";
        private const string PageName = "GridPage";
        private const string LayoutCaption = "grid-grid";
        private const string LayouClass1 = "col-md-3";
        private const string LayouClass2 = "col-md-9";
        private const int GridCount1 = 1;
    }
}
