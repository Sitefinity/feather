﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Framework.Utilities;
using Telerik.Sitefinity.TestUI.Framework.Wrappers.Backend.PageEditor;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.GridWidgets
{
    /// <summary>
    /// This is test class for old and new grid widget on page.
    /// </summary>
    [TestClass]
    public class OldLayoutWidgetAndNewGridWidgetOnTheSamePage_ : FeatherTestCase
    {
        /// <summary>
        /// UI test OldLayoutWidgetAndNewGridWidgetOnTheSamePage
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.FeatherTeam),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void OldLayoutWidgetAndNewGridWidgetOnTheSamePage()
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/pages", false));
            RuntimeSettingsModificator.ExecuteWithClientTimeout(800000, () => BAT.Macros().User().EnsureAdminLoggedIn());              
            BAT.Wrappers().Backend().Pages().PagesWrapper().OpenPageZoneEditor(PageName);
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().SwitchEditorLayoutMode(EditorLayoutMode.Layout);
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidgetToPlaceholder(OldLayoutCaption, "Body");
            BATFrontend.Wrappers().Backend().Widgets().GridWidgets().ClickBootstrapGridWidgetButton();
            BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropLayoutWidgetToPlaceholder(LayoutCaption, "Body");
            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().PublishPage();
            this.VerifyGridWidgetOnTheFrontend();
        }

        /// <summary>
        /// Verify grid widget on the frontend
        /// </summary>
        public void VerifyGridWidgetOnTheFrontend()
        {
            string[] layoutsNew = new string[] { LayouClass3, LayouClass4 };
            string[] layoutsOld = new string[] { LayouClass1, LayouClass2 };

            BAT.Macros().NavigateTo().CustomPage("~/" + PageName.ToLower(), false);
            ActiveBrowser.WaitUntilReady();

            ////BATFrontend.Wrappers().Frontend().Widgets().GridWidgets().VerifyOldGridWidgetOnTheFrontend(layoutsOld);
            BATFrontend.Wrappers().Frontend().Widgets().GridWidgets().VerifyNewGridWidgetOnTheFrontend(layoutsNew);
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
        private const string OldLayoutCaption = "25% + 75%";
        private const string LayoutCaption = "grid-3+9";
        private const string LayouClass1 = "sf_colsOut sf_2cols_1_25";
        private const string LayouClass2 = "sf_colsOut sf_2cols_2_75";
        private const string LayouClass3 = "col-md-3";
        private const string LayouClass4 = "col-md-9";
    }
}
