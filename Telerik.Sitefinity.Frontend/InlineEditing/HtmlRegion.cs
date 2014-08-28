using System;

namespace Telerik.Sitefinity.Frontend.InlineEditing
{
    /// <summary>
    /// This class is used to create an HTML region. 
    /// It closes the provided tag on dispose.
    /// </summary>
    public class HtmlRegion : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlRegion"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="htmlTagType">Type of the HTML tag.</param>
        public HtmlRegion(System.IO.TextWriter writer, string htmlTagType)
        {
            this.htmlTagType = htmlTagType;
            this.writer = writer;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            var closeHtmlTag = string.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", this.htmlTagType);
            this.writer.Write(closeHtmlTag, System.Globalization.CultureInfo.InvariantCulture);
        }

        private readonly string htmlTagType;
        private readonly System.IO.TextWriter writer;
    }
}
