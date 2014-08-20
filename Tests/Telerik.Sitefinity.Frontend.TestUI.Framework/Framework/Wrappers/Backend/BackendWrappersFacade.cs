using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend.PageTemplates;

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

        /// <summary>
        /// Provides access to the Page Templates Wrapper.
        /// </summary>
        /// <returns>Page Templates wrapper facade.</returns>
        public PageTemplatesWrapperFacade PageTemplates()
        {
            return new PageTemplatesWrapperFacade();
        }
    }
}
