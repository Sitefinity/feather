using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels
{
    /// <summary>
    ///  Exposes properties data for the current Sitefinity page.
    /// </summary>
    public class PageDataViewModel
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        public Lstring Keywords { get; set; }
    }
}
