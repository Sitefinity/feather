using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Helpers
{
    [ObjectInfo(typeof(DummyLocalizationControllerResources), Title = "DummyLocalizationControllerResources", Description = "DummyLocalizationControllerResourcesDescription")]
    public class DummyLocalizationControllerResources : Resource
    {
        /// <summary>
        /// Resources for Comments
        /// </summary>
        [ResourceEntry("DummyResource",
            Value = "Dummy Resource",
            Description = "Some dummy resource.",
            LastModified = "2014/05/20")]
        public string DummyResource
        {
            get
            {
                return this["DummyResource", System.Globalization.CultureInfo.InvariantCulture];
            }
        }
    }
}
