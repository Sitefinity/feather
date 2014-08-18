using ArtOfTest.WebAii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    public class WidgetsMap
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetsMap" /> class.
        /// </summary>
        /// <param name="find">The find.</param>
        public WidgetsMap(Find find)
        {
            this.find = find;
        }

        /// <summary>
        /// Gets the feathe widget designer
        /// </summary>
        public FeatherWidgetDesigner FeatherWidget
        {
            get
            {
                return new FeatherWidgetDesigner(this.find);
            }
        }

        private Find find;
    }
}
