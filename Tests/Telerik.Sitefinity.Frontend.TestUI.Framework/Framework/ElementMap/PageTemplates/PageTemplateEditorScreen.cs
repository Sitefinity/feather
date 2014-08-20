using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.PageTemplates
{
    /// <summary>
    /// Page Template Editor Screen Elements.
    /// </summary>
    public class PageTemplateEditorScreen : HtmlElementContainer
    {
        /// <summary>
        /// Initializes a new instance of the PageTemplateEditorScreen class.
        /// </summary>
        /// <param name="find">Find object for the current window.</param>
        public PageTemplateEditorScreen(Find find)
            : base(find)
        {
        }

        /// <summary>
        /// Gets the template main container from template editor.
        /// </summary>
        /// <value>Template container.</value>
        public HtmlDiv TemplateContainer
        {
            get
            {
                return this.Get<HtmlDiv>("id=sfPageContainer");
            }
        }
    }
}
