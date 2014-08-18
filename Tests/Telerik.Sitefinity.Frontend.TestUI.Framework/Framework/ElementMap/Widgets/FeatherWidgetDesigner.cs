using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap.Widgets
{
    public class FeatherWidgetDesigner : HtmlElementContainer
    {
        public FeatherWidgetDesigner(Find find)
            : base(find)
        {
        }

        /// <summary>
        /// Gets the widget title from the designer
        /// </summary>
        public HtmlControl WidgetTitleText
        {
            get
            {
                return this.Get<HtmlControl>("class=modal-title");
            }
        }

        /// <summary>
        /// Gets the widget input field label 
        /// </summary>
        public HtmlControl Label
        {
            get
            {
                return this.Get<HtmlControl>("tagname=label", "class=ng-binding");
            }
        }

        /// <summary>
        /// Gets the Save button of the widget designer
        /// </summary>
        public HtmlButton SaveButton
        {
            get
            {
                return this.Get<HtmlButton>("tagname=button", "class=btn btn-primary ng-scope");
            }
        }

        /// <summary>
        /// Gets the cancel button of the widget designer
        /// </summary>
        public HtmlAnchor CancelButton
        {
            get
            {
                return this.Get<HtmlAnchor>("tagname=a", "class=btn btn-link ng-scope");
            }
        }

        /// <summary>
        /// Gets the close button of the widget designer
        /// </summary>
        public HtmlButton CloseButton
        {
            get
            {
                return this.Get<HtmlButton>("type=button", "class=close");
            }
        }

        /// <summary>
        /// Gets the input field of Dummy mvc widget
        /// </summary>
        public HtmlInputText DummyWidgetInput
        {
            get
            {
                return this.Get<HtmlInputText>("id=prop-DummyText");
            }
 
        }
    }
}
