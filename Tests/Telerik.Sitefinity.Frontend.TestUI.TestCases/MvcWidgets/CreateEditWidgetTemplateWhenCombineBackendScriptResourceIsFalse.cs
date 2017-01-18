using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.TestUI.Core.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.MvcWidgets
{
    /// <summary>
    /// This is test class create and edit widget template when Combine backend script resourse is set to False.
    /// </summary>
    [TestClass]
    public class CreateEditWidgetTemplateWhenCombineBackendScriptResourceIsFalse_ : FeatherTestCase
    {
        /// <summary>
        /// UI test CreateEditWidgetTemplateWhenCombineBackendScriptResourceIsFalse.
        /// </summary>
        [TestMethod,
        Owner(FeatherTeams.SitefinityTeam2),
        TestCategory(FeatherTestCategories.PagesAndContent)]
        public void CreateEditWidgetTemplateWhenCombineBackendScriptResourceIsFalse()
        {            
            this.NavigateToValidationSectionInAdvancedSettings(this.sectionNames);
            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CombineBackendScriptResources(IsSetCombineResources);
            BAT.Wrappers().Backend().Comments().CommentsSettingsWrapper().ClickSaveChangesButton();
            BAT.Arrange(this.TestName).ExecuteArrangement("RestartApp");

            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().NavigateTo().CustomPage("~/sitefinity/design/controltemplates", false));
            BAT.Wrappers().Backend().ModuleBuilder().ContentTypePageActionsWrapper().OpenWidgetTemplateByAreaAndName(widgetTemplatesNames[0], AreaName);
            BAT.Wrappers().Backend().ModuleBuilder().ContentTypePageActionsWrapper().ClickSaveChangesLink();

            BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().CreateTemplate();
            BATFrontend.Wrappers().Backend().Widgets().WidgetTemplatesCreateScreenFrameWrapper().SelectTemplate("News - list");
            BATFrontend.Wrappers().Backend().Widgets().WidgetTemplatesCreateScreenFrameWrapper().EnterTextInTextArea("Test");
            BATFrontend.Wrappers().Backend().Widgets().WidgetTemplatesCreateScreenFrameWrapper().EnterWidgetTemplateName(widgetTemplatesNames[1]);
            BATFrontend.Wrappers().Backend().Widgets().WidgetTemplatesCreateScreenFrameWrapper().CreateThisTemplate();

            BAT.Wrappers().Backend().ModuleBuilder().ContentTypePageActionsWrapper().OpenWidgetTemplateByAreaAndName(widgetTemplatesNames[1], AreaName);
            BAT.Wrappers().Backend().ModuleBuilder().ContentTypePageActionsWrapper().VerifyWidgetTemplateContent("Test");
            BAT.Wrappers().Backend().ModuleBuilder().ContentTypePageActionsWrapper().ClickSaveChangesLink();
        }

        /// <summary>
        /// Navigates to validation section.
        /// </summary>
        /// <param name="sectionNames">The section names.</param>
        public void NavigateToValidationSectionInAdvancedSettings(string[] sectionNames)
        {
            RuntimeSettingsModificator.ExecuteWithClientTimeout(1600000, () => BAT.Macros().NavigateTo().CustomPage("~/Sitefinity/Administration/Settings/Advanced", false));
            ActiveBrowser.WaitForAsyncOperations();

            for (int i = 0; i < sectionNames.Length; i++)
            {
                BAT.Wrappers().Backend().Settings().SettingsWrapper().OpenSectionInAdvancedSettings(sectionNames[i]);
            }
        }

        /// <summary>
        /// Performs Server Setup and prepare the system with needed data.
        /// </summary>
        protected override void ServerSetup()
        {
            BAT.Macros().User().EnsureAdminLoggedIn();
        }

        /// <summary>
        /// Performs clean up and clears all data created by the test.
        /// </summary>
        protected override void ServerCleanup()
        {
            BAT.Arrange(this.TestName).ExecuteTearDown();
        }
        private const string IsSetCombineResources = "False";
        private string[] sectionNames = new string[] { "Pages" };
        private const string AreaName = @"News:";

        private string[] widgetTemplatesNames = new string[] { "Titles only", "NewsWidgetTemplate" };
    }
}
