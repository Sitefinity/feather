using System;
using ArtOfTest.WebAii.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.TestUI.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Framework.Utilities;

namespace Telerik.Sitefinity.Frontend.TestUI.TestCases.Module
{
    /// <summary>
    /// Deactivates Feather and checks if widget on pure page template is visible in the frontend and if it can be deleted in the backend.
    /// </summary>
    [TestClass]
    public class DeactivateFeatherAndDeleteWidgetFromPurePageTemplate_ : FeatherTestCase
    {
        /// <summary>
        /// Deactivates the feather and delete widget from pure page template.
        /// </summary>
        [TestMethod]
        [Owner(FeatherTeams.SitefinityTeam2)]
        [TestCategory(FeatherTestCategories.PagesAndContent)]
        public void DeactivateFeatherAndDeleteWidgetFromPurePageTemplate()
        {
            var featherDeactivated = false;

            try
            {
                // Add widget to template
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().User().EnsureAdminLoggedIn());
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(PageTemplatesPageUrl, false, null, new HtmlFindExpression("InnerText=" + PageTemplateName)));
                BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(PageTemplateName);
                BATFrontend.Wrappers().Backend().Pages().PageZoneEditorWrapper().DragAndDropWidgetToPlaceholder(WidgetName, Placeholder);
                Assert.IsTrue(BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().IsHtmlControlPresent(WidgetContent));
                BAT.Wrappers().Backend().PageTemplates().PageTemplateModifyScreen().PublishTemplate();

                // Verify on frontend
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(PageUrl, false, null, new HtmlFindExpression("InnerText=~" + WidgetContent)));
                Assert.IsTrue(BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsElementByInnerTextPresent(WidgetContent));

                // Deactivate Feather
                BATFrontend.Wrappers().Backend().FrontendModule().FrontendModule().DeactivateFeather(ActiveBrowser);
                featherDeactivated = true;

                // Verify on frontend
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(PageUrl, false));
                Assert.IsFalse(BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsElementByInnerTextPresent(WidgetContent));

                // Remove widget from template
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().User().EnsureAdminLoggedIn());
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(PageTemplatesPageUrl, false, null, new HtmlFindExpression("InnerText=" + PageTemplateName)));
                BAT.Wrappers().Backend().PageTemplates().PageTemplateMainScreen().OpenTemplateEditor(PageTemplateName);
                BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().DeleteWidget(WidgetName);
                Assert.IsFalse(BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().IsHtmlControlPresent(WidgetContent));
                BAT.Wrappers().Backend().PageTemplates().PageTemplateModifyScreen().PublishTemplate();

                // Verify on frontend
                RuntimeSettingsModificator.ExecuteWithClientTimeout(ClientTimeoutInterval, () => BAT.Macros().NavigateTo().CustomPage(PageUrl, false));
                Assert.IsFalse(BATFrontend.Wrappers().Backend().Widgets().WidgetsWrapper().IsElementByInnerTextPresent(WidgetContent));

                // Activate Feather
                BATFrontend.Wrappers().Backend().FrontendModule().FrontendModule().ActivateFeather(ActiveBrowser);
                featherDeactivated = false;
            }
            finally 
            {
                if (featherDeactivated)
                {
                    // Activate Feather if Test Failed
                    BATFrontend.Wrappers().Backend().FrontendModule().FrontendModule().ActivateFeather(ActiveBrowser);
                }
            }
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

        private readonly string PageUrl = "~/" + PageName.ToLower();

        private const string PageName = "Page_DeactivateFeatherAndDeleteWidgetFromPurePageTemplate";
        private const string PageTemplateName = "Template_DeactivateFeatherAndDeleteWidgetFromPurePageTemplate";

        private const int ClientTimeoutInterval = 80000;
        private const string PageTemplatesPageUrl = "~/sitefinity/Design/PageTemplates";
        private const string Placeholder = "Body";
        private const string WidgetName = "ModuleTestsWidget";
        private const string WidgetContent = "ca9af596-eaa3-44ed-a654-0e9170266a36";
    }
}
