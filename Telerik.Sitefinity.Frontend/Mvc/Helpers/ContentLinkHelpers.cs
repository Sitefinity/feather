using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model.ContentLinks;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains extension methods for working with <see cref="ContentLink"/>.
    /// </summary>
    public static class ContentLinkHelpers
    {
        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns></returns>
        public static Image GetImage(this ContentLink contentLink)
        {
            if (contentLink == null)
                return null;

            var imagesManager = Telerik.Sitefinity.Modules.Libraries.LibrariesManager.GetManager(contentLink.ChildItemProviderName);
            var image = imagesManager.GetImages().FirstOrDefault(i => i.Id == contentLink.ChildItemId);

            return image;
        }

        /// <summary>
        /// Gets the video.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns></returns>
        public static Video GetVideo(this ContentLink contentLink)
        {
            if (contentLink == null)
                return null;

            var videoManager = Telerik.Sitefinity.Modules.Libraries.LibrariesManager.GetManager(contentLink.ChildItemProviderName);
            var video = videoManager.GetVideos().FirstOrDefault(i => i.Id == contentLink.ChildItemId);

            return video;
        }

        /// <summary>
        /// Gets the document.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <returns></returns>
        public static Document GetDocument(this ContentLink contentLink)
        {
            if (contentLink == null)
                return null;

            var documentManager = Telerik.Sitefinity.Modules.Libraries.LibrariesManager.GetManager(contentLink.ChildItemProviderName);
            var document = documentManager.GetDocuments().FirstOrDefault(i => i.Id == contentLink.ChildItemId);

            return document;
        }
    }
}
