using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Data;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements.Module
{
    [ObjectInfo(typeof(ModuleTestsResources), ResourceClassId = "ModuleTestsWidgetResources", Title = "ModuleTestsWidgetResourcesTitle", Description = "ModuleTestsWidgetResourcesDescription")]
    public class ModuleTestsResources : Resource
    {
        #region Constructions

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialShareResources" /> class.
        /// </summary>
        public ModuleTestsResources()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialShareResources" /> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public ModuleTestsResources(ResourceDataProvider dataProvider)
            : base(dataProvider)
        {
        }

        #endregion

        /// <summary>
        /// "ca9af596-eaa3-44ed-a654-0e9170266a36".
        /// </summary>
        [ResourceEntry("ModuleTestsResourcesData",
            Value = "ca9af596-eaa3-44ed-a654-0e9170266a36",
            Description = "ca9af596-eaa3-44ed-a654-0e9170266a36",
            LastModified = "2015/12/17")]
        public string ModuleTestsResourcesData
        {
            get
            {
                return this["ModuleTestsResourcesData"];
            }
        }

        public const string ResourceKey = "ModuleTestsResourcesData";
    }
}
