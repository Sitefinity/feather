using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    public class AddWidgetToPageBasedOnLayoutWithMultiplePlaceholders : ITestArrangement
    {
        [ServerArrangement]
        public void AddNewLayoutFile()
        {
            this.CreateLayoutFolderAndCopyLayoutFile();
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.Templates().DeletePageTemplate(TemplateTitle);
            string filePath = this.GetFilePath();
            File.Delete(filePath);
        }

        private void CreateLayoutFolderAndCopyLayoutFile()
        {
            PageManager pageManager = PageManager.GetManager();
            int templatesCount = pageManager.GetTemplates().Count();

            string filePath = this.GetFilePath();

            FeatherServerOperations.ResourcePackages().AddNewResource(FileResource, filePath);
            FeatherServerOperations.ResourcePackages().WaitForTemplatesCountToIncrease(templatesCount, 1);
        }

        private string GetFilePath()
        {
            string folderPath = Path.Combine(FeatherServerOperations.ResourcePackages().SfPath, "MVC", "Views", "Layouts");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, LayoutFileName);

            return filePath;
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUtilities.Data.TestLayoutTwoPlaceholders.cshtml";
        private const string LayoutFileName = "TestLayoutTwoPlaceholders.cshtml";
        private const string TemplateTitle = "TestLayoutTwoPlaceholders";
    }
}
