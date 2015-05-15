using System;
using Telerik.Sitefinity.Services.Comments.DTO;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO
{
    /// <summary>
    /// <c>CommentCreateRequest</c> Represents the <see cref="IComment"/> object that should be created.
    /// </summary>
    internal class ReviewCreateRequest
    {
        /// <summary>
        /// Gets or sets the body of the message of the <see cref="IComment"/> object.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object custom data.
        /// </summary>
        /// <remarks>
        /// This custom data is extensibility point that can be used to store some custom data for the <see cref="IComment"/> object.
        /// </remarks>
        /// <value>
        /// The custom data.
        /// </value>
        public string CustomData { get; set; }

        /// <summary>
        /// Gets or sets the key of the <see cref="IThread"/> object that the <see cref="IComment"/> object is associated with.
        /// </summary>
        /// <value>
        /// The thread key.
        /// </value>
        public string ThreadKey { get; set; }

        /// <summary>
        /// Gets or sets the thread information when the parent thread should be created for this <see cref="IComment"/>.
        /// </summary>
        /// <value>
        /// The thread.
        /// </value>
        public ThreadCreateRequest Thread { get; set; }

        /// <summary>
        /// Gets or sets the captcha information.
        /// </summary>
        /// <value>
        /// The captcha data.
        /// </value>
        public CaptchaInfo Captcha { get; set; }

        /// <summary>
        /// Gets or sets the rating data.
        /// </summary>
        /// <remarks>
        /// This rating data is used in the context of reviews behaviour
        /// </remarks>
        /// <value>
        /// The captcha data.
        /// </value>
        public decimal? Rating { get; set; }
    }
}
