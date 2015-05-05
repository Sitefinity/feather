namespace Telerik.Sitefinity.Frontend.Services.FilesService.DTO
{
    /// <summary>
    /// A single file item view model used by <see cref="FilesViewModel"/>
    /// </summary>
    internal class FilesItemViewModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is folder.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is folder; otherwise, <c>false</c>.
        /// </value>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the item has children.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the item has children; otherwise, <c>false</c>.
        /// </value>
        public bool HasChildren { get; set; }
    }
}
