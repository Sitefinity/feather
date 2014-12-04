using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// View model for views that display the details of a single content item.
    /// </summary>
    public class ContentDetailsViewModel
    {
        /// <summary>
        /// Gets or sets the type of content that is loaded.
        /// </summary>
        public Type ContentType { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the CSS class that will be applied on the wrapper div of the widget.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        /// <value>
        /// The detail news.
        /// </value>
        public ItemViewModel Item { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable social sharing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should enable social sharing; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSocialSharing { get; set; }
    }
}
