using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using Telerik.WebAii.Controls.Html;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Wraps actions and elements available in the Page Editor.
    /// </summary>
    public class PageZoneEditorWrapper : BaseWrapper
    {
        /// <summary>
        /// Clicks edit link for a selected widget.
        /// </summary>
        /// <param name="widgetName">The widget name.</param>
        /// <param name="dropZoneIndex">The drop zone index.</param>
        public void EditWidget(string widgetName, int dropZoneIndex = 0)
        {
            ActiveBrowser.RefreshDomTree();
            var widgetHeader = ActiveBrowser
                                            .Find
                                            .AllByCustom<HtmlDiv>(d => d.CssClass.StartsWith("rdTitleBar") && d.ChildNodes.First().InnerText.Equals(widgetName))[dropZoneIndex]
                                                                                                                                                                               .AssertIsPresent(widgetName);
            widgetHeader.ScrollToVisible();
            HtmlAnchor editLink = widgetHeader.Find
                                              .ByCustom<HtmlAnchor>(a => a.TagName == "a" && a.Title.Equals("Edit"))
                                              .AssertIsPresent("edit link");
            editLink.Focus();
            editLink.Click();
            ActiveBrowser.WaitUntilReady();
            ActiveBrowser.WaitForAsyncOperations();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Verifies the content in MVC widget.
        /// </summary>
        /// <param name="content">The expected content.</param>
        public void VerifyContentInWidget(string content)
        {
            ActiveBrowser.Find.ByCustom<HtmlDiv>(d => d.InnerText.Equals(content)).AssertIsPresent("edit link");                                                                                                                                                           
        }

        /// <summary>
        /// Drag and drop a layout widget element
        /// </summary>
        /// <param name="layoutCaption">the layout widget caption</param>
        public void DragAndDropLayoutWidget(string layoutCaption)
        {
            var layout = ActiveBrowser.Find.ByContent<HtmlDiv>(layoutCaption);
            Assert.IsNotNull(layout, "The layout was not found on the page");

            var layoutAcceptor = ActiveBrowser.Find.ByExpression<HtmlDiv>("id=?RadDockZoneContentplaceholder1")
                .AssertIsPresent<HtmlDiv>("RadDockZoneContentplaceholder1");

            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().AddWidgetToDropZone(layout, layoutAcceptor);
        }
    }
}