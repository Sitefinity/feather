using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Views
{
    /// <summary>
    /// This class implements <see cref="System.Web.Mvc.IView"/> for test purposes.
    /// </summary>
    public class DummyView : IView
    {
        /// <summary>
        /// Gets or sets the inner HTML. Should be populated with the html of the view.
        /// </summary>
        /// <value>
        /// The inner HTML.
        /// </value>
        public string InnerHtml { get; set; }

        /// <summary>
        /// Renders the specified view context by using the specified the writer object into the <see cref="InnerHtml"/> property.
        /// </summary>
        /// <param name="viewContext">The view context.</param>
        /// <param name="writer">The writer object.</param>
        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write(this.InnerHtml);
        }
    }
}
