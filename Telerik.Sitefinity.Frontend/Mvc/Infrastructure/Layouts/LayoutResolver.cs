using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is responsible for the operations concerning the resolving of the layout pages.  
    /// </summary>
    public class LayoutResolver : ILayoutResolver
    {
        /// <summary>
        /// Resolves the layout virtual path and ensures that file exist on the specified location.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>
        /// If the provided template is not in pure MVC mode returns null.
        /// If the provided template is in pure MVC mode returns virtual path like "~/SfLayouts/some_title.master" and ensures that this path is pointing to existing resource.
        /// Otherwise returns null. 
        /// </returns>
        public virtual string GetVirtualPath(Pages.Model.IPageTemplate template)
        {
            if (template.GetTemplateFramework() != PageTemplateFramework.Mvc)
                return null;

            var hasTitle = template as IHasTitle;

            if (hasTitle == null)
                return null;

            string templateTitle = hasTitle.GetTitle();
            var virtualBuilder = new LayoutVirtualPathBuilder();
            var layoutVirtualPath = virtualBuilder.BuildPathFromTitle(templateTitle);
            var doesLayoutExist = VirtualPathManager.FileExists(layoutVirtualPath);

            if (!doesLayoutExist)
                layoutVirtualPath = null;

            return layoutVirtualPath;
        }     
    }
}
