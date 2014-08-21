using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Provides access to Pages wrapper.
    /// </summary>
    public class PagesWrapperFacade
    {
        /// <summary>
        /// Creates an instance of PageZoneEditorWrapper for the Wrappers() facade.
        /// </summary>
        /// <returns>A new instance of the PageZoneEditorWrapper.</returns>
        public PageZoneEditorWrapper PageZoneEditorWrapper()
        {
            return new PageZoneEditorWrapper();
        }
    }
}
