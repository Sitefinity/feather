using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.LayoutTemplates
{
    /// <summary>
    /// This is a test class with tests related to layout templates with multiple placeholders
    /// </summary>
    [TestClass]
    public class LayoutTemplateWithMultiplePlaceholders : FeatherTestCase
    {
        /// <summary>
        /// UI test AddWidgetToPageBasedOnLayoutWithMultiplePlaceholders.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void AddWidgetToPageBasedOnLayoutWithMultiplePlaceholders()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageTitle);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropWidgetToPlaceholder(WidgetCaption, FirstPlaceHolderId);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropWidgetToPlaceholder(WidgetCaption, SecondPlaceHolderId);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption, dropZoneIndex: 0);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetTextDummyWidget(FirstWidgetText);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption, dropZoneIndex: 1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetTextDummyWidget(SecondWidgetText);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();

            BAT.Macros().NavigateTo().CustomPage("~/" + PageTitle.ToLower(), false);

            Assert.IsTrue(ActiveBrowser.ContainsText(FirstWidgetText));
            Assert.IsTrue(ActiveBrowser.ContainsText(SecondWidgetText));
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

        private const string PageTitle = "FeatherTestPage";
        private const string WidgetCaption = "SimpleWidget";
        private const string FirstPlaceHolderId = "Contentplaceholder1";
        private const string SecondPlaceHolderId = "Contentplaceholder2";
        private const string FirstWidgetText = "Widget1";
        private const string SecondWidgetText = "Widget2";
    }
}
