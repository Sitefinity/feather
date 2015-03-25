using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This control is responsible for rendering the link tags.
    /// </summary>
    public class StyleSheetRenderer : LiteralControl
    {
        /// <inheritDoc/>
        protected override void OnPreRender(EventArgs e)
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            this.Text = ResourceHelper.RenderAllStylesheets(context);
        }
    }
}
