using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is responsible for resolving virtual paths for the layout templates. 
    /// </summary>
    public class LayoutVirtualPathBuilder
    {
        #region Public members

        /// <summary>
        /// Builds the path from template title.
        /// </summary>
        /// <param name="templateTitle">Title of the template.</param>
        /// <returns> Resolved path will be in the following format: "~/SfLayouts/some_title.master"</returns>
        public virtual string BuildPathFromTitle(string templateTitle)
        {
            var templateFileNameParser = new TemplateTitleParser();
            var fileName = templateFileNameParser.GetLayoutName(templateTitle);

            var layoutVirtualPath = string.Format(LayoutVirtualPathBuilder.layoutVirtualPathTemplate, LayoutVirtualPathBuilder.layoutsPrefix, fileName, LayoutVirtualPathBuilder.layoutSuffix);

            var packagesManager = new PackageManager();
            var currentPackage = packagesManager.GetCurrentPackage();

            layoutVirtualPath = (new VirtualPathBuilder()).AddParams(layoutVirtualPath, currentPackage);

            return layoutVirtualPath;
        }

        /// <summary>
        /// Gets the layout file name from virtual path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        public virtual string GetLayoutName(PathDefinition definition, string virtualPath)
        {
            if (!virtualPath.EndsWith(LayoutVirtualPathBuilder.layoutSuffix, StringComparison.OrdinalIgnoreCase))
                return null;

            var definitionVp = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.ToAppRelative(definition.VirtualPath));
            var pageTemplateNameLength= virtualPath.Length - definitionVp.Length - LayoutVirtualPathBuilder.layoutSuffix.Length - 1;
            string pageTemplateName = virtualPath.Substring(definitionVp.Length, pageTemplateNameLength);

            while (!string.IsNullOrEmpty(pageTemplateName) && pageTemplateName.EndsWith("."))
            {
                pageTemplateName = pageTemplateName.Substring(0, pageTemplateName.Length - 1);
            }

            return pageTemplateName;
        }

        #endregion

        #region Constants

        /// <summary>
        /// The layouts prefix.
        /// </summary>
        public const string layoutsPrefix = "SfLayouts";

        /// <summary>
        /// This suffix is recognized by the VirtualPathResolver for resolving the layout page.
        /// </summary>
        public const string layoutSuffix = "master";

        /// <summary>
        /// The template used when resolving the layout virtual path. 
        /// </summary>
        private const string layoutVirtualPathTemplate = "~/{0}/{1}.{2}"; 

        #endregion
    }
}
