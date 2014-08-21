using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers;
using Telerik.Sitefinity.TestUI.Core.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework
{
    /// <summary>
    /// Entry class for the fluent API for Batch Automated Testing.
    /// </summary>
    public static class BATFrontend
    {
        /// <summary>
        /// Provides access to wrappers.
        /// </summary>
        /// <param name="url">The base url.</param>
        /// <returns>New instance of wrappers facade.</returns>
        public static WrappersFacade Wrappers(Telerik.Sitefinity.TestUI.Core.Configuration.ConfigredUrls url = Telerik.Sitefinity.TestUI.Core.Configuration.ConfigredUrls.NotSet)
        {
            BATFrontend.ChangeBaseUrl(url);
            return new WrappersFacade();
        }

        private static void ChangeBaseUrl(Telerik.Sitefinity.TestUI.Core.Configuration.ConfigredUrls url)
        {
            var configUrl = ConfigurationHelper.GetConfiurationSettings(url);
            Manager.Current.Settings.Web.SetBaseUrl(configUrl);
        }
    }
}
