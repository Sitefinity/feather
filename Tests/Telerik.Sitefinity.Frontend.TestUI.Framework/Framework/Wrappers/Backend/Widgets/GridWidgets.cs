using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Grid widgets base actions. 
    /// </summary>
    public class GridWidgets : BaseWrapper
    {
        /// <summary>
        /// Clicks Bootstrap grid widget button.
        /// </summary>
        public void ClickBootstrapGridWidgetButton()
        {
            HtmlSpan gridButton = this.EM.Widgets.GridWidgets.BootstrapGridWidget
                                        .AssertIsPresent("Bootstrap grid widget button");
            gridButton.ScrollToVisible();
            gridButton.Focus();
            gridButton.MouseClick();
        }
    }
}
