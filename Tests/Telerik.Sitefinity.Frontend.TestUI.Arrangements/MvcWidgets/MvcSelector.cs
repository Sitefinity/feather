using System;
using System.IO;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Attributes;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// MvcWidgetDefaultFeatherDesigner arragement.
    /// </summary>
    public class MvcSelector : ITestArrangement
    {
        [ServerSetUp]
        public void SetUp()
        {
            Guid pageId = ServerOperations.Pages().CreatePage(PageName);
          
            var assembly = FileInjectHelper.GetArrangementsAssembly();
            Stream source = assembly.GetManifestResourceStream(FileResource);

            var path = Path.Combine("MVC", "Views", "DummyText", DesignerViewFileName);

            string filePath = FileInjectHelper.GetDestinationFilePath(path);
            Stream destination = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            FileInjectHelper.CopyStream(source, destination);
            ServerOperations.Widgets().AddMvcWidgetToPage(pageId, typeof(DummyTextController).FullName, WidgetCaption);
        }

        [ServerTearDown]
        public void TearDown()
        {
            var path = Path.Combine("MVC", "Views", "DummyText", DesignerViewFileName);
            string filePath = FileInjectHelper.GetDestinationFilePath(path);
            File.Delete(filePath); 
            ServerOperations.Pages().DeleteAllPages();
        }

        private const string FileResource = "Telerik.Sitefinity.Frontend.TestUI.Arrangements.Data.DesignerView.Selector.cshtml";
        private const string DesignerViewFileName = "DesignerView.Selector.cshtml";

        private const string PageName = "FeatherPage";
        private const string WidgetCaption = "SelectorWidget";
    }
}
