using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using System.Threading;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// This is test class for MVC widget designer test.
    /// </summary>
    [TestClass]
    public class MvcSelector : FeatherTestCase
    {
        /// <summary>
        /// UI test MVCWidgetDefaultFeatherDesigner.
        /// </summary>
        [TestMethod,
        Microsoft.VisualStudio.TestTools.UnitTesting.Owner("Feather team"),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void MvcSelectorTest()
        {
            BAT.Macros().NavigateTo().Pages();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            Thread.Sleep(5000);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetTitle(WidgetTitle);           

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetSaveButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCancelButton();

             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent();
             var actualCount = BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CountItems();
             Assert.AreEqual(3, actualCount);
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SetSearchText("Title1");
             var newActualCount = BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CountItems();
             Assert.AreEqual(1, newActualCount);
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedNewsName1);
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(SelectedNewsName1);

             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent(false);
             actualCount = BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CountItems();
            Assert.AreEqual(4, actualCount);
             newActualCount = BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CountItems();
             Assert.AreEqual(4, newActualCount);
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(ContentBlockTitle);
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(ContentBlockTitle);

             BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

             BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(ContentBlockTitle);
           
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(ContentBlockTitle);
            Thread.Sleep(5000);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);

            for (int i = 0; i < 2; i++)
            {
                BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickAdvancedButton();
                BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSelectorButton();

            }       
          
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent();
            actualCount = BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CountItems();
            Assert.AreEqual(3, actualCount);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectItem(SelectedNewsName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifySelectedItem(SelectedNewsName2);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();

            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().VerifyContentInWidget(SelectedNewsName2);
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
        private const string WidgetName = "SelectorWidget";
        private const string WidgetTitle = "DummyText";
        private const string WidgetCaption = "SelectorWidget";
        private const string DummyText = "Dummy Text";
        private const string SelectedNewsName1 = "News Item Title1";
        private const string SelectedNewsName2 = "News Item Title2"; 
        private const string ContentBlockTitle = "Content Block Title3";

    }
}
