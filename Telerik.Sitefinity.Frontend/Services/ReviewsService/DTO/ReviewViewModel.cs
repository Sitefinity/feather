using System;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO
{
    /// <summary>
    /// <c>CommentResponse</c>. Represents the <see cref="IComment"/> object that should be returned.
    /// </summary>
    internal class ReviewViewModel
    {
        /// <summary>
        /// Gets or sets the key of the <see cref="IComment"/> object.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the name of the author of the <see cref="IComment"/> object.
        /// </summary>
        /// <value>
        /// The name of the author.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the body of the message of the <see cref="IComment"/> object.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date when the <see cref="IComment"/> object was created.
        /// </summary>
        /// <value>
        /// The date created.
        /// </value>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author profile picture URL.
        /// </summary>
        /// <value>
        /// The profile picture URL.
        /// </value>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author profile picture thumbnail URL.
        /// </summary>
        /// <value>
        /// The profile picture thumbnail URL.
        /// </value>
        public string ProfilePictureThumbnailUrl { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author email.
        /// This is sensitive information that is returned if the request is made from authenticated backend user.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the key of the <see cref="IThread"/> object that the <see cref="IComment"/> object is associated with.
        /// </summary>
        /// <value>
        /// The thread key.
        /// </value>
        public string ThreadKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object author IP address.
        /// This is sensitive information that is returned if the request is made from authenticated backend user.
        /// </summary>
        /// <value>
        /// The author ip address.
        /// </value>
        public string AuthorIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object custom data.
        /// </summary>
        /// <remarks>
        /// This custom data is an extensibility point that can be used to store custom data for the <see cref="IComment"/> object.
        /// </remarks>
        /// <value>
        /// The custom data.
        /// </value>
        public string CustomData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IComment"/> object rating.
        /// </summary>
        /// <remarks>
        /// This rating field is used in the context of reviews behaviour.
        /// </remarks>
        /// <value>
        /// The custom data.
        /// </value>
        public decimal? Rating { get; set; }
    }
}
