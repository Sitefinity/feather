using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Controls.HtmlControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Frontend
{
    /// <summary>
    /// Grid widgets base actions. 
    /// </summary>
    public class GridWidgets : BaseWrapper
    {
        /// <summary>
        /// Verify grid widget on the frontend
        /// </summary>
        public void VerifyGridWidgetOnTheFrontend(string[] layouts)
        {
            HtmlDiv frontendPageMainDiv = BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().GetPageContent();

            List<HtmlDiv> layoutList = frontendPageMainDiv.Find.AllByExpression<HtmlDiv>("tagname=div", "class=^col-md").ToList<HtmlDiv>();

            for (int i = 0; i < layoutList.Count; i++)
            {
                var isContained = layoutList[i].CssClass.Equals(layouts[i]);
                Assert.IsTrue(isContained, "Layout was not found");
            }
        }
    }
}
