using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Frontend
{
    /// <summary>
    /// Provides access to the WidgetsWrapper.
    /// </summary>
    public class WidgetsWrapperFacade
    {
        /// <summary>
        /// Creates an instance of GridWidgetsWrapper.
        /// </summary>
        /// <returns>Returns a new instance of the GridWidgets.</returns>
        public GridWidgets GridWidgets()
        {
            return new GridWidgets();
        } 
    }
}
