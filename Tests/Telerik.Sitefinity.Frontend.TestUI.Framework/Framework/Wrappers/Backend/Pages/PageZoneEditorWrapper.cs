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
        /// Drag and drop a layout element to placeholder.
        /// </summary>
        /// <param name="layoutCaption">the layout caption.</param>
        /// <param name="layoutCaption">the placeholder Id.</param>
        public void DragAndDropLayoutWidgetToPlaceholder(string layoutCaption, string placeHolder = "Contentplaceholder1")
        {
            var layout = ActiveBrowser.Find.ByContent<HtmlDiv>(layoutCaption);
            Assert.IsNotNull(layout, "The layout was not found on the page");

            HtmlDiv radDockZone = ActiveBrowser.Find.ByExpression<HtmlDiv>("placeholderid=" + placeHolder)
              .AssertIsPresent<HtmlDiv>(placeHolder);

            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().AddWidgetToDropZone(layout, radDockZone);
        }

        /// <summary>
        /// Drag and drop a widget element to placeholder.
        /// </summary>
        /// <param name="layoutCaption">the widget caption.</param>
        /// <param name="layoutCaption">the placeholder Id.</param>
        public void DragAndDropWidgetToPlaceholder(string widgetCaption, string placeHolder = "Contentplaceholder1")
        {
            var widget = this.GetWidgetByName(widgetCaption);

            HtmlDiv radDockZone = ActiveBrowser.Find.ByExpression<HtmlDiv>("placeholderid=" + placeHolder)
              .AssertIsPresent<HtmlDiv>(placeHolder);

            BAT.Wrappers().Backend().Pages().PageZoneEditorWrapper().AddWidgetToDropZone(widget, radDockZone);
        }

        /// <summary>
        /// Verifies whether a layout widget is present in the layout toolbox section.
        /// </summary>
        /// <param name="layoutCaption">The name of the layout widget.</param>
        /// <returns>true or false depending on the widget presence in the toolbox.</returns>
        public bool IsLayoutWidgetPresentInToolbox(string layoutCaption)
        {
            ActiveBrowser.RefreshDomTree();

            HtmlDiv layoutWidgetsContainer = ActiveBrowser.WaitForElementEndsWithID("LayoutToolboxContainer").As<HtmlDiv>();
            Assert.IsNotNull(layoutWidgetsContainer, "The widget container wasn't found on the page");

            if (layoutWidgetsContainer.InnerText.Contains(layoutCaption))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the widget by name
        /// </summary>
        /// <param name="widgetLabelName">The widget label name</param>
        /// <returns>Returns the widget div</returns>
        public HtmlDiv GetWidgetByName(string widgetLabelName)
        {
            ActiveBrowser.RefreshDomTree();
            RadPanelBar toolbox = Manager.Current.ActiveBrowser.Find.ByExpression<RadPanelBar>("id=~ControlToolboxContainer");
            foreach (var item in toolbox.AllItems)
            {
                var dockZone = item.Find.ByCustom<RadDockZone>(zone => zone.CssClass.Contains("RadDockZone"));
                var widgetLabel = dockZone.Find.ByContent(widgetLabelName);
                if (widgetLabel != null)
                {
                    if (!item.Expanded)
                        item.Expand();
                    return new HtmlDiv(widgetLabel.Parent);
                }
            }

            Assert.IsNotNull(null, "The widget with name: " + widgetLabelName);
            return null;
        }
    }
}