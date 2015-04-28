namespace Telerik.Sitefinity.Frontend.Services.FilesService.DTO
{
    /// <summary>
    /// The DTO used by <see cref="FilesWebService"/>.
    /// </summary>
    internal class FilesGetRequest
    {
        /// <summary>
        /// Gets or sets the parent path (or null if root).
        /// </summary>
        /// <value>
        /// The parent path.
        /// </value>
        public string ParentPath { get; set; }

        /// <summary>
        /// Gets or sets the file extension.
        /// </summary>
        /// <value>
        /// The file extension.
        /// </value>
        public string FileExtension { get; set; }

        /// <summary>
        /// Gets or sets the skip.
        /// </summary>
        /// <value>
        /// The skip.
        /// </value>
        public int Skip { get; set; }

        /// <summary>
        /// Gets or sets the take.
        /// </summary>
        /// <value>
        /// The take.
        /// </value>
        public int Take { get; set; }
    }
}
