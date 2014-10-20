using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.ObjectModel;
using ArtOfTest.WebAii.TestTemplates;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    /// <summary>
    /// Feather widget designer screen elements.
    /// </summary>
    public class FeatherWidgetDesigner : HtmlElementContainer
    {
        /// <summary>
        /// Initializes a new instance of the FeatherWidgetDesigner class.
        /// </summary>
        /// <param name="find">Find object for the current window.</param>
        public FeatherWidgetDesigner(Find find)
            : base(find)
        {
        }

        /// <summary>
        /// Gets the widget title from the designer.
        /// </summary>
        /// <value>Widget title.</value>
        public HtmlControl WidgetTitleText
        {
            get
            {
                return this.Get<HtmlControl>("class=modal-title");
            }
        }

        /// <summary>
        /// Gets the widget input field label. 
        /// </summary>
        /// <value>The label.</value>
        public HtmlControl Label
        {
            get
            {
                return this.Get<HtmlControl>("tagname=label", "class=ng-binding");
            }
        }

        /// <summary>
        /// Gets the Save button of the widget designer.
        /// </summary>
        /// <value>Save button.</value>
        public HtmlButton SaveButton
        {
            get
            {
                return this.Get<HtmlButton>("tagname=button", "class=btn btn-primary pull-left ng-scope");
            }
        }

        /// <summary>
        /// Gets the cancel button of the widget designer.
        /// </summary>
        /// <value>Cancel button.</value>
        public HtmlAnchor CancelButton
        {
            get
            {
                return this.Get<HtmlAnchor>("tagname=a", "class=btn btn-link pull-left ng-scope");
            }
        }

        /// <summary>
        /// Gets the advanced button.
        /// </summary>
        /// <value>The advanced button.</value>
        public HtmlAnchor AdvancedButton
        {
            get
            {
                return this.Get<HtmlAnchor>("ng-hide=isCurrentView('PropertyGrid')");
            }
        }

        /// <summary>
        /// Gets the selector button.
        /// </summary>
        /// <value>The selector button.</value>
        public HtmlAnchor SelectorButton
        {
            get
            {
                return this.Get<HtmlAnchor>("ng-hide=isCurrentView('Selector')");
            }
        }

        /// <summary>
        /// Gets the close button of the widget designer.
        /// </summary>
        /// <value>Close button.</value>
        public HtmlButton CloseButton
        {
            get
            {
                return this.Get<HtmlButton>("type=button", "class=close");
            }
        }

        /// <summary>
        /// Gets the input field of Dummy MVC widget.
        /// </summary>
        /// <value>Widget input.</value>
        public HtmlInputText DummyWidgetInput
        {
            get
            {
                return this.Get<HtmlInputText>("id=prop-DummyText");
            }
        }

        /// <summary>
        /// Gets the select button.
        /// </summary>
        /// <value>The select button.</value>
        public HtmlButton SelectButton
        {
            get
            {
                return this.Get<HtmlButton>("id=openSelectorBtn");
            }
        }

        /// <summary>
        /// Gets the selected tab.
        /// </summary>
        /// <value>The selected tab.</value>
        public HtmlAnchor SelectedTab
        {
            get
            {
                return this.Get<HtmlAnchor>("class=k-link", "innertext=~Selected");
            }
        }

        /// <summary>
        /// Gets the done button.
        /// </summary>
        /// <value>The done button.</value>
        public HtmlButton DoneButton
        {
            get
            {
                return this.Get<HtmlButton>("ng-click=doneSelecting()");
            }
        }

        /// <summary>
        /// Gets the search input.
        /// </summary>
        /// <value>The search input.</value>
        public HtmlInputText SearchInput
        {
            get
            {
                return this.Get<HtmlInputText>("ng-model=~filter.search");
            }
        }

        /// <summary>
        /// Gets the content container.
        /// </summary>
        /// <value>The content container.</value>
        public HtmlDiv ContentContainer
        {
            get
            {
                return this.Find.ById<HtmlDiv>("viewsPlaceholder");
            }
        }

        public HtmlDiv ActiveTab
        {
            get
            {
                return this.Get<HtmlDiv>("class=k-content k-state-active");
            }
        }
    }
}
