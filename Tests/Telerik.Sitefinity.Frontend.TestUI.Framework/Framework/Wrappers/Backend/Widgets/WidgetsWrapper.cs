using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Widgets base actions. 
    /// </summary>
    public class WidgetsWrapper : BaseWrapper
    {
        /// <summary>
        /// Verifies that feather widget has the proper title.
        /// </summary>
        /// <param name="title">The widget title.</param>
        public void VerifyWidgetTitle(string title)
        {
            HtmlControl widgetTitleText = this.EM.Widgets.FeatherWidget.WidgetTitleText
                .AssertIsPresent("widget title text");

            widgetTitleText.AssertTextContentIsEqualTo(title, "widget title is not as expected");
        }

        /// <summary>
        /// Verifies that the feather widget designer has the proper label text.
        /// </summary>
        /// <param name="text">The label text.</param>
        public void VerifyWidgetInputFieldLabelText(string text)
        {
            HtmlControl label = this.EM.Widgets.FeatherWidget.Label
                .AssertIsPresent("label");

            label.AssertTextContentIsEqualTo(text, "Label is incorrect");
        }

        /// <summary>
        /// Verifies that input text field is present  for the MVC Dummy widget. 
        /// </summary>
        public void VerifyDummyWidgetInputTextField()
        {
            HtmlInputText inputTextField = this.EM.Widgets.FeatherWidget.DummyWidgetInput
                .AssertIsPresent("Input Text Field");
        }

        /// <summary>
        /// Verifies that the save button is present in the designer.
        /// </summary>
        public void VerifyWidgetSaveButton()
        {
            HtmlButton saveButton = this.EM.Widgets.FeatherWidget.SaveButton
                .AssertIsPresent("save button");

            Assert.IsTrue(saveButton.InnerText.Equals("Save"), "Save button text is not correct");
        }

        /// <summary>
        /// Verifies that the Cancel button is present in the designer.
        /// </summary>
        public void VerifyWidgetCancelButton()
        {
            HtmlAnchor cancelButton = this.EM.Widgets.FeatherWidget.CancelButton
                .AssertIsPresent("Cancel button");

            Assert.IsTrue(cancelButton.InnerText.Equals("Cancel"), "Cancel button text is not correct");
        }

        /// <summary>
        /// Verifies that the close button is present in the designer.
        /// </summary>
        public void VerifyWidgetCloseButton()
        {
            HtmlButton closeButton = this.EM.Widgets.FeatherWidget.CloseButton
                .AssertIsPresent("close button");
        }

        /// <summary>
        /// Adds text to the dummy MVC widget input text field.
        /// </summary>
        /// <param name="text">The text message.</param>
        public void SetTextDummyWidget(string text)
        {
            HtmlInputText input = this.EM.Widgets.FeatherWidget.DummyWidgetInput
                .AssertIsPresent("input");

            input.ScrollToVisible();
            input.Focus();
            input.MouseClick();

            Manager.Current.Desktop.KeyBoard.TypeText(text);
        }

        /// <summary>
        /// Clicks the save button.
        /// </summary>
        public void ClickSaveButton()
        {
            HtmlButton saveButton = this.EM.Widgets.FeatherWidget.SaveButton
                .AssertIsPresent("save button");

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
        }
    }
}
