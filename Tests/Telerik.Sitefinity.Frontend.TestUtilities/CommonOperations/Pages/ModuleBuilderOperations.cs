using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.Pages
{
    public class ModuleBuilderOperations
    {       
        /// <summary>
        /// Ensures that all custom dynamic modules are imported.
        /// </summary>
        /// <param name="modules">The modules to import.</param>
        public void EnsureModuleIsImported(Dictionary<string, string> modules)
        {
            var modulesToImport = modules;
            bool restartApplication = false;
            foreach (var module in modulesToImport)
            {
                string moduleName = module.Key;
                string moduleResource = module.Value;

                if (!ServerOperations.ModuleBuilder().IsModulePresent(moduleName))
                {
                    restartApplication = true;
                    var testsArrangementsAssembly = this.GetArrangementsAssembly();
                    using (Stream moduleStream = testsArrangementsAssembly.GetManifestResourceStream(moduleResource))
                    {
                        ServerOperations.ModuleBuilder().ImportModule(moduleStream);
                        ServerOperations.ModuleBuilder().ActivateModule(moduleName, string.Empty, "Module Installations", false);
                    }
                }
                else if (!ServerOperations.ModuleBuilder().IsModuleActive(moduleName))
                {
                    restartApplication = true;
                    ServerOperations.ModuleBuilder().ActivateModule(moduleName, string.Empty, "Module Installations", false);
                }
            }

            if (restartApplication)
            {
                ServerOperations.SystemManager().RestartApplication(false);
            }
        }

        internal Assembly GetArrangementsAssembly()
        {
            var testsArrangementsAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Equals("Telerik.Sitefinity.Frontend.TestUtilities")).FirstOrDefault();
            if (testsArrangementsAssembly == null)
            {
                throw new DllNotFoundException("Arrangements assembly wasn't found");
            }

            return testsArrangementsAssembly;
        }
    }
}
