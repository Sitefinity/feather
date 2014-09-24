using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class is responsible for resolving the actual file name of the layout used for template based on convention.
    /// </summary>
    internal class TemplateTitleParser
    {
        #region Public methods

        /// <summary>
        /// Gets the layout name from template title. This method will strip and replace the special characters.
        /// </summary>
        /// <param name="templateTitle">The template title.</param>
        /// <returns></returns>
        public virtual string GetLayoutName(string templateTitle)
        {
            templateTitle = this.StripPackageNameFromTemplateName(templateTitle);

            var packagesManager = new PackageManager();
            return packagesManager.StripInvalidCharacters(templateTitle);
        } 

        #endregion

        #region Private methods

        /// <summary>
        /// Strips the name of the package name from template.
        /// If there is no existing package presented on the searched location the method will preserve the full template name.
        /// </summary>
        /// <param name="templateTitle">Title of the template.</param>
        /// <returns></returns>
        private string StripPackageNameFromTemplateName(string templateTitle)
        {
            var parts = templateTitle.Split('.');

            if (parts.Length > 1)
            {
                var packagesManager = new PackageManager();
                var packageVirtualPath = packagesManager.GetPackageVirtualPath(parts[0]);
                var packagePath = FrontendManager.VirtualPathBuilder.MapPath(packageVirtualPath);
                if (Directory.Exists(packagePath))
                    templateTitle = string.Join(".", parts, 1, parts.Length - 1);
            }

            return templateTitle;
        } 

        #endregion
    }
}
