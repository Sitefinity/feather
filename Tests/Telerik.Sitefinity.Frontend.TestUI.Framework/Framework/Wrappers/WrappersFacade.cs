using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend;
using Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Frontend;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers
{
    /// <summary>
    /// Wrappers facade.
    /// </summary>
    public class WrappersFacade
    {
        /// <summary>
        /// Entry point to the backend wrappers.
        /// </summary>
        /// <returns>BackendWrappersFacade instance.</returns>
        public BackendWrappersFacade Backend()
        {
            return new BackendWrappersFacade();
        }

        /// <summary>
        /// Entry point to the frontend wrappers.
        /// </summary>
        /// <returns>FrontendWrappesFacade instance.</returns>
        public FrontendWrappesFacade Frontend()
        {
            return new FrontendWrappesFacade();
        }
    }
}
