using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Models.Fields
{
    /// <summary>
    /// This class represents view model for related media.
    /// </summary>
    public class RelatedMediaViewModel : RelatedViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedMediaViewModel"/> class.
        /// </summary>
        public RelatedMediaViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelatedMediaViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RelatedMediaViewModel(IDataItem item) : base(item)
        {
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get
            {
                var dynamicFieldContainer = (IDynamicFieldsContainer)this.Item;
                var title = dynamicFieldContainer.GetValue("Title").ToString();

                return title;
            }
        }

        /// <summary>
        /// Gets the media URL.
        /// </summary>
        /// <value>
        /// The media URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string MediaUrl
        {
            get 
            {
                var mediaContent = this.Item as MediaContent;

                return mediaContent.MediaUrl;
            }
        }

        /// <summary>
        /// Gets the thumbnail URL.
        /// </summary>
        /// <value>
        /// The thumbnail URL.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ThumbnailUrl
        {
            get
            {
                var mediaContent = this.Item as MediaContent;

                return mediaContent.ThumbnailUrl;
            }
        }
    }
}
