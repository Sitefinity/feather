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
    /// This is test class for grid widget on page template.
    /// </summary>
    [TestClass]
    public class ManageGridWidgetOnThePageTemplate_ : FeatherTestCase
    {
        /// <summary>
        /// UI test ManageGridWidgetOnThePageTemplate
        /// </summary>
        [TestMethod,
       Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
       TestCategory(FeatherTestCategories.PagesAndContent)]
        public void ManageGridWidgetOnThePageTemplate()
        {
            BAT.Macros().NavigateTo().Design().PageTemplates();
            BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(PageTemplateName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickBootstrapGridWidgetButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidget(LayoutCaption);
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount1);
            this.DuplicateGridElement();
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount2);
            this.DeleteGridElement();
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount1);
            this.DeleteGridElement();
            BAT.Wrappers().Backend().Pages().PageLayoutEditorWrapper().VerifyLayoutWidgetPageEditor(LayoutCaption, GridCount0);
            BAT.Wrappers().Backend().PageTemplates().PageTemplateModifyScreen().PublishTemplate();
        }

        /// <summary>
        /// Duplicates a grid widget in the zone editor
        /// </summary>
        public void DuplicateGridElement()
        {
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SelectExtraOptionForWidget(DuplicateOperation);
            ActiveBrowser.WaitUntilReady();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Deletes a grid widget from the editor
        /// </summary>
        public void DeleteGridElement()
        {
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SelectExtraOptionForWidget(DeleteOperation);
            ActiveBrowser.WaitUntilReady();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Performs Server Setup and prepare the system with needed data.
        /// </summary>
        protected override void ServerSetup()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
        }

        private const string PageTemplateName = "Bootstrap.default";
        private const string DuplicateOperation = "Duplicate";
        private const string DeleteOperation = "Delete";
        private const string LayoutCaption = "3 + 9";
        private const int GridCount0 = 0;
        private const int GridCount1 = 1;
        private const int GridCount2 = 2;
    }
}
