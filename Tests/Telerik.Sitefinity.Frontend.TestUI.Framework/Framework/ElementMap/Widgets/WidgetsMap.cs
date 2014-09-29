using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    /// <summary>
    /// Feather widgets map.
    /// </summary>
    public class WidgetsMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetsMap" /> class.
        /// </summary>
        /// <param name="find">Find object for the current window.</param>
        public WidgetsMap(Find find)
        {
            this.find = find;
        }

        /// <summary>
        /// Gets the feather widget designer.
        /// </summary>
        /// <value>The feather widget designer.</value> 
        public FeatherWidgetDesigner FeatherWidget
        {
            get
            {
                return new FeatherWidgetDesigner(this.find);
            }
        }

        /// <summary>
        /// Gets the grid widget designer.
        /// </summary>
        /// <value>The feather grid designer.</value> 
        public GridWidgets GridWidgets
        {
            get
            {
                return new GridWidgets(this.find);
            }
        }

        private Find find;
    }
}
