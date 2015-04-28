using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations.ActionFilters;
using Telerik.Sitefinity.Modules.Pages;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers
{
    /// <summary>
    /// This class represents a dummy controller that returns registered script resource.
    /// </summary>
    public class DummyScriptController : Controller
    {
        /// <summary>
        /// Gets or sets the referred script source.
        /// </summary>
        /// <value>
        /// The script source.
        /// </value>
        public string ScriptSource { get; set; }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>
        /// The name of the section.
        /// </value>
        public string SectionName { get; set; }

        /// <summary>
        /// This action provides jQuery script reference.
        /// </summary>
        /// <returns></returns>
        [ExecutionRegistrationFilter]
        public ContentResult Index()
        {
            var dummyHttpContext = this.HttpContext;
            var dummyViewContext = new ViewContext();
            dummyViewContext.HttpContext = dummyHttpContext;
            var dummyViewDataContainer = (IViewDataContainer)new ViewPage();

            var htmlHelper = new System.Web.Mvc.HtmlHelper(dummyViewContext, dummyViewDataContainer);

            if (!this.ScriptSource.IsNullOrEmpty())
            {
                return this.Content(htmlHelper.Script(this.ScriptSource, throwException: false, sectionName: this.SectionName).ToString());
            }
            else
            {
                return this.Content(htmlHelper.Script(ScriptRef.JQuery).ToString());
            }
        }
    }
}
