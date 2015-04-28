using System;
using System.Linq;

namespace Telerik.Sitefinity.Frontend.Services.ListsService.DTO
{
    /// <summary>
    /// <c>SpecificListsGetRequest</c>Represents a filter for retrieving a specific items of list objects.
    /// </summary>
    internal class SpecificListsGetRequest
    {
        /// <summary>
        /// Gets or sets the list ids.
        /// </summary>
        /// <value>The list ids.</value>
        public Guid[] Ids { get; set; }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string Provider { get; set; }
    }
}
