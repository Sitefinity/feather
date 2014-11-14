using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Model.ContentLinks;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    public static class ContentLinkHelpers
    {
        public static Image GetImage(this ContentLink contentLink)
        {
            var imagesManager = Telerik.Sitefinity.Modules.Libraries.LibrariesManager.GetManager(contentLink.ChildItemProviderName);
            var image = imagesManager.GetImages().FirstOrDefault(i => i.Id == contentLink.ChildItemId);

            return image;
        }
    }
}
