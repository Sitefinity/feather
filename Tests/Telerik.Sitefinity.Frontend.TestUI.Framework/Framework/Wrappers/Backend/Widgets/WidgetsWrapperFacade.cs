using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Provides access to the WidgetsWrapper.
    /// </summary>
    public class WidgetsWrapperFacade
    {
        /// <summary>
        /// Creates an instance of WidgetsWrapper.
        /// </summary>
        /// <returns>Returns a new instance of the WidgetsWrapper.</returns>
        public WidgetsWrapper WidgetsWrapper()
        {
            return new WidgetsWrapper();
        }

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
