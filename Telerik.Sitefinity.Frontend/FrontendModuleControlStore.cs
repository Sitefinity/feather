using System;
using System.Collections.Generic;
using System.Linq;
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
            FrontendModuleControlStore.ProcessPageControls(delete: false);
            FrontendModuleControlStore.ProcessTemplateControls(delete: false);
            FrontendModuleControlStore.ProcessPageDraftControls(delete: false);
            FrontendModuleControlStore.ProcessTemplateDraftControls(delete: false);
        }

        /// <summary>
        /// Deletes the pages with controls.
        /// </summary>
        public static void DeletePagesWithControls()
        {
            FrontendModuleControlStore.ProcessPageControls(delete: true);
            FrontendModuleControlStore.ProcessTemplateControls(delete: true);
            FrontendModuleControlStore.ProcessPageDraftControls(delete: true);
            FrontendModuleControlStore.ProcessTemplateDraftControls(delete: true);
        }

        private static void ProcessPageControls(bool delete)
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
                    .ToList();

                if (pages.Count > 0)
                {
                    foreach (var page in pages)
                    {
                        if (page != null)
                            page.BuildStamp++;
                    }

                    if (delete)
                    {
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

        private static void ProcessTemplateControls(bool delete)
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

                    if (delete)
                    {
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

        private static void ProcessPageDraftControls(bool delete)
        {
            // We process page draft controls only by deleting them
            if (!delete)
                return;

            var manager = PageManager.GetManager();
            var iteration = 0;
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
                    .Skip(iteration * FrontendModuleControlStore.BufferSize)
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
                        iteration++;
                        continue;
                    }
                }

                break;
            }
        }

        private static void ProcessTemplateDraftControls(bool delete)
        {
            // We process page draft controls only by deleting them
            if (!delete)
                return;

            var manager = PageManager.GetManager();
            var iteration = 0;
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
                    .Skip(iteration * FrontendModuleControlStore.BufferSize)
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
                        iteration++;
                        continue;
                    }
                }

                break;
            }
        }

        private const int BufferSize = 200;
        private const string FeatherControlObjectType = "Telerik.Sitefinity.Frontend.GridSystem.GridControl";
        private const string FeatherControlPropertiesName = "ControllerName";
        private const string FeatherControlPropertiesValue = "Telerik.Sitefinity.Frontend";
    }
}
