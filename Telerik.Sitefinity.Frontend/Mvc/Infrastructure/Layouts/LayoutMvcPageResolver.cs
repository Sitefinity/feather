using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Instances of this class render virtual page files for pure MVC pages that can be based on a layout file.
    /// </summary>
    internal class LayoutMvcPageResolver : PureMvcPageResolver
    {
        /// <summary>
        /// Appends markup to the virtual page file depending on the current master page.
        /// </summary>
        /// <param name="virtualPath">The master page virtual path.</param>
        /// <param name="context">The context.</param>
        /// <param name="output">The output.</param>
        /// <param name="placeHolders">The place holders.</param>
        /// <param name="directives">The directives.</param>
        protected override void BuildWithMasterPage(string virtualPath, RequestContext context, StringBuilder output, CursorCollection placeHolders, DirectiveCollection directives)
        {
            if (LayoutMvcPageResolver.IsLayoutPath(virtualPath))
            {
                var httpContext = new HttpContextWrapper(new HttpContext(HttpContext.Current.Request, HttpContext.Current.Response));
                httpContext.Items[PackageManager.CurrentPackageKey] = context.HttpContext.Items[PackageManager.CurrentPackageKey];
                SystemManager.RunWithHttpContext(httpContext, () => base.BuildWithMasterPage(virtualPath, context, output, placeHolders, directives));
            }
            else
            {
                base.BuildWithMasterPage(virtualPath, context, output, placeHolders, directives);
            }
        }

        private static bool IsLayoutPath(string virtualPath)
        {
            return virtualPath != null && virtualPath.StartsWith(string.Format(CultureInfo.InvariantCulture, "~/{0}", LayoutVirtualFileResolver.ResolverPath), StringComparison.Ordinal);
        }
    }
}
