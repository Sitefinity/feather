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
        }

        /// <summary>
        /// Uninstalls the specified initializers.
        /// </summary>
        /// <param name="initializers">The initializers.</param>
        public static void Uninstall(IEnumerable<IInitializer> initializers)
        {
            FrontendModuleUninstaller.Uninitialize(initializers);
        }

        // Called both by Unload and Uninstall
        private static void Uninitialize(IEnumerable<IInitializer> initializers)
        {
            foreach (var initializer in initializers)
                initializer.Uninitialize();

            // Force mvc initialization to run again after feather uninstalls
            typeof(SystemManager).GetField("mvcEnabled", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, false);
        }
    }
}
