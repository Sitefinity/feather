using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class represents one resource item
    /// </summary>
    internal class ResourceItem
    {
        /// <summary>
        /// Gets or sets the source of the resource item.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ResourceKey { get; set; }

        /// <summary>
        /// Gets or sets the attributes that will be set to the rendered resource.
        /// </summary>
        /// <value>The attributes that will be set to the rendered resource.</value>
        public List<KeyValuePair<string, string>> Attributes { get; set; }

        public ResourceItem(string resourceKey, List<KeyValuePair<string, string>> attributes) 
        {
            ResourceKey = resourceKey;
            Attributes = attributes;
        }
    }
}
