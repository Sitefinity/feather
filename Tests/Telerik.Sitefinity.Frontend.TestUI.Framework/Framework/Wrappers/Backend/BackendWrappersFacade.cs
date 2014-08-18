using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Backend wrapper facade.
    /// </summary>
    public class BackendWrappersFacade
    {
        /// <summary>
        /// Provides access to pages wrapper.
        /// </summary>
        /// <returns>Pages wrapper facade.</returns>
        public PagesWrapperFacade Pages()
        {
            return new PagesWrapperFacade();
        }

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
