using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend.PageTemplates
{
    /// <summary>
    /// Provides operations for Page Template Editor Screen.
    /// </summary>
    public class PageTemplateEditorWrapper : BaseWrapper
    {
        /// <summary>
        /// Verifies if certain text in present in the template container div.
        /// </summary>
        /// <param name="text">The searched string.</param>
        /// <returns>True or false depending on the text present.</returns>
        public bool IsTextPresentInTemplateMainContainer(string text)
        {
            HtmlDiv container = EM.PageTemplates.PageTemplateEditor.TemplateContainer
                .AssertIsPresent("Template container");

            bool isTextPresent = container.InnerText.Contains(text);

            return isTextPresent;
        }

        /// <summary>
        /// Verifies if certain placeholder in present.
        /// </summary>
        /// <param name="placeHolderId">The placeholder Id.</param>
        /// <returns>True or false depending on whether the placeholder is present.</returns>
        public bool IsPlaceHolderPresent(string placeHolderId)
        {
            HtmlDiv placeHolder = ActiveBrowser.Find.ByExpression<HtmlDiv>("placeholderid=" + placeHolderId);

            bool isPlaceholderPresent = true;
            try
            {
                isPlaceholderPresent = placeHolder != null && placeHolder.IsVisible();
            }
            catch (NullReferenceException)
            {
                isPlaceholderPresent = false;
            }

            return isPlaceholderPresent;
        }
    }
}
