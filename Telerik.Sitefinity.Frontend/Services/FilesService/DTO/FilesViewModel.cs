using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Services.FilesService.DTO
{
    /// <summary>
    /// The view model
    /// </summary>
    internal class FilesViewModel
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IEnumerable<FilesItemViewModel> Items { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }
    }
}
