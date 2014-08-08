using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.InlineEditing.Attributes;

namespace Telerik.Sitefinity.Frontend.InlineEditing
{
    /// <summary>
    /// This class contains helper methods for creating the necessary HTML processing which makes the InlineEditing possible. 
    /// </summary>
    public class HtmlProcessor
    {
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
            string htmlTagType = "div";
            var tagBuilder = new TagBuilder(htmlTagType);
            tagBuilder.Attributes.Add("data-sf-provider", providerName);
            tagBuilder.Attributes.Add("data-sf-type", type);

            if (id != Guid.Empty)
                tagBuilder.Attributes.Add("data-sf-id", id.ToString());

            writer.Write(tagBuilder.ToString(TagRenderMode.StartTag));

            return new HtmlRegion(writer, htmlTagType);
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

            if (fieldInfoAttr == null)
            {
                htmlString = propValue.ToString();
            }
            else
            {
                htmlString = string.Format(HtmlProcessor.InlineEditingHtmlWrapper, fieldInfoAttr.Name, fieldInfoAttr.Type, propValue);
            }

            return htmlString;
        }

        public const string InlineEditingHtmlWrapper = "<div data-sf-field='{0}' data-sf-ftype='{1}'>{2}</div>";
    }
}
