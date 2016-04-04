using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MbUnit.Framework;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.Mvc.StringResources;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.ControlTemplates;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Events;
using Telerik.Sitefinity.TestIntegration.Data.Content;
using Telerik.Sitefinity.TestIntegration.SDK;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.TestIntegration.Module
{
    /// <summary>
    /// This class contains tests for unloading of the Feather module.
    /// </summary>
    [TestFixture]
    [Category(TestCategories.MvcCore)]
    [Description("This class contains tests for unloading of the Feather module.")]
    public class ModuleUnloadTests
    {
        #region Application state

        /// <summary>
        /// Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.
        /// </summary>
        /// <remarks>
        /// Check_Res_RegisterResource_ShouldBeUndone is not needed - Registering of resources is actually registering types in the Object factory
        /// Check_RouteTable_Routes_MapRoute_ShouldBeUndone is not needed - Routes are in one collection checked by UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone
        /// </remarks>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the Sitefinity application changes it has done are undone.")]
        public void DeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation()
        {
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                FeatherServerOperations.FeatherModule().DeactivateFeather();
                this.CheckDeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation();
            }
            finally
            {
                FeatherServerOperations.FeatherModule().ActivateFeather();
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the Sitefinity application changes it has done are undone.
        /// </summary>
        /// <remarks>
        /// Check_Res_RegisterResource_ShouldBeUndone is not needed - Registering of resources is actually registering types in the Object factory
        /// Check_RouteTable_Routes_MapRoute_ShouldBeUndone is not needed - Routes are in one collection checked by UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone
        /// </remarks>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the Sitefinity application changes it has done are undone.")]
        public void UninstallingFeatherShouldRestoreAppStateToPreFeatherActivation()
        {
            FeatherServerOperations.FeatherModule().EnsureFeatherEnabled();

            try
            {
                FeatherServerOperations.FeatherModule().DeactivateFeather();
                FeatherServerOperations.FeatherModule().UninstallFeather();
                this.CheckDeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation();
            }
            finally
            {
                FeatherServerOperations.FeatherModule().InstallFeather();
            }
        }

        #endregion

        #region Widgets

        #region Page edit

        /// <summary>
        /// Checks whether after deactivating Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsOnHybridPage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Hybrid, out pageUrl);
                this.AddGridToPage(pageId, PageTemplateFramework.Hybrid);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                ServerOperations.Pages().DeletePage(pageId);
                moduleOperations.ActivateFeather();
            }
        }

        /// <summary>
        /// Checks whether after deactivating Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsOnPurePage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Mvc, out pageUrl);
                this.AddGridToPage(pageId, PageTemplateFramework.Mvc);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsOnHybridPage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Hybrid, out pageUrl);
                this.AddGridToPage(pageId, PageTemplateFramework.Hybrid);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.InstallFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsOnPurePage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Mvc, out pageUrl);
                this.AddGridToPage(pageId, PageTemplateFramework.Mvc);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.InstallFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        #endregion

        #region Template edit

        /// <summary>
        /// Checks whether after deactivating Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsOnHybridPageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreateHybridMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                this.AddGridToPageTemplate(templateId, PageTemplateFramework.Hybrid);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after deactivating Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsOnPurePageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreatePureMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                this.AddGridToPageTemplate(templateId, PageTemplateFramework.Mvc);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsOnHybridPageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreateHybridMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                this.AddGridToPageTemplate(templateId, PageTemplateFramework.Hybrid);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.InstallFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsOnPurePageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreatePureMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                this.AddGridToPageTemplate(templateId, PageTemplateFramework.Mvc);
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentAfterDeactivate.Contains(ModuleUnloadTests.GridUnavailableMessage));
            }
            finally
            {
                moduleOperations.InstallFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        #endregion

        #endregion

        #region Toolbox
        
        #region Page edit

        /// <summary>
        /// Checks whether after deactivating Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsInToolboxHybridPage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Hybrid, out pageUrl);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                ServerOperations.Pages().DeletePage(pageId);
                moduleOperations.ActivateFeather();
            }
        }

        /// <summary>
        /// Checks whether after deactivating Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsInToolboxPurePage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Mvc, out pageUrl);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsInToolboxHybridPage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Hybrid, out pageUrl);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                var configManager = ConfigManager.GetManager();
                var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();

                foreach (var toolbox in toolboxesConfig.Toolboxes.Values)
                {
                    foreach (var section in toolbox.Sections)
                    {
                        var existsFeatherWidgets = ((ICollection<ToolboxItem>)section.Tools)
                            .Any(i =>
                                i.ControlType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal));

                        Assert.IsFalse(existsFeatherWidgets, "Feather widget still exists in section " + section.Name);
                    }
                }
            }
            finally
            {
                moduleOperations.InstallFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page edit toolbox on pure page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsInToolboxPurePage_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            Guid pageId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                string pageUrl;
                pageId = this.CreatePage(PageTemplateFramework.Mvc, out pageUrl);

                var pageContentBeforeDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsTrue(pageContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var pageContentAfterDeactivate = this.ExecuteWebRequest(pageUrl + this.AppendEditUrl());
                Assert.IsFalse(pageContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.InstallFeather();
                ServerOperations.Pages().DeletePage(pageId);
            }
        }

        #endregion

        #region Template edit

        /// <summary>
        /// Checks whether after deactivating Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsInToolboxHybridPageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreateHybridMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after deactivating Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after deactivating Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.")]
        public void DeactivateFeather_GridWidgetsInToolboxPurePageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreatePureMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.ActivateFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page template edit toolbox on hybrid page doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsInToolboxHybridPageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreateHybridMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.InstallFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        /// <summary>
        /// Checks whether after uninstalling Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.
        /// </summary>
        [Test]
        [Author(FeatherTeams.FeatherTeam)]
        [Description("Checks whether after uninstalling Feather the page template edit toolbox on pure page template doesn't contain Feather grid widgets.")]
        public void UninstallFeather_GridWidgetsInToolboxPurePageTemplate_VerifyBackend()
        {
            var moduleOperations = FeatherServerOperations.FeatherModule();
            var templatesOperations = ServerOperations.Templates();
            Guid templateId = Guid.Empty;

            moduleOperations.EnsureFeatherEnabled();

            try
            {
                templateId = templatesOperations.CreatePureMVCPageTemplate(ModuleUnloadTests.PageTemplateTitle + Guid.NewGuid().ToString("N"));
                string templateUrl = UrlPath.ResolveAbsoluteUrl(ModuleUnloadTests.SitefinityTemplateRoutePrefix + templateId.ToString());

                var templateContentBeforeDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsTrue(templateContentBeforeDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));

                moduleOperations.DeactivateFeather();
                moduleOperations.UninstallFeather();

                var templateContentAfterDeactivate = this.ExecuteWebRequest(templateUrl + this.AppendUncacheUrl());
                Assert.IsFalse(templateContentAfterDeactivate.Contains(ModuleUnloadTests.FeatherGridToolboxItemMarkup));
            }
            finally
            {
                moduleOperations.InstallFeather();
                templatesOperations.DeletePageTemplate(templateId);
            }
        }

        #endregion

        #endregion
        
        #region Private members

        #region Checks

        /// <summary>
        /// Execute all the checks after feather is deactivated/uninstalled.
        /// </summary>
        private void CheckDeactivatingFeatherShouldRestoreAppStateToPreFeatherActivation()
        {
            this.Check_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone();
            this.Check_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone();
            this.Check_ObjectFactory_Container_RegisterType_ShouldBeUndone();
            this.Check_SystemManager_RegisterRoute_ShouldBeUndone();
            this.Check_EventHub_Subscribe_ShouldBeUndone();
            this.Check_IFileMonitor_Start_ShouldBeUndone();
            this.Check_GlobalFilters_Filters_Add_ShouldBeUndone();
            this.Check_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone();
            this.Check_ViewEngines_Engines_Remove_ShouldBeUndone();
            this.Check_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone();
            this.Check_ControllerStore_AddController_ShouldBeUndone();
            this.Check_RouteTable_Routes_Insert_ShouldBeUndone();
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the VirtualPathManager AddVirtualFileResolver calls are undone.
        /// </summary>
        /// <remarks>
        /// There are no changes on virtualPaths.
        /// </remarks>
        private void Check_VirtualPathManager_AddVirtualFileResolver_ShouldBeUndone()
        {
            var wildcardPaths = typeof(VirtualPathManager).GetField("wildcardPaths", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as IList<PathDefinition>;
            if (wildcardPaths != null)
            {
                Assert.IsFalse(wildcardPaths.Any(wp => wp.ResolverName == "MvcFormsResolver"));
                Assert.IsFalse(wildcardPaths.Any(wp => wp.ResolverName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
            }
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the SystemManager RegisterServiceStackPlugin calls are undone.
        /// </summary>
        private void Check_SystemManager_RegisterServiceStackPlugin_ShouldBeUndone()
        {
            var pendingServiceStackPlugins = typeof(SystemManager).GetField("PendingServiceStackPlugins", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as IEnumerable<object>;
            Assert.IsFalse(pendingServiceStackPlugins.Any(pssp => pssp.GetType().FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the ObjectFactory Container RegisterType calls are undone.
        /// </summary>
        private void Check_ObjectFactory_Container_RegisterType_ShouldBeUndone()
        {
            var frondendPostUninstallMappedToTypesNames = new List<string>() 
            { 
                typeof(FeatherEnabledToolboxFilter).FullName,
                typeof(GridControlToolboxFilter).FullName,
                typeof(PersonalizationDesignerResources).FullName,
                typeof(GridDesignerResources).FullName,
                typeof(DesignerResources).FullName
            };

            var frontendRegistrations = ObjectFactory.Container.Registrations.Where(r =>
                (r.RegisteredType != null && !string.IsNullOrEmpty(r.RegisteredType.FullName) && r.RegisteredType.FullName.Contains(ModuleUnloadTests.FrontendAssemblyPrefix)) ||
                (r.MappedToType != null && !string.IsNullOrEmpty(r.MappedToType.FullName) && r.MappedToType.FullName.Contains(ModuleUnloadTests.FrontendAssemblyPrefix)));

            var leftRegistrations = frontendRegistrations.Where(r => r.MappedToType != null && !string.IsNullOrEmpty(r.MappedToType.FullName) && !frondendPostUninstallMappedToTypesNames.Contains(r.MappedToType.FullName) && !r.MappedToType.FullName.StartsWith("Telerik.Sitefinity.Frontend.Test"));
            Assert.AreEqual(0, leftRegistrations.Count(), string.Join(Environment.NewLine, leftRegistrations.Select(r => string.Format("Frontend registration [Name: {0} MappedToType:{1}] left in the object factory", r.Name, r.MappedToType == null ? "null" : r.MappedToType.FullName))));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the SystemManager RegisterRoute calls are undone.
        /// </summary>
        private void Check_SystemManager_RegisterRoute_ShouldBeUndone()
        {
            // Routes are checked in RouteTable.Routes in UnloadingFeather_RouteTable_Routes_Insert_ShouldBeUndone
            var routeRegistrationModuleName = Type.GetType("Telerik.Sitefinity.Abstractions.RouteRegistration, Telerik.Sitefinity").GetProperty("ModuleName", BindingFlags.NonPublic | BindingFlags.Instance);
            var routeRegistrations = Type.GetType("Telerik.Sitefinity.Services.RouteManager, Telerik.Sitefinity").GetField("routeRegistrations", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as IEnumerable<object>;
            Assert.IsFalse(routeRegistrations.Any(r => (routeRegistrationModuleName.GetValue(r) as string).StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the EventHub Subscribe calls are undone.
        /// </summary>
        private void Check_EventHub_Subscribe_ShouldBeUndone()
        {
            var handlerLists = typeof(EventService).GetField("handlerLists", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(ObjectFactory.Resolve<IEventService>());
            var allHandlersValues = handlerLists.GetType().GetProperty("Values").GetValue(handlerLists) as IEnumerable<object>;
            var innerHandlersProperty = typeof(EventService).GetNestedType("HandlerList", BindingFlags.Instance | BindingFlags.NonPublic).GetField("handlers", BindingFlags.Instance | BindingFlags.NonPublic);
            var allHandlers = allHandlersValues.SelectMany(h => innerHandlersProperty.GetValue(h) as IList<Delegate>);
            Assert.IsFalse(allHandlers.Any(h => h.Method.ReflectedType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the IFileMonitor Start calls are undone.
        /// </summary>
        private void Check_IFileMonitor_Start_ShouldBeUndone()
        {
            var moduleInstance = SystemManager.GetModule("Feather");
            if (moduleInstance != null)
            {
                var initializers = typeof(FrontendModule).GetField("initializers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(moduleInstance) as Lazy<IEnumerable<IInitializer>>;
                var fileInitializer = initializers.Value.FirstOrDefault(i => i is FileMonitoringInitializer);
                var fileMonitorInstance = typeof(FileMonitoringInitializer).GetField("fileMonitor", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fileInitializer);
                Assert.IsNull(fileMonitorInstance);
            }
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the GlobalFilters Filters Add calls are undone.
        /// </summary>
        private void Check_GlobalFilters_Filters_Add_ShouldBeUndone()
        {
            Assert.IsFalse(GlobalFilters.Filters.Any(f => f.Instance is CacheDependentAttribute));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the ControlTemplates RegisterTemplatableControl calls are undone.
        /// </summary>
        private void Check_ControlTemplates_RegisterTemplatableControl_ShouldBeUndone()
        {
            var controlTemplates = typeof(ControlTemplates).GetField("controlTemplates", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, IControlTemplateInfo>;
            Assert.IsFalse(controlTemplates.Values.Any(cti => cti.ControlType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the ViewEngines Engines Remove calls are undone.
        /// </summary>
        private void Check_ViewEngines_Engines_Remove_ShouldBeUndone()
        {
            Assert.IsTrue(ViewEngines.Engines.Any(ve => ve is SitefinityViewEngine));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the ControllerBuilder Current SetControllerFactory calls are undone.
        /// </summary>
        private void Check_ControllerBuilder_Current_SetControllerFactory_ShouldBeUndone()
        {
            Assert.IsFalse(ControllerBuilder.Current.GetControllerFactory() is FrontendControllerFactory);
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the ControllerStore AddController calls are undone.
        /// </summary>
        private void Check_ControllerStore_AddController_ShouldBeUndone()
        {
            Assert.IsFalse(ControllerStore.Controllers().Any(c => c.ControllerType.FullName.StartsWith(ModuleUnloadTests.FrontendAssemblyPrefix, StringComparison.Ordinal)));
        }

        /// <summary>
        /// Checks whether after unloading/uninstalling Feather the RouteTable Routes Insert calls are undone.
        /// </summary>
        private void Check_RouteTable_Routes_Insert_ShouldBeUndone()
        {
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url == "Telerik.Sitefinity.Frontend/{controller}/Master/{widgetName}"));
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url == "Telerik.Sitefinity.Frontend/{controller}/View/{widgetName}/{viewType}"));
            Assert.IsFalse(RouteTable.Routes.Any(r => r is Route && ((Route)r).Url.Contains(ModuleUnloadTests.FrontendAssemblyPrefix)));
            Assert.IsFalse(RouteTable.Routes.Any(r => r.GetType().FullName == "System.Web.Mvc.Routing.RouteCollectionRoute" && ((IEnumerable<RouteBase>)r).Any(rb => rb is Route && ((Route)rb).Url == "rest-api/login-status")));
            Assert.IsFalse(RouteTable.Routes.Any(r => r.GetType().FullName == "System.Web.Mvc.Routing.LinkGenerationRoute" && ((Route)r).Url == "rest-api/login-status"));
        }

        #endregion

        #region Helpers

        private Guid CreatePage(PageTemplateFramework framework, out string pageUrl)
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

            return pageId;
        }

        private void AddGridToPage(Guid pageId, PageTemplateFramework framework, string gridVirtualPath = ModuleUnloadTests.GridVirtualPath)
        {
            var placeholder = framework == PageTemplateFramework.Hybrid ? "Body" : "Contentplaceholder1";
            var control = new GridControl() { Layout = gridVirtualPath };
            SampleUtilities.AddControlToPage(pageId, control, placeholder, "9 + 3");
        }

        private void AddGridToPageTemplate(Guid pageTemplateId, PageTemplateFramework framework, string gridVirtualPath = ModuleUnloadTests.GridVirtualPath)
        {
            var placeholder = framework == PageTemplateFramework.Hybrid ? "Body" : "Contentplaceholder1";
            var control = new GridControl() { Layout = gridVirtualPath };
            SampleUtilities.AddControlToTemplate(pageTemplateId, control, placeholder, "9 + 3");
        }

        private string ExecuteWebRequest(string url)
        {
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
        
        private string AppendEditUrl()
        {
            return "/Action/Edit" + this.AppendUncacheUrl();
        }

        private string AppendUncacheUrl()
        {
            return "?t=" + Guid.NewGuid().ToString();
        }

        #endregion
        
        #region Constants

        private const string FrontendAssemblyPrefix = "Telerik.Sitefinity.Frontend";
        private const string FeatherGridToolboxItemMarkup = "controltype=\"Telerik.Sitefinity.Frontend.GridSystem.GridControl, Telerik.Sitefinity.Frontend\"";
        private const string PageTemplateTitle = "TestPageTemplate";

        private const string SitefinityTemplateRoutePrefix = "~/Sitefinity/Template/";
        private const string GridUnavailableMessage = "This grid widget doesn't work, because Feather module has been deactivated.";
        private const string GridVirtualPath = "~/ResourcePackages/Bootstrap/GridSystem/Templates/grid-9+3.html";

        #endregion

        #endregion
    }
}
