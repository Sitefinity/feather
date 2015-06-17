using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// This class is used for setting up the Core project before the application has been started
    /// </summary>
    public static class ApplicationPreStart
    {
        /// <summary>
        /// The method that initializes all the preparations
        /// </summary>
        public static void Init()
        {
            if (!RouteTable.Routes.Contains(ApplicationPreStart.UITestsServiceRoute))
            {
                RouteTable.Routes.Add("ui-tests", ApplicationPreStart.UITestsServiceRoute);
            }
        }

        internal static readonly ServiceRoute UITestsServiceRoute =
            new ServiceRoute(TestsArrangemetsService.UiTestsWebServiceUrl, new WebServiceHostFactory(), typeof(TestsArrangemetsService));
    }
}
