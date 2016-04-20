using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements.MvcWidgets;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestUI.Arrangements.Framework;
using Telerik.Sitefinity.TestUtilities.CommonOperations;
using Telerik.Sitefinity.TestUtilities.Utilities;
using MvcServerOperations = Telerik.Sitefinity.Mvc.TestUtilities.CommonOperations.ServerOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// CreateEditWidgetTemplateWhenCombineBackendScriptResourceIsFalse arragement.
    /// </summary>
    public class CreateEditWidgetTemplateWhenCombineBackendScriptResourceIsFalse : ITestArrangement
    {
        /// <summary>
        /// Restart app
        /// </summary>
        [ServerArrangement]
        public void RestartApp()
        {
            ServerOperations.SystemManager().RestartApplication(false);           
            WaitUtils.WaitForSitefinityToStart(HttpContext.Current.Request.Url
                .GetLeftPart(UriPartial.Authority) + (HostingEnvironment.ApplicationVirtualPath.TrimEnd('/') ?? string.Empty));
        }

        [ServerTearDown]
        public void TearDown()
        {
            FeatherServerOperations.Pages().EnableCombineScriptForPages(true);
            ServerOperations.Widgets().DeleteWidgetTemplate("NewsWidgetTemplate");
        }
    }
}
