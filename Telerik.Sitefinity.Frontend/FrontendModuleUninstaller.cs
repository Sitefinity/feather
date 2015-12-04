using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Configuration;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Handles the logic for Feather unload/uninstall
    /// </summary>
    internal static class FrontendModuleUninstaller
    {
        /// <summary>
        /// Unloads the specified initializers.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        public static void Unload(IEnumerable<IInitializer> initializers)
        {
            FrontendModuleUninstaller.Uninitialize(initializers);

            Bootstrapper.Initialized -= FrontendModuleUninstaller.Bootstrapper_Initialized;
            Bootstrapper.Initialized += FrontendModuleUninstaller.Bootstrapper_Initialized;
        }

        /// <summary>
        /// Uninstalls the specified initializers.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        /// <param name="initializer">The initializer.</param>
        public static void Uninstall(IEnumerable<IInitializer> initializers, SiteInitializer initializer)
        {
            FrontendModuleUninstaller.Uninitialize(initializers);

            var featherWidgetTypes = new List<string>();
            var configManager = ConfigManager.GetManager();
            var toolboxesConfig = configManager.GetSection<ToolboxesConfig>();

            foreach (var toolbox in toolboxesConfig.Toolboxes.Values)
            {
                ICollection<ToolboxSection> emptySections = new List<ToolboxSection>();
                foreach (var section in toolbox.Sections)
                {
                    var featherWidgets = ((ICollection<ToolboxItem>)section.Tools)
                        .Where(i =>
                            i.ControlType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal) ||
                            (!i.ControllerType.IsNullOrEmpty() && i.ControllerType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal)));
                    featherWidgetTypes.AddRange(featherWidgets.Select(t => t.ControllerType));

                    var mvcToolsToDelete = featherWidgets.Select(i => i.GetKey());
                    foreach (var key in mvcToolsToDelete)
                    {
                        section.Tools.Remove(section.Tools.Elements.SingleOrDefault(e => e.GetKey() == key));
                    }

                    if (section.Tools.Count == 0)
                        emptySections.Add(section);
                }

                foreach (var emptySection in emptySections)
                {
                    toolbox.Sections.Remove(emptySection);
                }
            }

            // Delete widgets from pages
            FrontendModuleControlStore.DeletePagesWithControls(initializer.PageManager);

            configManager.SaveSection(toolboxesConfig);
        }

        // Called both by Unload and Uninstall
        private static void Uninitialize(IEnumerable<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
                initializer.Uninitialize();

            // Force mvc initialization to run again after feather uninstalls
            typeof(SystemManager).GetField("mvcEnabled", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
        }

        private static void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                FrontendModuleControlStore.InvalidatePagesWithControls(null);

                Bootstrapper.Initialized -= FrontendModuleUninstaller.Bootstrapper_Initialized;
            }
        }
    }
}
