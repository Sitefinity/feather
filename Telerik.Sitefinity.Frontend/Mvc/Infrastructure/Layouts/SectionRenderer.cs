using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This control is responsible for rendering the section content.
    /// </summary>
    public class SectionRenderer : Control
    {
        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object, which writes the content to be rendered on the client.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the server control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            var context = new HttpContextWrapper(this.Context);
            writer.Write(ResourceHelper.RenderAllScripts(context, this.Name));
            writer.Write(ResourceHelper.RenderAllStylesheets(context, this.Name));
        }
    }
}
