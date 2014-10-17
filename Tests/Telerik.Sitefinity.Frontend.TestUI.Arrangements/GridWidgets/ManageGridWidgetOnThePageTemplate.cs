using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// ManageGridWidgetOnThePageTemplate arragement.
    /// </summary>
    public class ManageGridWidgetOnThePageTemplate : ITestArrangement
    {
        /// <summary>
        /// Server side set up. 
        /// </summary>
        [ServerSetUp]
        public void SetUp()
        {
            string templateFileOriginal = FileInjectHelper.GetDestinationFilePath(this.layoutTemplatePath);
            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);

            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();
            File.Copy(templateFileOriginal, templateFileCopy);
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);
            File.Delete(templateFileCopy);

            ServerOperations.Templates().DeletePageTemplate(PageTemplateName);
        }

        private const string PageTemplateName = "Bootstrap.defaultNew";
        private string layoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
        private string newLayoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");
    }
}
