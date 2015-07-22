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
    /// This is test class for MVC widget designer test.
    /// </summary>
    [TestClass]
    public class MvcWidgetDefaultDesigner : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcWidgetDefaultFeatherDesigner()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetTitle(WidgetName);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetInputFieldLabelText(InputFieldLabel);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyDummyWidgetInputTextField();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetSaveButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCancelButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCloseButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetTextDummyWidget(DummyText);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower());
            var pageContent = BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().GetPageContent();
            Assert.IsTrue(pageContent.TextContent.Contains(DummyText));
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
        private const string WidgetName = "DummyWidget";
        private const string InputFieldLabel = "DummyText";
        private const string DummyText = "Dummy Text";
    }
}
