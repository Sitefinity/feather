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
        /// <param name="pageManager">The page manager.</param>
        public static void InvalidatePagesWithControls(PageManager pageManager = null)
        {
            var handleTransaction = false;
            if (pageManager == null)
            {
                handleTransaction = true;
                pageManager = PageManager.GetManager();
            }

            FrontendModuleControlStore.ProcessPagesWithControls(pageManager, deleteControls: false);

            if (handleTransaction)
                pageManager.SaveChanges();
        }

        /// <summary>
        /// Deletes the pages with controls.
        /// </summary>
        /// <param name="pageManager">The page manager.</param>
        public static void DeletePagesWithControls(PageManager pageManager = null)
        {
            var handleTransaction = false;
            if (pageManager == null)
            {
                handleTransaction = true;
                pageManager = PageManager.GetManager();
            }

            FrontendModuleControlStore.ProcessPagesWithControls(pageManager, deleteControls: true);

            if (handleTransaction)
                pageManager.SaveChanges();
        }

        private static void ProcessPagesWithControls(PageManager pageManager, bool deleteControls)
        {
            List<ControlData> featherControls = new List<ControlData>();
            
            featherControls.AddRange(FrontendModuleControlStore.GetControlsToProcess(pageManager));

            List<PageData> pagesToInvalidate = new List<PageData>();
            foreach (var control in featherControls)
            {
                if (control is PageControl)
                {
                    var pageForInvalidation = ((PageControl)control).Page;
                    if (pageForInvalidation != null)
                        pagesToInvalidate.Add(pageForInvalidation);
                }
                else if (control is TemplateControl)
                {
                    pagesToInvalidate.AddRange(((TemplateControl)control).Page.Pages());
                }

                if (deleteControls)
                    pageManager.Delete(control);
            }

            foreach (var page in pagesToInvalidate.Distinct())
                page.BuildStamp++;
        }
                
        private static List<ControlData> GetControlsToProcess(PageManager pageManager)
        {
            List<ControlData> controlsToProcess;
            try
            {
                // Could fail if there are persisted records for not loaded types inherited from ControlData
                controlsToProcess = FrontendModuleControlStore.GetFeatherControlsToProcess<ControlData>(pageManager);
            }
            catch
            {
                controlsToProcess = new List<ControlData>();
                controlsToProcess.AddRange(FrontendModuleControlStore.GetFeatherControlsToProcess<PageControl>(pageManager));
                controlsToProcess.AddRange(FrontendModuleControlStore.GetFeatherControlsToProcess<PageDraftControl>(pageManager));
                controlsToProcess.AddRange(FrontendModuleControlStore.GetFeatherControlsToProcess<TemplateControl>(pageManager));
                controlsToProcess.AddRange(FrontendModuleControlStore.GetFeatherControlsToProcess<TemplateDraftControl>(pageManager));
            }

            return controlsToProcess;
        }

        private static List<ControlData> GetFeatherControlsToProcess<TControlData>(PageManager pageManager) where TControlData : ControlData
        {
            var controlsToProcess = new List<ControlData>();

            controlsToProcess.AddRange(
                pageManager.GetControls<TControlData>()
                .Where(c => c.ObjectType.StartsWith("Telerik.Sitefinity.Frontend.GridSystem.GridControl") || 
                    c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend")))
                .ToList());

            return controlsToProcess;
        }
    }
}
