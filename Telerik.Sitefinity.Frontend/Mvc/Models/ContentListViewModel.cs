using System;
using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// View model for views that display a list of content items.
    /// </summary>
    public class ContentListViewModel
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the type of content that is loaded.
        /// </summary>
        public Type ContentType { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapping element of the widget.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets an enumerable of the items that should be displayed.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<ItemViewModel> Items { get; set; }

        /// <summary>
        /// Gets or sets the total pages count.
        /// </summary>
        /// <value>
        /// The total pages count.
        /// </value>
        public int? TotalPagesCount { get; set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show pager.
        /// </summary>
        /// <value>
        ///   <c>true</c> to show pager; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPager { get; set; }
    }
}
