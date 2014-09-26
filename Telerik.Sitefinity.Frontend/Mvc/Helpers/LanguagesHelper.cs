using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helpers for working with defined front-end and back-end languages
    /// </summary>
    public static class LanguagesHelper
    {
        /// <summary>
        /// Gets the defined front-end languages.
        /// </summary>
        /// <returns></returns>
        public static string GetDefinedFrontendLanguages()
        {
            var appSettings = SystemManager.CurrentContext.AppSettings;
            var languages = appSettings.DefinedFrontendLanguages.Select(l => l.Name);

            var serialzier = new JavaScriptSerializer();
            var serialziedLanguages = serialzier.Serialize(languages);

            return serialziedLanguages;
        }
    }
}
