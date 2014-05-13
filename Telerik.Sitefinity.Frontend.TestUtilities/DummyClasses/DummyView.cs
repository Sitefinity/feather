using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    public class DummyView : IView
    {
        public string InnerHtml { get; set; }

        public void Render(ViewContext viewContext, System.IO.TextWriter writer)
        {
            writer.Write(InnerHtml);
        }
    }
}
