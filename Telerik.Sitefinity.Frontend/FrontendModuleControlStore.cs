using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            FrontendModuleControlStore.ProcessPageControls(deleteControls: false);
            FrontendModuleControlStore.ProcessTemplateControl(deleteControls: false);
            FrontendModuleControlStore.ProcessPageDraftControls(deleteControls: false);
            FrontendModuleControlStore.ProcessTemplateDraftControls(deleteControls: false);
        }

        /// <summary>
        /// Deletes the pages with controls.
        /// </summary>
        public static void DeletePagesWithControls()
        {
            FrontendModuleControlStore.ProcessPageControls(deleteControls: true);
            FrontendModuleControlStore.ProcessTemplateControl(deleteControls: true);
            FrontendModuleControlStore.ProcessPageDraftControls(deleteControls: true);
            FrontendModuleControlStore.ProcessTemplateDraftControls(deleteControls: true);
        }

        private static void ProcessPageControls(bool deleteControls)
        {
            const int BufferSize = 200;

            var manager = new PageManager();

            var iteration = 0;
            while (true)
            {
                var range = manager.GetControls<PageControl>()
                    .Where(c =>
                        c.ObjectType.StartsWith("Telerik.Sitefinity.Frontend.GridSystem.GridControl") ||
                        c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend")))
                    .OrderBy(c => c.Id)
                    .Skip(iteration * BufferSize)
                    .Take(BufferSize)
                    .ToList();

                foreach (var control in range)
                {
                    if (control.Page != null)
                        control.Page.BuildStamp++;

                    if (deleteControls)
                        manager.Delete(control);
                }

                manager.SaveChanges();

                if (range.Count == 0 || range.Count % BufferSize != 0)
                    break;

                iteration++;
            }
        }

        private static void ProcessTemplateControl(bool deleteControls)
        {
            const int BufferSize = 200;

            var manager = new PageManager();

            var iteration = 0;
            while (true)
            {
                var range = manager.GetControls<TemplateControl>()
                    .Where(c =>
                        c.ObjectType.StartsWith("Telerik.Sitefinity.Frontend.GridSystem.GridControl") ||
                        c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend")))
                    .OrderBy(c => c.Id)
                    .Skip(iteration * BufferSize)
                    .Take(BufferSize)
                    .ToList();

                foreach (var control in range)
                {
                    if (control.Page != null)
                    {
                        foreach (var page in control.Page.Pages())
                        {
                            page.BuildStamp++;
                        }
                    }

                    if (deleteControls)
                        manager.Delete(control);
                }

                manager.SaveChanges();

                if (range.Count == 0 || range.Count % BufferSize != 0)
                    break;

                iteration++;
            }
        }

        private static void ProcessPageDraftControls(bool deleteControls)
        {
            // We only process page draft controls by deleting them
            if (!deleteControls)
                return;

            const int BufferSize = 200;

            var manager = new PageManager();

            var iteration = 0;
            while (true)
            {
                var range = manager.GetControls<PageDraftControl>()
                    .Where(c =>
                        c.ObjectType.StartsWith("Telerik.Sitefinity.Frontend.GridSystem.GridControl") ||
                        c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend")))
                    .OrderBy(c => c.Id)
                    .Skip(iteration * BufferSize)
                    .Take(BufferSize)
                    .ToList();

                foreach (var control in range)
                {
                    manager.Delete(control);
                }

                manager.SaveChanges();

                if (range.Count == 0 || range.Count % BufferSize != 0)
                    break;

                iteration++;
            }
        }

        private static void ProcessTemplateDraftControls(bool deleteControls)
        {
            // We only process page draft controls by deleting them
            if (!deleteControls)
                return;

            const int BufferSize = 200;

            var manager = new PageManager();

            var iteration = 0;
            while (true)
            {
                var range = manager.GetControls<TemplateDraftControl>()
                    .Where(c =>
                        c.ObjectType.StartsWith("Telerik.Sitefinity.Frontend.GridSystem.GridControl") ||
                        c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend")))
                    .OrderBy(c => c.Id)
                    .Skip(iteration * BufferSize)
                    .Take(BufferSize)
                    .ToList();

                foreach (var control in range)
                {
                    manager.Delete(control);
                }

                manager.SaveChanges();

                if (range.Count == 0 || range.Count % BufferSize != 0)
                    break;

                iteration++;
            }
        }
    }
}
