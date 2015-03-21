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
