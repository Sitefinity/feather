using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// ManageGridWidgetOnThePageTemplate arragement.
    /// </summary>
    public class ManageGridWidgetOnThePageTemplate : TestArrangementBase
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
            ServerOperations.Templates().SharePageTemplateWithSite(PageTemplateName, "SecondSite");
        }

        /// <summary>
        /// Tears down.
        /// </summary>
        [ServerTearDown]
        public void TearDown()
        {
            var template = ServerOperations.Templates().GetTemplateIdByTitle(PageTemplateName);
            
            ServerOperations.Templates().UnSharePageTemplateWithSite(PageTemplateName, "SecondSite");
            ServerOperations.Templates().DeletePageTemplate(template);            

            string templateFileCopy = FileInjectHelper.GetDestinationFilePath(this.newLayoutTemplatePath);
            File.Delete(templateFileCopy);
        }

        private const string PageTemplateName = "defaultNew";
        private string layoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "default.cshtml");
        private string newLayoutTemplatePath = Path.Combine("ResourcePackages", "Bootstrap", "MVC", "Views", "Layouts", "defaultNew.cshtml");
    }
}
