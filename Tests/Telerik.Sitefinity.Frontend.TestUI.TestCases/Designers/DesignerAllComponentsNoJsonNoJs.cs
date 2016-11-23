﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.TestUI.Core.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.Designers
{
    /// <summary>
    /// This is test class for DesignerAllComponentsNoJsonNoJs.
    /// </summary>
    [TestClass]
    public class DesignerAllComponentsNoJsonNoJs_ : FeatherTestCase
    {
        /// <summary>
        /// UI test DesignerAllComponentsNoJsonNoJs.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.SitefinityTeam2),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void DesignerAllComponentsNoJsonNoJs()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(3000000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/pages", false));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(3000000, () => BAT.Macros().User().EnsureAdminLoggedIn());
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().EditWidget(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForSaveButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetTitle(WidgetCaption);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetSaveButton();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyWidgetCancelButton();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("Top");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("selectors");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("news");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("newsItemsSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForDoneButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("taxa");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().SelectContent("tagSingleSelector");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().WaitForDoneButtonToAppear();
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().DoneSelecting();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("Top");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("Bottom");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ExpandOptions("html field");
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().VerifyHtmlFieldContent();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().ClickSaveButton();
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
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

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";
        private const string SelectedNewsName1 = "News Item Title1";
        private const string SelectedNewsName2 = "News Item Title2"; 
        private const string TagTitle = "Tag Title3";
    }
}
