using System.Linq;
using System.ServiceModel.Activation;
﻿using System.Web.Routing;
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
            if(!RouteTable.Routes.Any(x => (x as ServiceRoute) != null && (x as ServiceRoute).Url.StartsWith(TestsArrangemetsService.UiTestsWebServiceUrl)))
            {
                var uiTestsServiceRoute = new ServiceRoute(TestsArrangemetsService.UiTestsWebServiceUrl, new WebServiceHostFactory(), typeof(TestsArrangemetsService));
                RouteTable.Routes.Add("ui-tests", uiTestsServiceRoute);
            }
        }
    }
}
