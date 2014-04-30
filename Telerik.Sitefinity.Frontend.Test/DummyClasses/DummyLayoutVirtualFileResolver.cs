using System;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// Dummy class implemented for testing purposes. Accesses protected members of LayoutVirtualFileResolver without reflection.
    /// </summary>
    //public class DummyLayoutVirtualFileResolver : LayoutVirtualFileResolver
    //{
    //    /// <summary>
    //    /// Tries to parse package name and page template.
    //    /// </summary>
    //    /// <param name="definition">The definition.</param>
    //    /// <param name="virtualPath">The virtual path.</param>
    //    /// <param name="packageName">Name of the package.</param>
    //    /// <param name="pageTemplateId">The page template id.</param>
    //    public bool TestTryResolvesPageTemplateName(PathDefinition definition, string virtualPath, out string pageTemplateName)
    //    {
    //        pageTemplateName = base.ResolvesPageTemplateName(definition, virtualPath);
    //        return !string.IsNullOrEmpty(pageTemplateName);
    //    }
    //}
}
