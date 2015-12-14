using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Handles the logic for Feather related controls
    /// </summary>
    internal static class FrontendModuleControlStore
    {
        /// <summary>
        /// Invalidates the pages with controls.
        /// </summary>
        public static void InvalidatePagesWithControls()
        {
            IObjectScope objectScope;
            var manager = FrontendModuleControlStore.GetManager(out objectScope);
            var activeConnectionTimeout = objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout;
            objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout = FrontendModuleControlStore.ScopeTimeoutSeconds;

            try
            {
                FrontendModuleControlStore.InvalidatePageControls(manager);
                FrontendModuleControlStore.InvalidateTemplateControls(manager);
            }
            finally
            {
                objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout = activeConnectionTimeout;
            }
        }

        /// <summary>
        /// Deletes the pages with controls.
        /// </summary>
        public static void DeletePagesWithControls()
        {
            IObjectScope objectScope;
            var manager = FrontendModuleControlStore.GetManager(out objectScope);
            var activeConnectionTimeout = objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout;
            objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout = FrontendModuleControlStore.ScopeTimeoutSeconds;

            try
            {
                FrontendModuleControlStore.DeletePageControls(manager);
                FrontendModuleControlStore.DeleteTemplateControls(manager);
                FrontendModuleControlStore.DeletePageDraftControls(manager);
                FrontendModuleControlStore.DeleteTemplateDraftControls(manager);
            }
            finally
            {
                objectScope.Database.BackendConfiguration.ConnectionPool.ActiveConnectionTimeout = activeConnectionTimeout;
            }
        }

        private static PageManager GetManager(out IObjectScope objectScope)
        {
            var manager = PageManager.GetManager(null, Guid.NewGuid().ToString("N"));

            var provider = manager.Provider as IOpenAccessDataProvider;
            var context = provider.GetContext();

            objectScope = context.GetType().GetProperty("Scope", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(context) as IObjectScope;
            
            return manager;
        }

        private static void InvalidatePageControls(PageManager manager)
        {
            var iteration = 0;
            while (true)
            {
                var pages = manager
                    .GetPageDataList()
                    .Where(p => p.Controls.Any(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(p => p.Id)
                    .Skip(iteration * FrontendModuleControlStore.BufferSize)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (pages.Count > 0)
                {
                    foreach (var page in pages)
                    {
                        if (page != null)
                            page.BuildStamp++;
                    }

                    manager.SaveChanges();

                    if (pages.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        iteration++;
                        continue;
                    }
                }

                break;
            }
        }

        private static void InvalidateTemplateControls(PageManager manager)
        {
            var iteration = 0;
            while (true)
            {
                var templates = manager
                    .GetTemplates()
                    .Where(t => t.Controls.Any(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(t => t.Id)
                    .Skip(iteration * FrontendModuleControlStore.BufferSize)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (templates.Count > 0)
                {
                    foreach (var template in templates)
                    {
                        var pages = template.Pages().ToList();
                        foreach (var page in pages)
                        {
                            if (page != null)
                                page.BuildStamp++;
                        }
                    }

                    manager.SaveChanges();

                    if (templates.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        iteration++;
                        continue;
                    }
                }

                break;
            }
        }

        private static void DeletePageControls(PageManager manager)
        {
            while (true)
            {
                var pages = manager
                    .GetPageDataList()
                    .Where(p => p.Controls.Any(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(p => p.Id)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (pages.Count > 0)
                {
                    foreach (var page in pages)
                    {
                        if (page != null)
                            page.BuildStamp++;
                    }

                    var controls = pages
                            .SelectMany(p => p.Controls.Where(ctrl =>
                                ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                                ctrl.Properties.Any(prop =>
                                    prop.Name == FeatherControlPropertiesName &&
                                    prop.Value.StartsWith(FeatherControlPropertiesValue))))
                            .ToList();

                    foreach (var control in controls)
                    {
                        manager.Delete(control);
                    }

                    manager.SaveChanges();

                    if (pages.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        continue;
                    }
                }

                break;
            }
        }

        private static void DeleteTemplateControls(PageManager manager)
        {
            while (true)
            {
                var templates = manager
                    .GetTemplates()
                    .Where(t => t.Controls.Any(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(t => t.Id)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (templates.Count > 0)
                {
                    foreach (var template in templates)
                    {
                        var pages = template.Pages().ToList();
                        foreach (var page in pages)
                        {
                            if (page != null)
                                page.BuildStamp++;
                        }
                    }

                    var controls = templates
                            .SelectMany(p => p.Controls.Where(ctrl =>
                                ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                                ctrl.Properties.Any(prop =>
                                    prop.Name == FeatherControlPropertiesName &&
                                    prop.Value.StartsWith(FeatherControlPropertiesValue)))).ToList();

                    foreach (var control in controls)
                    {
                        manager.Delete(control);
                    }

                    manager.SaveChanges();

                    if (templates.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        continue;
                    }
                }

                break;
            }
        }

        private static void DeletePageDraftControls(PageManager manager)
        {
            while (true)
            {
                var drafts = manager
                    .GetDrafts<PageDraft>()
                    .SelectMany(d => d.Controls.Where(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(d => d.Id)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (drafts.Count > 0)
                {
                    foreach (var draft in drafts)
                    {
                        manager.Delete(draft);
                    }

                    manager.SaveChanges();

                    if (drafts.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        continue;
                    }
                }

                break;
            }
        }

        private static void DeleteTemplateDraftControls(PageManager manager)
        {
            while (true)
            {
                var drafts = manager
                    .GetDrafts<TemplateDraft>()
                    .SelectMany(t => t.Controls.Where(ctrl =>
                        ctrl.ObjectType.StartsWith(FeatherControlObjectType) ||
                        ctrl.Properties.Any(prop =>
                            prop.Name == FeatherControlPropertiesName &&
                            prop.Value.StartsWith(FeatherControlPropertiesValue))))
                    .OrderBy(t => t.Id)
                    .Take(FrontendModuleControlStore.BufferSize)
                    .ToList();

                if (drafts.Count > 0)
                {
                    foreach (var draft in drafts)
                    {
                        manager.Delete(draft);
                    }

                    manager.SaveChanges();

                    if (drafts.Count % FrontendModuleControlStore.BufferSize == 0)
                    {
                        continue;
                    }
                }

                break;
            }
        }

        private const int BufferSize = 200;
        private const int ScopeTimeoutSeconds = 300;
        private const string FeatherControlObjectType = "Telerik.Sitefinity.Frontend.GridSystem.GridControl";
        private const string FeatherControlPropertiesName = "ControllerName";
        private const string FeatherControlPropertiesValue = "Telerik.Sitefinity.Frontend";
    }
}
