using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    /// <summary>
    /// Grid widgets map.
    /// </summary>
    public class GridWidgets : HtmlElementContainer
    {
        /// <summary>
        /// Initializes a new instance of the GridWidgets class.
        /// </summary>
        /// <param name="find">Find object for the current window.</param>
        public GridWidgets(Find find)
            : base(find)
        {
        }

        /// <summary>
        /// Gets the Bootstrap grid widgets.
        /// </summary>
        /// <value>Bootstrap grid widgets.</value>
        public HtmlSpan BootstrapGridWidget
        {
            get
            {
                return this.Get<HtmlSpan>("tagname=span", "InnerText=Bootstrap grid widgets");
            }
        }
    }
}
