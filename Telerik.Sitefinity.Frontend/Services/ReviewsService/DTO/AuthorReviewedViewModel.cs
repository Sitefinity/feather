namespace Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO
{
    /// <summary>
    /// The view model used by <see cref="ReviewsWebService"/>.
    /// </summary>
    internal class AuthorReviewedViewModel
    {
        /// <summary>
        /// Gets or sets a value indicating whether author already reviewed.
        /// </summary>
        /// <value>
        /// <c>true</c> if [author already reviewed]; otherwise, <c>false</c>.
        /// </value>
        public bool AuthorAlreadyReviewed { get; set; }
    }
}
