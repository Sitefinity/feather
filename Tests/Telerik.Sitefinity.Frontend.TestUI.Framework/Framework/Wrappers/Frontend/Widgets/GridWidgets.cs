using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.Common.UnitTesting;
using ArtOfTest.WebAii.Controls.HtmlControls;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers.Frontend
{
    /// <summary>
    /// Grid widgets base actions. 
    /// </summary>
    public class GridWidgets : BaseWrapper
    {
        /// <summary>
        /// Verify new grid widget on the frontend
        /// </summary>
        public void VerifyNewGridWidgetOnTheFrontend(string[] layouts)
        {
            HtmlDiv frontendPageMainDiv = BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().GetPageContent();

            List<HtmlDiv> layoutList = frontendPageMainDiv.Find.AllByExpression<HtmlDiv>("tagname=div", "class=^col-md").ToList<HtmlDiv>();

            for (int i = 0; i < layoutList.Count; i++)
            {
                var isContained = layoutList[i].CssClass.Equals(layouts[i]);
                Assert.IsTrue(isContained, "Layout was not found");
            }
        }

        /// <summary>
        /// Verify old grid widget on the frontend
        /// </summary>
        public void VerifyOldGridWidgetOnTheFrontend(string[] layoutsOld)
        {
            HtmlDiv frontendPageMainDiv = BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().GetPageContent();

            List<HtmlDiv> layoutListOld = frontendPageMainDiv.Find.AllByExpression<HtmlDiv>("tagname=div", "class=^sf_colsOut").ToList<HtmlDiv>();

            var isContained1 = layoutListOld[2].CssClass.Equals(layoutsOld[0]);
            Assert.IsTrue(isContained1, "Layout was not found");

            var isContained2 = layoutListOld[3].CssClass.Equals(layoutsOld[1]);
            Assert.IsTrue(isContained2, "Layout was not found");
        }

        /// <summary>
        /// Verify new grid widget on the frontend
        /// </summary>
        public void VerifyCustomGridWidgetOnTheFrontend(string[] layouts)
        {
            HtmlDiv frontendPageMainDiv = BAT.Wrappers().Frontend().Pages().PagesWrapperFrontend().GetPageContent();

            List<HtmlDiv> layoutList = frontendPageMainDiv.Find.AllByExpression<HtmlDiv>("tagname=div", "class=^sf_colsIn").ToList<HtmlDiv>();

            for (int i = 0; i < layoutList.Count; i++)
            {
                var isContained = layoutList[i].CssClass.Contains(layouts[i]);
                Assert.IsTrue(isContained, "Layout was not found");
            }
        }
    }
}
