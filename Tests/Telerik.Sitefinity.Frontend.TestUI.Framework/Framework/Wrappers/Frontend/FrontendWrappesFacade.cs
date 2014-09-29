using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Frontend
{
    /// <summary>
    /// Frontend wrapper facade.
    /// </summary>
    public class FrontendWrappesFacade
    {
        /// <summary>
        /// Provides access to the widgets wrapper.
        /// </summary>
        /// <returns>Widgets wrapper facade.</returns>
        public WidgetsWrapperFacade Widgets()
        {
            return new WidgetsWrapperFacade();
        }
    }
}
