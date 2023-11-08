using System.Linq;
using System.Web.Script.Serialization;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries.Configuration;
using Telerik.Sitefinity.Modules.Libraries.Images;
using Telerik.Sitefinity.Web.UI.ContentUI.Views.Backend.Detail.Definitions;
using Telerik.Sitefinity.Web.UI.Fields.Config;
using Telerik.Sitefinity.Web.UI.Fields.Definitions;

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
                    var singleImageUploadDefinition = libratiesConfig.ContentViewControls[ImagesDefinitions.BackendImagesDefinitionName]?.Views[ImagesDefinitions.SingleImageUploadDetailsView] as DetailFormViewDefinition;
                    var altTextRequired = (singleImageUploadDefinition?.Sections.First(s => s.Name == "MainSection")?.Fields.First(f => f.FieldName == nameof(Image.AlternativeText)) as TextFieldDefinition)?.Validation?.Required.GetValueOrDefault();

                    settings = new
                    {
                        AllowedExensionsSettings = libratiesConfig.Images.AllowedExensionsSettings,
                        EnableAllLanguagesSearch = libratiesConfig.EnableAllLanguagesSearch,
                        EnableSelectedFolderSearch = libratiesConfig.EnableSelectedFolderSearch,
                        AltTextRequired = altTextRequired
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
                        EnableSelectedFolderSearch = libratiesConfig.EnableSelectedFolderSearch,
                        AllowedExensions = libratiesConfig.Documents.AllowedExensions
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
