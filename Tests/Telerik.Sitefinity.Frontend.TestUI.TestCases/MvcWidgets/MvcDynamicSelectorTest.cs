using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// This is test class for MVC widget designer test.
    /// </summary>
    [TestClass]
    public class MvcDynamicSelectorTest_ : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
        TestCategory(FeatherTestCategories.PagesAndContent), Ignore]
        public void MvcDynamicSelectorTest()

        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetTitle(WidgetTitle);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetSaveButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCancelButton();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("dynamicItemsSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(3);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetSearchText("Title1");

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedName1);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(SelectedName1);
         
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(SelectedName1);

            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(SelectedName1);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("dynamicItemsSingleSelector");

            //// Commented because of Bug 276501
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForItemsToAppear(3);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(SelectedName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(SelectedName2);
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
        private const string WidgetTitle = "DummyText";
        private const string WidgetCaption = "SelectorWidget";
        private const string SelectedName1 = "Item Title1";
        private const string SelectedName2 = "Item Title2"; 
    }
}
