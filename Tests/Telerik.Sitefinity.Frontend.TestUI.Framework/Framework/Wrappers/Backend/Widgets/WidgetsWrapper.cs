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
            HtmlAnchor saveButton = this.EM.Widgets.FeatherWidget.SelectorButton;

            saveButton.Wait.ForExists();

            saveButton.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Selects the content.
        /// </summary>
        /// <param name="selector">Selector name.</param>
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
                var itemDiv = this.EM.Widgets.FeatherWidget.Find.ByCustom<HtmlDiv>(a => a.InnerText.Equals(itemName));

                itemDiv.ScrollToVisible();
                itemDiv.MouseClick();
            }

            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Checks the notification in selected tab.
        /// </summary>
        /// <param name="itemNames">The item names.</param>
        public void CheckNotificationInSelectedTab(int expectedCout)
        {
            var span = this.EM.Widgets.FeatherWidget.Find.ByExpression<HtmlSpan>("class=badge ng-binding", string.Format("InnerText=~{0}", expectedCout));
            span.AssertIsPresent("item name not present");
        }

        /// <summary>
        /// Opens the selected tab.
        /// </summary>
        public void OpenSelectedTab()
        {
            HtmlSpan selectedTab = this.EM.Widgets.FeatherWidget.SelectedTab
                                         .AssertIsPresent("selected tab");
            selectedTab.Click();
            ActiveBrowser.WaitForAsyncRequests();
            ActiveBrowser.RefreshDomTree();
        }

        /// <summary>
        /// Opens the all tab.
        /// </summary>
        public void OpenAllTab()
        {
            HtmlSpan allTab = this.EM.Widgets.FeatherWidget.AllTab
                                    .AssertIsPresent("all tab");

            allTab.Click();
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
        public void VerifySelectedItemInMultipleSelectors(string[] itemNames, bool isNewsItem = false)
        {
            if (isNewsItem)
            {
                var divList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlDiv>("ng-bind=hierarchical ? item.Path : bindIdentifierField(item)");
                int divListCount = divList.Count;

                for (int i = 0; i < divListCount; i++)
                {
                    Assert.AreEqual(divList[i].InnerText, itemNames[i]);
                }
            }
            else
            {
                var divList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlDiv>("ng-repeat=item in selectedItems | limitTo:5");
                int divListCount = divList.Count;

                for (int i = 0; i < divListCount; i++)
                {
                    Assert.AreEqual(divList[i].InnerText, itemNames[i]);
                }
            }
        }
 
        public void VerifySelectedItemInFlatSelectors(params string[] itemNames)
        {
            foreach (var item in itemNames)
            {
                ActiveBrowser.Find.ByExpression<HtmlDiv>("InnerText=" + item).AssertIsPresent(item + " not present");
            }
        }

        /// <summary>
        /// Sets a text to search in t he search input.
        /// </summary>
        /// <param name="text">The text to be searched for.</param>
        public void SetSearchText(string text)
        {
            var activeDialog = this.EM.Widgets.FeatherWidget.ActiveTab.AssertIsPresent("Content container");
            var searchInputTextBox = activeDialog.Find.ByExpression<HtmlInputText>("ng-model=sfFilter.searchString");

            searchInputTextBox.Focus();
            searchInputTextBox.MouseClick();
            if (text != string.Empty)
            {
                searchInputTextBox.Text = string.Empty;
                Manager.Current.Desktop.KeyBoard.TypeText(text);
            }
            else
            {
                //// select all and delete current text typing
                Manager.Current.Desktop.KeyBoard.KeyDown(System.Windows.Forms.Keys.Control);
                Manager.Current.Desktop.KeyBoard.KeyPress(System.Windows.Forms.Keys.A);
                Manager.Current.Desktop.KeyBoard.KeyUp(System.Windows.Forms.Keys.Control);
                Manager.Current.Desktop.KeyBoard.KeyPress(System.Windows.Forms.Keys.Back);
            }
            
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
        /// Waits for items to appear.
        /// </summary>
        public void WaitForItemsToAppear()
        {
            Manager.Current.Wait.For(() => this.ContainsItems(), 50000);
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

            var items = activeDialog.Find.AllByExpression<HtmlDiv>("class=ng-binding", "ng-bind=~bindIdentifierField(item");
            int count = items.Count;

            if (count == 0)
            {
                items = activeDialog.Find.AllByExpression<HtmlDiv>("ng-click=itemClicked(item)");
                count = items.Count;
            }

            //// if items count is more than 12 elements, then you need to scroll
            if (count > 12)
            {
                items[count - 1].Wait.ForExists();
                items[count - 1].Wait.ForVisible();
                items[count - 1].ScrollToVisible();
            }
           
            bool isCountCorrect = expected == items.Count;
            return isCountCorrect;
        }

        /// <summary>
        /// Verifies the collection contains any items
        /// </summary>
        /// <returns>True or False depending on the items count.</returns>
        public bool ContainsItems()
        {
            ActiveBrowser.RefreshDomTree();
            var activeDialog = this.EM.Widgets.FeatherWidget.ActiveTab.AssertIsPresent("Content container");

            var items = activeDialog.Find.AllByExpression<HtmlDiv>("class=ng-binding", "ng-bind=~bindIdentifierField(item");
            int count = items.Count;

            if (count > 0)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Verifies if given tab is selected.
        /// </summary>
        /// <param name="tabName">Tab name.</param>
        public void VerifySelectedTab(string tabName)
        {
            var activeDialog = this.EM.Widgets.FeatherWidget.ActiveTab.AssertIsPresent("Content container");
            var selectedTab = activeDialog.ChildNodes.Where(c => c.InnerText.Contains(tabName));
            Assert.IsNotNull(selectedTab);
        }

        /// <summary>
        /// Verifies selected items in All tab.
        /// </summary>
        /// <param name="itemPrefixName">Selected item prefix name.</param>
        /// <param name="itemNames">Selected item names.</param>
        public void VerifySelectedItemsInAllTab(string itemPrefixName, params string[] selectedItemNames)
        {
            var archorList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlAnchor>("ng-repeat=item in items");
            int archorListCount = archorList.Count;

            for (int i = 0; i < archorListCount; i++)
            {
                if (selectedItemNames.Contains(archorList[i].InnerText))
                {
                    var inputCheckbox = archorList[i].ChildNodes.Where(c => c.ContainsAttribute("checked")).SingleOrDefault();
                    Assert.IsNotNull(inputCheckbox);

                    var spanElement = archorList[i].ChildNodes.Where(c => c.InnerText.Contains(itemPrefixName + i)).SingleOrDefault();
                    Assert.IsNotNull(spanElement);
                }
            }
        }

        /// <summary>
        /// Reorders the selected items.
        /// </summary>
        /// <param name="expectedOrder">The expected order.</param>
        /// <param name="selectedItemNames">The selected item names.</param>
        public void ReorderSelectedItems(string[] expectedOrder, string[] selectedItemNames, Dictionary<int, int> reorderedIndexMapping)
        {
            var activeDialog = this.EM.Widgets.FeatherWidget.ActiveTab.AssertIsPresent("Content container");
            var divList = activeDialog.Find.AllByExpression<HtmlDiv>("ng-repeat=item in items");
         
            for (int i = 0; i < divList.Count; i++)
            {
                Assert.AreEqual(selectedItemNames[i], divList[i].InnerText, selectedItemNames[i] + "is not positioned correctly in Selected tab");
            }

            var spanList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlSpan>("class=handler list-group-item-drag");

            foreach (KeyValuePair<int, int> reorderingPair in reorderedIndexMapping)
            {
                spanList[reorderingPair.Key].DragTo(spanList[reorderingPair.Value]);
            }

            activeDialog.Refresh();
            var reorderedDivList = activeDialog.Find.AllByExpression<HtmlDiv>("ng-repeat=item in items");
         
            for (int i = 0; i < reorderedDivList.Count; i++)
            {
                Assert.AreEqual(expectedOrder[i], reorderedDivList[i].InnerText, expectedOrder[i] + " is not reordered correctly"); 
            }
        }

        /// <summary>
        /// Selects the more link.
        /// </summary>
        /// <param name="name">The name.</param>
        public void SelectMoreLink(string name)
        {
            var link = this.EM.Widgets.FeatherWidget.Find.ByCustom<HtmlAnchor>(a => a.InnerText.Contains(name));
            link.AssertIsPresent("more link is not present");
            link.Click();
        }

        /// <summary>
        /// Verifies search result span element.
        /// </summary>
        /// <param name="expectedSpanCount">expected count of span elements</param>
        /// <param name="isHiddenSpan">should span decorations be hidden</param>
        public void VerifyReorderingIconVisibility(int expectedSpanCount, bool isHiddenSpan)
        {
            ActiveBrowser.RefreshDomTree();

            ICollection<HtmlSpan> spanList = null;
            if (isHiddenSpan)
            {
                spanList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlSpan>("class=handler list-group-item-drag ng-hide");
            }
            else
            {
                spanList = this.EM.Widgets.FeatherWidget.Find.AllByExpression<HtmlSpan>("class=handler list-group-item-drag");
            }

            int actualSpanCount = spanList.Count;

            Assert.AreEqual(expectedSpanCount, actualSpanCount, "Expected and actual count of span elements are not equal.");
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
            var saveButton = this.EM.Widgets.FeatherWidget.SaveButton;

            bool result = saveButton != null && saveButton.IsVisible();

            return result;
        }
    }
}