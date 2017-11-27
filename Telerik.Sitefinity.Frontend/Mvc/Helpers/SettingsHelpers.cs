using System.Linq;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Modules.Libraries.Configuration;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// This class contains helpers for working with widget settings.
    /// </summary>
    public static class SettingsHelpers
    {
        /// <summary>
        /// Gets the media settings for specific media type
        /// </summary>
        /// <returns>Serialized media settings</returns>
        public static string GetMediaSettings(string mediaType)
        {
            var libratiesConfig = Config.Get<LibrariesConfig>();
            object settings;

            switch (mediaType)
            {
                case "Image":
                    settings = new
                    {
                        AllowedExensionsSettings = libratiesConfig.Images.AllowedExensionsSettings,
                        EnableAllLanguagesSearch = libratiesConfig.EnableAllLanguagesSearch,
                        EnableSelectedFolderSearch = libratiesConfig.EnableSelectedFolderSearch
                    };
                    break;
                case "Video":
                    settings = new
                    {
                        AllowedExensionsSettings = libratiesConfig.Videos.AllowedExensionsSettings,
                        EnableAllLanguagesSearch = libratiesConfig.EnableAllLanguagesSearch,
                        EnableSelectedFolderSearch = libratiesConfig.EnableSelectedFolderSearch
                    };
                    break;
                case "Document":
                    settings = new
                    {
                        AllowedExensionsSettings = libratiesConfig.Documents.AllowedExensionsSettings,
                        EnableAllLanguagesSearch = libratiesConfig.EnableAllLanguagesSearch,
                        EnableSelectedFolderSearch = libratiesConfig.EnableSelectedFolderSearch
                    };
                    break;
                default:
                    settings = null;
                    break;
            }

            var serialzier = new JavaScriptSerializer();
            return serialzier.Serialize(settings);
        }
    }
}
