using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend.PageTemplates
{
    /// <summary>
    /// Provides wrappers for Page Templates.
    /// </summary>
    public class PageTemplatesWrapperFacade
    {
        /// <summary>
        /// Provides access to PageTemplateEditorWrapper.
        /// </summary>
        /// <returns>The PageTemplateEditorWrapper.</returns>
        public PageTemplateEditorWrapper PageTemplateEditor()
        {
            return new PageTemplateEditorWrapper();
        }
    }
}
