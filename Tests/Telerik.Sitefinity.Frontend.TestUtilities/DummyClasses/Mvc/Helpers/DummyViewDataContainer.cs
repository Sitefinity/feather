using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Helpers
{
    /// <summary>
    /// This class is used as a dummy implementation of the IViewDataContainer used for the HtmlHelpers tests
    /// </summary>
    public class DummyViewDataContainer : IViewDataContainer
    {
        public ViewDataDictionary ViewData
        {
            get
            {
                return this.viewData;
            }

            set
            {
                this.viewData = value;
            }
        }

        private ViewDataDictionary viewData = new ViewDataDictionary();
    }
}
