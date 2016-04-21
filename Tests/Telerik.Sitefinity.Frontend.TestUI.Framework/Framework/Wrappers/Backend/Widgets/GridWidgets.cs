using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;

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

        /// <summary>
        /// Sets the custom CSS to grid widget.
        /// </summary>
        /// <param name="css">The CSS.</param>
        public void SetCustomCssToGridWidget(string css)
        {
            HtmlInputText input = this.EM.Widgets.GridWidgets.CustomBodyText
                                      .AssertIsPresent("input");

            input.ScrollToVisible();
            input.Focus();
            input.MouseClick();

            Manager.Current.Desktop.KeyBoard.KeyDown(System.Windows.Forms.Keys.Control);
            Manager.Current.Desktop.KeyBoard.KeyPress(System.Windows.Forms.Keys.A);
            Manager.Current.Desktop.KeyBoard.KeyUp(System.Windows.Forms.Keys.Control);
            Manager.Current.Desktop.KeyBoard.KeyPress(System.Windows.Forms.Keys.Delete);

            Manager.Current.Desktop.KeyBoard.TypeText(css);
        }

        /// <summary>
        /// Clicks the save button.
        /// </summary>
        public void ClickSaveButton()
        {
            HtmlButton saveButton = this.EM.Widgets.GridWidgets.SaveButton
                                        .AssertIsPresent("save button");

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }
    }
}
