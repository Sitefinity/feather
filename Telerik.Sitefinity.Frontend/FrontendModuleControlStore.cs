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
        /// Processes the pages with controls.
        /// </summary>
        /// <param name="pageManager">The page manager.</param>
        /// <param name="shouldDelete">if set to <c>true</c> [should delete].</param>
        public static void ProcessPagesWithControls(PageManager pageManager, bool shouldDelete)
        {
            List<ControlData> featherControls = new List<ControlData>();
            
            featherControls.AddRange(FrontendModuleControlStore.GetControlsToDelete(pageManager));

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

                if (shouldDelete)
                    pageManager.Delete(control);
            }

            foreach (var page in pagesToInvalidate.Distinct())
                page.BuildStamp++;
        }

        private static List<ControlData> GetControlsToDelete(PageManager pageManager)
        {
            List<ControlData> controlsToDelete;
            try
            {
                // Could fail if there are persisted records for not loaded types inherited from ControlData
                controlsToDelete = FrontendModuleControlStore.GetFeatherControlsToDelete<ControlData>(pageManager);
            }
            catch
            {
                controlsToDelete = new List<ControlData>();
                controlsToDelete.AddRange(FrontendModuleControlStore.GetFeatherControlsToDelete<PageControl>(pageManager));
                controlsToDelete.AddRange(FrontendModuleControlStore.GetFeatherControlsToDelete<PageDraftControl>(pageManager));
                controlsToDelete.AddRange(FrontendModuleControlStore.GetFeatherControlsToDelete<TemplateControl>(pageManager));
                controlsToDelete.AddRange(FrontendModuleControlStore.GetFeatherControlsToDelete<TemplateDraftControl>(pageManager));
            }

            return controlsToDelete;
        }

        private static List<ControlData> GetFeatherControlsToDelete<TControlData>(PageManager pageManager) where TControlData : ControlData
        {
            var controlsToDelete = new List<ControlData>();

            controlsToDelete.AddRange(pageManager.GetControls<TControlData>().Where(c => c.Properties.Any(p => p.Name == "ControllerName" && p.Value.StartsWith("Telerik.Sitefinity.Frontend"))).ToList());

            return controlsToDelete;
        }
    }
}
