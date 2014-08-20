using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.PageTemplates
{
    /// <summary>
    /// Page Templates Map.
    /// </summary>
    public class PageTemplatesMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageTemplatesMap" /> class.
        /// </summary>
        /// <param name="find">Find object for the current window.</param>
        public PageTemplatesMap(Find find)
        {
            this.find = find;
        }

        /// <summary>
        /// Gets the page template editor.
        /// </summary>
        /// <value>The page template editor.</value> 
        public PageTemplateEditorScreen PageTemplateEditor
        {
            get
            {
                return new PageTemplateEditorScreen(this.find);
            }
        }

        private Find find;
    }
}
