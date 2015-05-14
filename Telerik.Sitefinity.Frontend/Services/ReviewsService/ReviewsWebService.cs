using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService
{
    /// <summary>
    /// This class provides reviews status.
    /// </summary>
    internal class ReviewsWebService : Service
    {
        #region Actions
        
        [AddHeader(ContentType = MimeTypes.Json)]
        public ReviewsViewModel Get(ReviewsGetRequest reviewsGetRequest)
        {
            var result = new ReviewsViewModel()
            {
                AuthorAlreadyReviewed = this.CheckIfAuthorAlreadyCommented(reviewsGetRequest.ThreadKey)
            };

            return result;
        }

        #endregion

        private bool CheckIfAuthorAlreadyCommented(string threadKey)
        {
            bool hasCommented = false;

            Type type = Type.GetType("Telerik.Sitefinity.Modules.Comments.CommentsUtilities, Telerik.Sitefinity");
            MethodInfo method = type.GetMethod("GetCommentsByThreadForCurrentAuthorWithRating", BindingFlags.NonPublic | BindingFlags.Static);
            object commentsObject = method.Invoke(null, new object[] { threadKey, SystemManager.GetCommentsService() });
            var comments = commentsObject as IEnumerable<IComment>;
            if (comments != null)
            {
                hasCommented = comments.Any();
            }

            return hasCommented;
        }
    }
}
