using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Views;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Layouts
{
    /// <summary>
    /// This class fakes the <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts.LayoutRenderer"/> class members in fake context. Used for test purposes only.
    /// </summary>
    internal class DummyLayoutRenderer : LayoutRenderer
    {
        #region Properties

        /// <summary>
        /// Gets or sets the inner HTML string with form tag appended to it. Used for test purposes only.
        /// </summary>
        /// <value>
        /// The inner HTML string.
        /// </value>
        public string InnerHtmlStringWithForm { get; set; }

        /// <summary>
        /// Gets or sets the inner HTML string without appended form tag.Used for test purposes only.
        /// </summary>
        /// <value>
        /// The inner HTML string without appended form tag.
        /// </value>
        public string InnerHtmlStringWithoutForm { get; set; }

        /// <summary>
        /// The test keywords
        /// </summary>
        public const string TestKeywords = "TestKeywords";

        #endregion 

        #region Public methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyLayoutRenderer"/> class.
        /// </summary>
        public DummyLayoutRenderer()
        {
            this.InnerHtmlStringWithForm = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n \r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head runat=\"server\">\r\n    <title></title>\r\n</head>\r\n<body><form runat='server'>\r\n    <asp:ScriptManager ID=\"ScriptManager1\" runat=\"server\" />\r\n \r\n        <div id=\"Header\">\r\n            <asp:contentplaceholder id=\"Header\" runat=\"server\" />\r\n        </div>\r\n        <div id=\"Content\">\r\n            <asp:contentplaceholder id=\"Content\" runat=\"server\" />\r\n        </div>\r\n        <div id=\"Footer\">\r\n            <asp:contentplaceholder id=\"Footer\" runat=\"server\" />\r\n        </div>\r\n    </form></body>\r\n</html>";
            this.InnerHtmlStringWithoutForm = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\r\n \r\n<html xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head runat=\"server\">\r\n    <title></title>\r\n</head>\r\n<body>\r\n    <asp:ScriptManager ID=\"ScriptManager1\" runat=\"server\" />\r\n \r\n        <div id=\"Header\">\r\n            <asp:contentplaceholder id=\"Header\" runat=\"server\" />\r\n        </div>\r\n        <div id=\"Content\">\r\n            <asp:contentplaceholder id=\"Content\" runat=\"server\" />\r\n        </div>\r\n        <div id=\"Footer\">\r\n            <asp:contentplaceholder id=\"Footer\" runat=\"server\" />\r\n        </div>\r\n    </body>\r\n</html>";
        }

        /// <summary>
        /// Creates a controller in the mocked context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routeData">The route data.</param>
        /// <returns></returns>
        public new Controller CreateController(RouteData routeData = null)
        {
            Controller controller = new GenericController();
            var context = new HttpContextWrapper(new System.Web.HttpContext(
               new HttpRequest(null, "http://tempuri.org", null),
               new HttpResponse(null)));
            context.Items["CurrentResourcePackage"] = "test";

            var baseTemplateBuilder = (LayoutRenderer)this;

            SystemManager.RunWithHttpContext(
                context, 
                () => { controller = baseTemplateBuilder.CreateController(routeData); });

            return controller;
        }

        /// <summary>
        /// Gets dummy view engine result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="isPartial">if set to <c>true</c> method requests only partial views.</param>
        /// <returns></returns>
        public override ViewEngineResult GetViewEngineResult(ControllerContext context, string viewPath, bool isPartial = false)
        {
            return new ViewEngineResult(new DummyView { InnerHtml = this.InnerHtmlStringWithoutForm }, ViewEngines.Engines.FirstOrDefault());
        }
        #endregion
    }
}
