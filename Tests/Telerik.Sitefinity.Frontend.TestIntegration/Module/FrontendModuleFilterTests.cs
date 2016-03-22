using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Web;
using System.Web.Mvc;
using MbUnit.Framework;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Proxy;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.SDK;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Module
{
    /// <summary>
    /// This class contains tests for FrontendModuleFilter.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for FrontendModuleFilter.")]
    public class FrontendModuleFilterTests
    {
        #region Pages

        /// <summary>
        /// Checks whether after deactivating Feather, a hybrid page with widget on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather, a hybrid page with widget on the frontend and show warning in the backend.")]
        public void DeactivateFeather_HybridPage_Verify()
        {
            Guid pageId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                pageId = this.CreatePageWithWidget(PageTemplateFramework.Hybrid, new DummyController(), out pageUrl);

                var backendPageContentBeforeDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeDeactivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;
                
                var backendPageContentAfterDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterDeactivate = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().ActivateFeather();
                featherActivated = true;

                var backendPageContentAfterActivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterActivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
            }
        }
        
        /// <summary>
        /// Checks whether after deactivating Feather, a pure page with widget on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather, a pure page with widget on the frontend and show warning in the backend.")]
        public void DeactivateFeather_PurePage_Verify()
        {
            Guid pageId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                pageId = this.CreatePageWithWidget(PageTemplateFramework.Mvc, new DummyController(), out pageUrl);

                var backendPageContentBeforeDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeDeactivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;

                var backendPageContentAfterDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterDeactivate = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().ActivateFeather();
                featherActivated = true;

                var backendPageContentAfterActivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterActivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather, a hybrid page with widget on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather, a hybrid page with widget on the frontend and show warning in the backend.")]
        public void UninstallFeather_HybridPage_Verify()
        {
            Guid pageId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                pageId = this.CreatePageWithWidget(PageTemplateFramework.Hybrid, new DummyController(), out pageUrl);

                var backendPageContentBeforeUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeUninstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().UninstallFeather();
                featherActivated = false;
                FeatherServerOperations.FeatherModule().UninstallFeather();

                var backendPageContentAfterUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterUninstall = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().InstallFeather();
                featherActivated = true;

                var backendPageContentAfterInstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterInstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather, a pure page with widget on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather, a pure page with widget on the frontend and show warning in the backend.")]
        public void UninstallFeather_PurePage_Verify()
        {
            Guid pageId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                pageId = this.CreatePageWithWidget(PageTemplateFramework.Mvc, new DummyController(), out pageUrl);

                var backendPageContentBeforeUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeUninstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().UninstallFeather();
                featherActivated = false;
                FeatherServerOperations.FeatherModule().UninstallFeather();

                var backendPageContentAfterUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterUninstall = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().InstallFeather();
                featherActivated = true;

                var backendPageContentAfterInstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterInstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        #endregion

        #region Templates

        /// <summary>
        /// Checks whether after deactivating Feather, a hybrid template with widget and page based on it hide on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather, a hybrid template with widget and page based on it hide on the frontend and show warning in the backend.")]
        public void DeactivateFeather_HybridPageTemplate_Verify()
        {
            Guid pageId = Guid.Empty;
            Guid templateId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                templateId = this.CreateTemplateWithWidgetAndBasePageOnIt(PageTemplateFramework.Hybrid, new DummyController(), out pageId, out pageUrl);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(FrontendModuleFilterTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentBeforeDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeDeactivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;

                var templateContentAfterDeactivate = this.GetContent(templateUrl);
                Assert.IsFalse(templateContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(templateContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterDeactivate = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().ActivateFeather();
                featherActivated = true;

                var templateContentAfterActivate = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterActivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterActivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
                ServerOperations.Templates().DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after deactivating Feather, a pure template with widget and page based on it hide on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather, a pure template with widget and page based on it hide on the frontend and show warning in the backend.")]
        public void DeactivateFeather_PurePageTemplate_Verify()
        {
            Guid pageId = Guid.Empty;
            Guid templateId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                templateId = this.CreateTemplateWithWidgetAndBasePageOnIt(PageTemplateFramework.Mvc, new DummyController(), out pageId, out pageUrl);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(FrontendModuleFilterTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentBeforeDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeDeactivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;

                var templateContentAfterDeactivate = this.GetContent(templateUrl);
                Assert.IsFalse(templateContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(templateContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterDeactivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterDeactivate = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterDeactivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().ActivateFeather();
                featherActivated = true;

                var templateContentAfterActivate = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterActivate = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterActivate = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterActivate.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
                ServerOperations.Templates().DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather, a hybrid template with widget and page based on it hide on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather, a hybrid template with widget and page based on it hide on the frontend and show warning in the backend.")]
        public void UninstallFeather_HybridPageTemplate_Verify()
        {
            Guid pageId = Guid.Empty;
            Guid templateId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                templateId = this.CreateTemplateWithWidgetAndBasePageOnIt(PageTemplateFramework.Hybrid, new DummyController(), out pageId, out pageUrl);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(FrontendModuleFilterTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeUninstall = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentBeforeUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeUninstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;
                FeatherServerOperations.FeatherModule().UninstallFeather();

                var templateContentAfterUninstall = this.GetContent(templateUrl);
                Assert.IsFalse(templateContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(templateContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterUninstall = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().InstallFeather();
                featherActivated = true;

                var templateContentAfterInstall = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterInstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterInstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
                ServerOperations.Templates().DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather, a pure template with widget and page based on it hide on the frontend and show warning in the backend.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather, a pure template with widget and page based on it hide on the frontend and show warning in the backend.")]
        public void UninstallFeather_PurePageTemplate_Verify()
        {
            Guid pageId = Guid.Empty;
            Guid templateId = Guid.Empty;

            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();
            var featherActivated = true;

            try
            {
                string pageUrl;
                templateId = this.CreateTemplateWithWidgetAndBasePageOnIt(PageTemplateFramework.Mvc, new DummyController(), out pageId, out pageUrl);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(FrontendModuleFilterTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeUninstall = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentBeforeUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentBeforeUninstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentBeforeUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().DeactivateFeather();
                featherActivated = false;
                FeatherServerOperations.FeatherModule().UninstallFeather();

                var templateContentAfterUninstall = this.GetContent(templateUrl);
                Assert.IsFalse(templateContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(templateContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterUninstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsFalse(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsTrue(backendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterUninstall = this.GetContent(pageUrl);
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterUninstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                FeatherServerOperations.FeatherModule().InstallFeather();
                featherActivated = true;

                var templateContentAfterInstall = this.GetContent(templateUrl);
                Assert.IsTrue(templateContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(templateContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var backendPageContentAfterInstall = this.GetContent(pageUrl, openInEdit: true);
                Assert.IsTrue(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(backendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));

                var frontendPageContentAfterInstall = this.GetContent(pageUrl);
                Assert.IsTrue(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.DummyController.ResponseString));
                Assert.IsFalse(frontendPageContentAfterInstall.Contains(FrontendModuleFilterTests.FeatherModuleUnavailableMessage));
            }
            finally
            {
                if (!featherActivated)
                    FeatherServerOperations.FeatherModule().ActivateFeather();

                ServerOperations.Pages().DeletePage(pageId);
                ServerOperations.Templates().DeletePageTemplate(templateId);
            }
        }

        #endregion

        #region Helpers

        private Guid CreatePageWithWidget(PageTemplateFramework framework, Controller widgetController, out string pageUrl)
        {
            Guid pageId = Guid.Empty;
            pageUrl = string.Empty;
            var suffix = Guid.NewGuid().ToString("N");

            if (framework == PageTemplateFramework.Hybrid)
            {
                var namePrefix = "TestPageName";
                var titlePrefix = "TestPageTitle";
                var urlPrefix = "test-page-url";
                var index = 1;

                pageId = new PageContentGenerator().CreatePage(
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", namePrefix + suffix, index.ToString(CultureInfo.InvariantCulture)),
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", titlePrefix + suffix, index.ToString(CultureInfo.InvariantCulture)),
                    string.Format(CultureInfo.InvariantCulture, "{0}{1}", urlPrefix + suffix, index.ToString(CultureInfo.InvariantCulture)));

                pageUrl = UrlPath.ResolveAbsoluteUrl("~/" + urlPrefix + suffix + index);
            }
            else if (framework == PageTemplateFramework.Mvc)
            {
                var pagesOperations = FeatherServerOperations.Pages();
                var pageManager = PageManager.GetManager();

                var bootstrapTemplate = pageManager.GetTemplates().FirstOrDefault(t => (t.Name == "Bootstrap.default" && t.Title == "default") || t.Title == "Bootstrap.default");
                if (bootstrapTemplate == null)
                    throw new ArgumentException("Bootstrap template not found");

                pageId = pagesOperations.CreatePageWithTemplate(bootstrapTemplate, "FormsPageBootstrap" + suffix, "forms-page-bootstrap" + suffix);
                pageUrl = RouteHelper.GetAbsoluteUrl(pageManager.GetPageNode(pageId).GetFullUrl());
            }

            var placeholder = framework == PageTemplateFramework.Hybrid ? "Body" : "Contentplaceholder1";
            FeatherServerOperations.Pages().AddMvcWidgetToPage(pageId, widgetController.GetType().FullName, "FrontendModuleFilterTestsWidgetCaption", placeholder);

            return pageId;
        }

        private Guid CreateTemplateWithWidgetAndBasePageOnIt(PageTemplateFramework framework, Controller widgetController, out Guid pageId, out string pageUrl)
        {
            var pageManager = PageManager.GetManager();

            // Create template
            var templateId = framework == PageTemplateFramework.Hybrid ?
                ServerOperations.Templates().CreateHybridMVCPageTemplate(FrontendModuleFilterTests.TemplateTitle + Guid.NewGuid().ToString("N")) :
                ServerOperations.Templates().CreatePureMVCPageTemplate(FrontendModuleFilterTests.TemplateTitle + Guid.NewGuid().ToString("N"));

            // Place widget on template
            var mvcProxy = new MvcControllerProxy();
            mvcProxy.ControllerName = widgetController.GetType().FullName;
            mvcProxy.Settings = new ControllerSettings(widgetController);
            SampleUtilities.AddControlToTemplate(templateId, mvcProxy, "Body", "FrontendModuleFilterTestsWidgetCaption");

            // Create page with template
            var template = pageManager.GetTemplates().Where(t => t.Id == templateId).SingleOrDefault();
            pageId = FeatherServerOperations.Pages().CreatePageWithTemplate(template, "TestPageName", "test-page-url");
            var page = pageManager.GetPageNode(pageId);
            pageUrl = RouteHelper.GetAbsoluteUrl(page.GetFullUrl());

            return templateId;
        }

        private string GetContent(string url, bool openInEdit = false)
        {
            url += openInEdit ? "/Action/Edit?t=" + Guid.NewGuid().ToString() : "?t=" + Guid.NewGuid().ToString();

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Timeout = 120 * 1000;
            webRequest.CookieContainer = new CookieContainer();
            webRequest.Headers["Authorization"] = HttpContext.Current.Request.Headers["Authorization"];
            webRequest.CachePolicy = new RequestCachePolicy();
            var webResponse = (HttpWebResponse)webRequest.GetResponse();

            string responseContent;
            using (var sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8))
                responseContent = sr.ReadToEnd();

            return responseContent;
        }

        #endregion

        #region Private fields

        private const string SitefinityTemplateRoutePrefix = "~/Sitefinity/Template/";
        private const string FeatherModuleUnavailableMessage = "This widget doesn't work, because <strong>Feather</strong> module has been deactivated.";
        private const string TemplateTitle = "TestPageTemplate";

        private class DummyController : Controller
        {
            public ActionResult Index()
            {
                return this.Content(DummyController.ResponseString);
            }

            public const string ResponseString = "5f2e5db3-4929-4b47-8f5c-9ff71c63e8a5";
        }

        #endregion
    }
}
