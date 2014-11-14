using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Model.ContentLinks;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers.ViewModels.Fields
{
    /// <summary>
    /// This class represents view model for media fields.
    /// </summary>
    public class MediaFieldViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaFieldViewModel" /> class.
        /// </summary>
        /// <param name="mediaItem">The media item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldTitle">The field title.</param>
        public MediaFieldViewModel(ContentLink mediaItem, string fieldName, string fieldTitle)
        {
            this.FieldTitle = fieldTitle;
            this.FieldName = fieldName;
            this.MediaItem = mediaItem;
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the field title.
        /// </summary>
        /// <value>
        /// The field title.
        /// </value>
        public string FieldTitle { get; set; }

        /// <summary>
        /// Gets or sets the media item.
        /// </summary>
        /// <value>
        /// The media item.
        /// </value>
        public ContentLink MediaItem { get; set; }
    }
}
