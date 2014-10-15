using System;
using System.Collections.Generic;
using System.IO;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// MvcSelectorTest arragement.
    /// </summary>
    public class MvcSelectMoreThanOneDynamicItem : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            FeatherServerOperations.ModuleBuilder().EnsureModuleIsImported(this.relatedDataDuplicateModule);

            for (int i = 0; i < 20; i++)
            {
                ServerOperations.DynamicTypes().CreateDynamicItem("Telerik.Sitefinity.DynamicTypes.Model.DuplicateRelatedDataModule.Duplicaterelateddata", "SomeUrlName", title: ItemTitle + i);
            }
        
            Guid pageId = ServerOperations.Pages().CreatePage(PageName);

            FeatherServerOperations.ResourcePackages().ImportDataForSelectorsTests(FileResource, DesignerViewFileName, FileResourceJson, JsonFileName, ControllerFileResource, ControllerFileName);
            ServerOperations.Widgets().AddMvcWidgetToPage(pageId, typeof(DummyTextController).FullName, WidgetCaption);
        }

        [ServerTearDown]
        public void TearDown()
        {
            ServerOperations.Pages().DeleteAllPages();
            ServerOperations.DynamicTypes().DeleteAllDynamicItemsForAllModules();
            ServerOperations.ModuleBuilder().DeleteAllModules(string.Empty, "Module Installations");

            FeatherServerOperations.ResourcePackages().DeleteSelectorsData(DesignerViewFileName, JsonFileName, ControllerFileName);
        }

        private readonly Dictionary<string, string> relatedDataDuplicateModule = new Dictionary<string, string>()
        {
             { "DuplicateRelatedDataModule", "Telerik.Sitefinity.Frontend.TestUtilities.Data.DuplicateRelatedDataModule.zip" }       
        };

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.cshtml";
        private const string FileResourceJson = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.json";
        private const string ControllerFileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.designerview-selector.js";

        private const string DesignerViewFileName = "DesignerView.Selector.cshtml";
        private const string JsonFileName = "DesignerView.Selector.json";
        private const string ControllerFileName = "designerview-selector.js";

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";

        private const string ItemTitle = "Item Title";
    }
}
