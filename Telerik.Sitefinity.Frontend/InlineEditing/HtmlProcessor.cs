using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.InlineEditing.Attributes;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.InlineEditing
{
    /// <summary>
    /// This class contains helper methods for creating the necessary HTML processing which makes the InlineEditing possible. 
    /// </summary>
    public class HtmlProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlProcessor"/> class.
        /// </summary>
        public HtmlProcessor()
            : this(SystemManager.IsInlineEditingMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlProcessor"/> class.
        /// </summary>
        /// <param name="isInlineEditing">Value indicating whether HTML should be generated for inline editing.</param>
        public HtmlProcessor(bool isInlineEditing)
        {
            this.isInlineEditingMode = isInlineEditing;
        }

        /// <summary>
        /// Creates a region which has the required by the InlineEditing attributes.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="type">The type.</param>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual HtmlRegion CreateInlineEditingRegion(TextWriter writer, string providerName, string type, Guid id)
        {
            if (this.isInlineEditingMode)
            {
                string htmlTagType = "div";
                var tagBuilder = new TagBuilder(htmlTagType);
                tagBuilder.Attributes.Add("data-sf-provider", providerName);
                tagBuilder.Attributes.Add("data-sf-type", type);

                if (id != Guid.Empty)
                    tagBuilder.Attributes.Add("data-sf-id", id.ToString());

                writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));

                return new HtmlRegion(writer, htmlTagType);
            }
            else
            {
                return new HtmlRegion(writer, string.Empty);
            }
        }

        /// <summary>
        /// Gets the string content from a model by a given property name.
        /// If the property is marked with FieldInfoAttribute the text will be wrapped into a InlineEditing region.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propName">Name of the property.</param>
        /// <returns></returns>
        public virtual string GetStringContent(object model, string propName)
        {
            string htmlString;
            Type t = model.GetType();
            var prop = t.GetProperty(propName);
            var propValue = prop.GetValue(model, null);
            var propattr = prop.GetCustomAttributes(false);

            var fieldInfoAttr = propattr.OfType<FieldInfoAttribute>().FirstOrDefault();

            if (fieldInfoAttr == null || !this.isInlineEditingMode)
            {
                htmlString = propValue.ToString();
            }
            else
            {
                htmlString = string.Format(System.Globalization.CultureInfo.InvariantCulture, HtmlProcessor.InlineEditingHtmlWrapper, fieldInfoAttr.Name, fieldInfoAttr.FieldType, propValue);
            }

            return htmlString;
        }

        private bool isInlineEditingMode;

        public const string InlineEditingHtmlWrapper = "<div data-sf-field='{0}' data-sf-ftype='{1}'>{2}</div>";
    }
}
