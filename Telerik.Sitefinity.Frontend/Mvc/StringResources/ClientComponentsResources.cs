using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend.Mvc.StringResources
{
    /// <summary>
    /// Localizable strings for the client components.
    /// </summary>
    [ObjectInfo(typeof(ClientComponentsResources), Title = "ClientComponentsResourcesTitle", Description = "ClientComponentsResourcesDescription")]
    public class ClientComponentsResources : Resource
    {
        /// <summary>
        /// Insert hyperlink
        /// </summary>
        [ResourceEntry("InsertHyperlink",
            Value = "Insert hyperlink",
            Description = "Insert hyperlink",
            LastModified = "2015/03/21")]
        public string InsertHyperlink
        {
            get
            {
                return this["InsertHyperlink"];
            }
        }

        /// <summary>
        /// Insert image
        /// </summary>
        [ResourceEntry("InsertImage",
            Value = "Insert image",
            Description = "Insert image",
            LastModified = "2015/03/21")]
        public string InsertImage
        {
            get
            {
                return this["InsertImage"];
            }
        }

        /// <summary>
        /// Insert file
        /// </summary>
        [ResourceEntry("InsertFile",
            Value = "Insert file",
            Description = "Insert file",
            LastModified = "2015/03/21")]
        public string InsertFile
        {
            get
            {
                return this["InsertFile"];
            }
        }

        /// <summary>
        /// phrase: sfAction attribute is required!
        /// </summary>
        [ResourceEntry("SfActionAttrRequired",
            Value = "sfAction attribute is required!",
            Description = "phrase: sfAction attribute is required!",
            LastModified = "2015/03/23")]
        public string SfActionAttrRequired
        {
            get
            {
                return this["SfActionAttrRequired"];
            }
        }
        
        /// <summary>
        /// No items have been created yet.
        /// </summary>
        [ResourceEntry("NoItemsCreated",
            Value = "No items have been created yet. [From Res.Get]",
            Description = "No items have been created yet.",
            LastModified = "2015/03/21")]
        public string NoItemsCreated
        {
            get
            {
                return this["NoItemsCreated"];
            }
        }
    }
}
