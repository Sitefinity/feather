using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helper methods for registering client resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Registers javascript reference.
        /// </summary>
        /// <param name="scriptPath">The path to the javascript file.</param>
        /// <param name="throwException">OPTIONAL: Indicates whether to throw an exception if the javascript is already registered. By default the value is set to <value>false</value>.</param>
        /// <returns>MvcHtmlString</returns>
        public static MvcHtmlString Script(this HtmlHelper helper, string scriptPath, bool throwException = false)
        {
            var attributes = new KeyValuePair<string, string>[2];
            attributes[0] = new KeyValuePair<string, string>("src", scriptPath);
            attributes[1] = new KeyValuePair<string, string>("type", "text/javascript");

            var register = new ResourceRegister("JsRegister", helper.ViewContext.HttpContext);

            return ResourceHelper.RegisterResource(register, scriptPath, throwException, tagName: "script", attribbutes: attributes);
        }

        /// <summary>
        /// Registers resource reference.
        /// </summary>
        private static MvcHtmlString RegisterResource(ResourceRegister register, string resourceKey, bool throwException, string tagName, KeyValuePair<string, string>[] attribbutes)
        {
            string output;
            MvcHtmlString result;

            if (throwException)
            {
                register.RegisterResource(resourceKey);
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
                result = new MvcHtmlString(output);
            }
            else if (register.TryRegisterResource(resourceKey))
            {
                output = ResourceHelper.GenerateTag(tagName, attribbutes);
                result = new MvcHtmlString(output);
            }
            else
            {
                result = MvcHtmlString.Empty;
            }

            return result;
        }

        /// <summary>
        /// Creates a string representation of a tag. 
        /// </summary>
        /// <param name="tag">The type of the HTML tag that would be generated for every registered resource.</param>
        /// <param name="attributes">The attributes associated with the tag.</param>
        /// <returns>The string representation of a tag.</returns>
        private static string GenerateTag(string tagName, params KeyValuePair<string, string>[] attribbutes)
        {
            var tag = new TagBuilder(tagName);

            foreach (var attr in attribbutes)
                tag.Attributes[attr.Key] = attr.Value;

            return tag.ToString();
        }
    }
}