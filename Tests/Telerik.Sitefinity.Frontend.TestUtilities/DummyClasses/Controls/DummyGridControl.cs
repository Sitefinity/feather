using Telerik.Sitefinity.Frontend.GridSystem;
using Telerik.Sitefinity.Utilities.HtmlParsing;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Controls
{
    /// <summary>
    /// This class is used to fake the functionality of <see cref="Telerik.Sitefinity.Frontend.GridSystem.GridControl"/> class to expose its protected methods for testing purposes.
    /// </summary>
    public class DummyGridControl : GridControl
    {
        /// <inheritdoc />
        public string PublicProcessLayoutString(string template, bool ensureSfColsWrapper)
        {
            return this.ProcessLayoutString(template, ensureSfColsWrapper);
        }

        /// <inheritdoc />
        public string PublicGetAttributeValue(HtmlChunk chunk, string attributeName)
        {
            return this.GetAttributeValue(chunk, attributeName);
        }
    }
}
