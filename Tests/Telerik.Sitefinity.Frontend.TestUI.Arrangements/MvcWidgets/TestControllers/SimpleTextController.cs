using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets.TestControllers
{
    /// <summary>
    /// This class represents a simple MVC widget which has an action that returns text.
    /// </summary>
    [ControllerToolboxItem(Name = "SimpleWidget", Title = "SimpleWidget", SectionName = "MVC")]
    public class SimpleTextController : Controller
    {
        /// <summary>
        /// Gets or sets the dummy text.
        /// </summary>
        /// <value>
        /// The dummy text.
        /// </value>
        public string DummyText { get; set; }

        /// <summary>
        /// Gets the dummy sample text
        /// </summary>
        /// <returns>The dummy text.</returns>
        public string Index()
        {
            return this.DummyText;
        }
    }
}
