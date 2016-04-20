using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets;
using Telerik.Sitefinity.TestUI.Framework.Wrappers.Backend.BaseWrappers.ContentItems;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Class for WidgetTemplatesCreateEditScreen
    /// </summary>
    /// <typeparam name="TScreen">The type of the T screen.</typeparam>
    public abstract class WidgetTemplatesCreateEditScreenWrapper<TScreen> : ContentItemEditBaseWrapper where TScreen : WidgetTemplatesCreateEditBaseScreen
    {
        /// <summary>
        /// Gets the ActiveWindowEM
        /// </summary>
        protected abstract TScreen ActiveWindowEM { get; }

        /// <summary>
        /// Selects the template.
        /// </summary>
        public void SelectTemplate(string name)
        {
            HtmlSelect selectDropdown = this.ActiveWindowEM.Find.ByExpression<HtmlSelect>("id=controlTemplateEditor_ctl00_ctl00_controlTypesList_ctl00_ctl00_dropDown");
            selectDropdown.SelectByText(name);
        }

        /// <summary>
        /// Enters the content in text area.
        /// </summary>
        /// <param name="text">The text.</param>
        public void EnterTextInTextArea(string text)
        {
            HtmlTextArea input = this.ActiveWindowEM.TextArea
                                     .AssertIsPresent("text area");
            input.Click();
            input.Text = text;
        }

        /// <summary>
        /// Enters the name of the widget template.
        /// </summary>
        /// <param name="name">The name.</param>
        public void EnterWidgetTemplateName(string name)
        {
            HtmlInputText input = this.ActiveWindowEM.TemplateNameTextBox
                                      .AssertIsPresent("name field");
            input.Text = name;
        }

        /// <summary>
        /// Creates the template.
        /// </summary>
        public void CreateThisTemplate()
        {
            HtmlAnchor createButton = this.ActiveWindowEM.CreateTemplateButton
                                          .AssertIsPresent("create button");

            createButton.Click();
            this.ActiveWindowEM.OwnerBrowser.WaitForAsyncOperations();
            this.ActiveWindowEM.OwnerBrowser.RefreshDomTree();
        }
    }
}