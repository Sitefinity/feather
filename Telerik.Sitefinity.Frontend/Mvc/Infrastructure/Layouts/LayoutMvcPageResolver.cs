using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Routing;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Mvc.Rendering;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// Instances of this class render virtual page files for pure MVC pages that can be based on a layout file.
    /// </summary>
    internal class LayoutMvcPageResolver : PureMvcPageResolver
    {
        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies"></param>
        /// <param name="utcStart"></param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency" /> object for the specified virtual resources.
        /// </returns>
        public override CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var baseDependencies = base.GetCacheDependency(definition, virtualPath, virtualPathDependencies, utcStart);
            var pageData = this.GetCurrentPageData();
            if (pageData != null && pageData.Template != null)
            {
                var masterPagePath = LayoutMvcPageResolver.GetMasterPagePath(pageData.Template);
                if (LayoutMvcPageResolver.IsLayoutPath(masterPagePath))
                {
                    var layoutDependency = HostingEnvironment.VirtualPathProvider.GetCacheDependency(masterPagePath, virtualPathDependencies, utcStart);
                    var aggregate = new AggregateCacheDependency();
                    aggregate.Add(layoutDependency, baseDependencies);
                    
                    return aggregate;
                }
            }

            return baseDependencies;
        }

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

        private static string GetMasterPagePath(IPageTemplate template)
        {
            var masterPage = template.MasterPage;
            if (masterPage.IsNullOrEmpty())
            {
                var layoutTemplateBuilder = ObjectFactory.Resolve<ILayoutResolver>();
                masterPage = layoutTemplateBuilder.GetVirtualPath(template);
            }

            if (!masterPage.IsNullOrEmpty())
            {
                return masterPage;
            }
            else if (template.ParentTemplate != null)
            {
                return LayoutMvcPageResolver.GetMasterPagePath(template.ParentTemplate);
            }
            else
            {
                return null;
            }
        }

        private PageData GetCurrentPageData()
        {
            RequestContext requestContext;
            var node = this.GetRequestedPageNode(out requestContext);
            if (node == null)
                return null;

            var siteMap = (SiteMapBase)node.Provider;
            var pageManager = PageManager.GetManager(siteMap.PageProviderName);
            var pageData = pageManager.GetPageData(node.PageId);

            return pageData;
        }
    }
}
