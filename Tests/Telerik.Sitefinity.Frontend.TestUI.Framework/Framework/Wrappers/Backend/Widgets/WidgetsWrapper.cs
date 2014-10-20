using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Backend
{
    /// <summary>
    /// Widgets base actions. 
    /// </summary>
    public class WidgetsWrapper : BaseWrapper
    {
        /// <summary>
        /// Verifies that feather widget has the proper title.
        /// </summary>
        /// <param name="title">The widget title.</param>
        public void VerifyWidgetTitle(string title)
        {
            HtmlControl widgetTitleText = this.EM.Widgets.FeatherWidget.WidgetTitleText
                                              .AssertIsPresent("widget title text");

            widgetTitleText.AssertTextContentIsEqualTo(title, "widget title is not as expected");
        }

        /// <summary>
        /// When the first control is added to a form a submit button is automatically added to the page. 
        /// This method waits for the submit button to be added.
        /// </summary>
        public void WaitForSaveButtonToAppear()
        {
            Manager.Current.Wait.For(this.WaitForSaveButton, Manager.Current.Settings.ClientReadyTimeout);
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Verifies that the feather widget designer has the proper label text.
        /// </summary>
        /// <param name="text">The label text.</param>
        public void VerifyWidgetInputFieldLabelText(string text)
        {
            HtmlControl label = this.EM.Widgets.FeatherWidget.Label
                                    .AssertIsPresent("label");

            label.AssertTextContentIsEqualTo(text, "Label is incorrect");
        }

        /// <summary>
        /// Verifies that input text field is present  for the MVC Dummy widget. 
        /// </summary>
        public void VerifyDummyWidgetInputTextField()
        {
            HtmlInputText inputTextField = this.EM.Widgets.FeatherWidget.DummyWidgetInput
                                               .AssertIsPresent("Input Text Field");
        }

        /// <summary>
        /// Verifies that the save button is present in the designer.
        /// </summary>
        public void VerifyWidgetSaveButton()
        {
            HtmlButton saveButton = this.EM.Widgets.FeatherWidget.SaveButton
                                        .AssertIsPresent("save button");

            Assert.IsTrue(saveButton.InnerText.Equals("Save"), "Save button text is not correct");
        }

        /// <summary>
        /// Verifies that the Cancel button is present in the designer.
        /// </summary>
        public void VerifyWidgetCancelButton()
        {
            HtmlAnchor cancelButton = this.EM.Widgets.FeatherWidget.CancelButton
                                          .AssertIsPresent("Cancel button");

            Assert.IsTrue(cancelButton.InnerText.Equals("Cancel"), "Cancel button text is not correct");
        }

        /// <summary>
        /// Verifies that the close button is present in the designer.
        /// </summary>
        public void VerifyWidgetCloseButton()
        {
            HtmlButton closeButton = this.EM.Widgets.FeatherWidget.CloseButton
                                         .AssertIsPresent("close button");
        }

        /// <summary>
        /// Adds text to the dummy MVC widget input text field.
        /// </summary>
        /// <param name="text">The text message.</param>
        public void SetTextDummyWidget(string text)
        {
            HtmlInputText input = this.EM.Widgets.FeatherWidget.DummyWidgetInput
                                      .AssertIsPresent("input");

            input.ScrollToVisible();
            input.Focus();
            input.MouseClick();

            Manager.Current.Desktop.KeyBoard.TypeText(text);
        }

        /// <summary>
        /// Clicks the save button.
        /// </summary>
        public void ClickSaveButton()
        {
            HtmlButton saveButton = this.EM.Widgets.FeatherWidget.SaveButton
                                        .AssertIsPresent("save button");

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Clicks the advanced button.
        /// </summary>
        public void ClickAdvancedButton()
        {
            HtmlAnchor saveButton = this.EM.Widgets.FeatherWidget.AdvancedButton
                                        .AssertIsPresent("advanced button");

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Clicks the selector button.
        /// </summary>
        public void ClickSelectorButton()
        {
            HtmlAnchor saveButton = this.EM.Widgets.FeatherWidget.SelectorButton
                                        .AssertIsPresent("selector button");

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Selects the content.
        /// </summary>
        /// <param name="isFirstSelector">True or false depending on the selector.</param>
        public void SelectContent(string selector)
        {
            var contentSelector = this.GetContentSelectorByName(selector)
                                      .As<HtmlControl>()
                                      .AssertIsPresent("selector directive");

            HtmlButton selectButton = contentSelector.Find
                                                     .ByAttributes("class=~openSelectorBtn")
                                                     .As<HtmlButton>()
                                                     .AssertIsPresent("select button");

            selectButton.Click();
            ActiveBrowser.WaitForAsyncOperations();
            ActiveBrowser.RefreshDomTree();                    
        }

        /// <summary>
        /// Selects the item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        public void SelectItem(params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                var anchor = this.EM.Widgets.FeatherWidget.Find.ByCustom<HtmlAnchor>(a => a.InnerText.Contains(itemName));
                anchor.AssertIsPresent("item name not present");

                anchor.Click();
            }
        }

        /// <summary>
        /// Checks the notification in selected tab.
        /// </summary>
        /// <param name="itemNames">The item names.</param>
        public void CheckNotificationInSelectedTab(int expectedCout)
        {

            var span = this.EM.Widgets.FeatherWidget.Find.ByExpression<HtmlSpan>("ng-show=selectedItemsInTheDialog.length !== 0", string.Format("InnerText=~{0}", expectedCout));
            span.AssertIsPresent("item name not present");
        }

        /// <summary>
        /// Opens the selected tab.
        /// </summary>
        public void OpenSelectedTab()
        {
            HtmlAnchor selectedTab = this.EM.Widgets.FeatherWidget.SelectedTab
                                        .AssertIsPresent("selector button");

            selectedTab.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Confirms the selecting.
        /// </summary>
        public void DoneSelecting()
        {
            HtmlButton doneButton = this.EM.Widgets.FeatherWidget.DoneButton
                                        .AssertIsPresent("done button");

            doneButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Verifies the selected item.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        public void VerifySelectedItem(params string[] itemNames)
        {
            foreach (var itemName in itemNames)
            {
                var item = this.EM.Widgets.FeatherWidget.Find.ByCustom<HtmlSpan>(a => a.InnerText.Contains(itemName));
                item.AssertIsPresent("item name not present");
            }
        }

        /// <summary>
        /// Sets a text to search in t he search input.
        /// </summary>
        /// <param name="text">The text to be searched for.</param>
        public void SetSearchText(string text)
        {
            HtmlInputText input = this.EM.Widgets.FeatherWidget.SearchInput
                                      .AssertIsPresent("input");

            input.ScrollToVisible();
            input.Focus();
            input.MouseClick();

            Manager.Current.Desktop.KeyBoard.TypeText(text);

            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Waits for items count to appear.
        /// </summary>
        /// <param name="expectedCount">The expected items count.</param>
        public void WaitForItemsToAppear(int expectedCount)
        {
            Manager.Current.Wait.For(() => this.CountItems(expectedCount), 50000);
        }

        /// <summary>
        /// Verifies if the items count is as expected.
        /// </summary>
        /// <param name="expected">The expected items count.</param>
        /// <returns>True or false depending on the items count.</returns>
        public bool CountItems(int expected)
        {
            ActiveBrowser.RefreshDomTree();
            var activeDialog = this.EM.Widgets.FeatherWidget.ActiveTab.AssertIsPresent("Content container");
            var items = activeDialog.Find.AllByExpression<HtmlAnchor>("ng-repeat=item in items");
            int actual = items.Count;
            bool isCountCorrect = expected == actual;
            return isCountCorrect;
        }

        private Element GetContentSelectorByName(string cssClass)
        {
            var contentContainer = this.EM.Widgets.FeatherWidget.ContentContainer.AssertIsPresent("Content container");
            var contentSelector = contentContainer.Find.ByName(cssClass);
            return contentSelector;
        }

        private bool WaitForSaveButton()
        {
            Manager.Current.ActiveBrowser.RefreshDomTree();
            var saveButton = ActiveBrowser.Find.ByExpression<HtmlButton>("tagname=button", "class=btn btn-primary pull-left ng-scope");
            bool result = saveButton != null && saveButton.IsVisible();

            return result;
        }
    }
}
