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
            FrontendModuleControlStore.InvalidatePageControls();
            FrontendModuleControlStore.InvalidateTemplateControls();
        }

        /// <summary>
        /// Deletes the pages with controls.
        /// </summary>
        public static void DeletePagesWithControls()
        {
            FrontendModuleControlStore.DeletePageControls();
            FrontendModuleControlStore.DeleteTemplateControls();
            FrontendModuleControlStore.DeletePageDraftControls();
            FrontendModuleControlStore.DeleteTemplateDraftControls();
        }
        
        private static void InvalidatePageControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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

        private static void InvalidateTemplateControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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

        private static void DeletePageControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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

        private static void DeleteTemplateControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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

        private static void DeletePageDraftControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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

        private static void DeleteTemplateDraftControls()
        {
            var manager = PageManager.GetManager();
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
                    .WithOption(new QueryOptions() { CommandTimeout = FrontendModuleControlStore.CommandTimeoutSeconds })
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
        private const int CommandTimeoutSeconds = 100;
        private const string FeatherControlObjectType = "Telerik.Sitefinity.Frontend.GridSystem.GridControl";
        private const string FeatherControlPropertiesName = "ControllerName";
        private const string FeatherControlPropertiesValue = "Telerik.Sitefinity.Frontend";
    }
}
