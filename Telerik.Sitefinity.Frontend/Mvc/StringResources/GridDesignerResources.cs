using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.StringResources
{
    /// <summary>
    /// Localizable strings for the Grid's designer.
    /// </summary>
    [ObjectInfo(typeof(GridDesignerResources), Title = "GridDesignerResourcesTitle", Description = "GridDesignerResourcesDescription")]
    public class GridDesignerResources : Resource
    {
        /// <summary>
        /// Title for the grid designer resources resources class.
        /// </summary>
        [ResourceEntry("GridResourcesTitle",
            Value = "Grid designer resources",
            Description = "Title for the grid designer resources resources class.",
            LastModified = "2015/06/23")]
        public string GridResourcesTitle
        {
            get
            {
                return this["GridResourcesTitle"];
            }
        }

        /// <summary>
        /// Description for the grid designer resources class.
        /// </summary>
        [ResourceEntry("GridDesignerResourcesDescription",
            Value = "Localizable strings for the Grid designer.",
            Description = "Description for the grid designer resources class.",
            LastModified = "2015/06/23")]
        public string GridDesignerResourcesDescription
        {
            get
            {
                return this["GridDesignerResourcesDescription"];
            }
        }

        /// <summary>
        /// Error!
        /// </summary>
        [ResourceEntry("Error",
            Value = "Error!",
            Description = "Error!",
            LastModified = "2015/06/23")]
        public string Error
        {
            get
            {
                return this["Error"];
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        [ResourceEntry("Cancel",
            Value = "Cancel",
            Description = "Cancel",
            LastModified = "2015/06/23")]
        public string Cancel
        {
            get
            {
                return this["Cancel"];
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        [ResourceEntry("Save",
            Value = "Save",
            Description = "Save",
            LastModified = "2015/06/23")]
        public string Save
        {
            get
            {
                return this["Save"];
            }
        }

        /// <summary>
        /// Labels
        /// </summary>
        [ResourceEntry("Labels",
            Value = "Labels",
            Description = "Labels",
            LastModified = "2015/06/23")]
        public string Labels
        {
            get
            {
                return this["Labels"];
            }
        }

        /// <summary>
        /// Apply custom labels to the columns for your convenience only.
        /// </summary>
        [ResourceEntry("LabelForYourConvenienceOnly",
            Value = "Apply custom labels to the columns for your convenience only.",
            Description = "phrase: Apply custom labels to the columns for your convenience only.",
            LastModified = "2015/07/08")]
        public string LabelForYourConvenienceOnly
        {
            get
            {
                return this["LabelForYourConvenienceOnly"];
            }
        }

        /// <summary>
        /// Labels
        /// </summary>
        [ResourceEntry("Classes",
            Value = "Classes",
            Description = "Classes",
            LastModified = "2015/06/23")]
        public string Classes
        {
            get
            {
                return this["Classes"];
            }
        }
    }
}
