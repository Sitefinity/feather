using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Utilities.HtmlParsing;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// Inheritor of GridControl used to test its protected methods.
    /// </summary>
    public class DummyGridControl : GridControl
    {
        /// <summary>
        /// Makes sure that the system containers are runat="server" so the layout declaration can be used as a proper container.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="ensureSfColsWrapper">if set to <c>true</c> ensures sf_cols containers exists in the template.</param>
        public string PublicProcessLayoutString(string template, bool ensureSfColsWrapper)
        {
            return this.ProcessLayoutString(template, ensureSfColsWrapper);
        }

        /// <summary>
        /// Gets the value of a given attribute by its name.
        /// </summary>
        /// <param name="chunk">The chunk.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public string PublicGetAttributeValue(HtmlChunk chunk, string attributeName)
        {
            return this.GetAttributeValue(chunk, attributeName);
        }
    }
}
