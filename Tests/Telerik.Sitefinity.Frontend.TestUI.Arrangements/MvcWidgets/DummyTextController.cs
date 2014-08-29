using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets
{
    /// <summary>
    /// This class represents a dummy MVC widget which has an action that returns simple text.
    /// </summary>
    public class DummyTextController : Controller
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
