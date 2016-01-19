using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Framework.Wrappers.Backend.Modules
{
    /// <summary>
    /// Provides wrappers for Feather Module.
    /// </summary>
    public class FrontendModuleWrapperFacade
    {
        /// <summary>
        /// Frontends the module.
        /// </summary>
        /// <returns>FrontendModuleWrapper.</returns>
        public FrontendModuleWrapper FrontendModule()
        {
            return new FrontendModuleWrapper();
        }
    }
}
