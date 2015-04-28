namespace Telerik.Sitefinity.Frontend.Services.FilesService.DTO
{
    /// <summary>
    /// The view model
    /// </summary>
    internal class FilesViewModel
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
    }
}
