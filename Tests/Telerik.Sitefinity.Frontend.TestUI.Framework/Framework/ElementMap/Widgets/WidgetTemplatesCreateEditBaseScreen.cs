using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using Telerik.TestUI.Core.ElementMap;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    /// <summary>
    /// Element map for WidgetTemplatesCreateEditScreen
    /// </summary>
    public abstract class WidgetTemplatesCreateEditBaseScreen : FrameElementContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WidgetTemplatesCreateEditBaseScreen" /> class.
        /// </summary>
        /// <param name="find">The find.</param>
        public WidgetTemplatesCreateEditBaseScreen(Find find) : base(find)
        {
        }

        /// <summary>
        /// Gets the hosted frame info.
        /// </summary>
        /// <returns></returns>
        public override FrameInfo GetHostedFrameInfo()
        {
            return new FrameInfo();
        }

        /// <summary>
        /// Gets the create template button.
        /// </summary>
        /// <value>The create template button.</value>
        public HtmlAnchor CreateTemplateButton
        {
            get
            {
                return this.Get<HtmlAnchor>("id=controlTemplateEditor_ctl00_ctl00_topCommandBar_ctl01_ctl00_buttonLink");
            }
        }
   
        /// <summary>
        /// Gets the templates dropdown.
        /// </summary>
        /// <value>The templates dropdown.</value>
        public HtmlSelect TemplatesDropdown
        {
            get
            {
                return this.Get<HtmlSelect>("id=controlTemplateEditor_ctl00_ctl00_controlTypesList_ctl00_ctl00_dropDown");
            }
        }

        /// <summary>
        /// Gets the template name text box.
        /// </summary>
        /// <value>The template name text box.</value>
        public HtmlInputText TemplateNameTextBox
        {
            get
            {
                return this.Get<HtmlInputText>("id=controlTemplateEditor_ctl00_ctl00_templateNameField_ctl00_ctl00_textBox_write");
            }
        }

        /// <summary>
        /// Gets the text area.
        /// </summary>
        /// <value>The text area.</value>
        public HtmlTextArea TextArea
        {
            get
            {
                return this.Get<HtmlTextArea>("style=~position: absolute");
            }
        }
    }
}
