using System;
using System.Linq;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;
using Telerik.Sitefinity.Services.Comments.Proxies;
using Telerik.Sitefinity.Web.Services;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService
{
    /// <summary>
    /// This class provides reviews status.
    /// </summary>
    internal class ReviewsWebService : Service
    {
        #region Get Author already reviewed

        /// <summary>
        /// Gets if the author already has review for current thread.
        /// </summary>
        /// <param name="request">The reviews get request.</param>
        /// <returns>
        /// <see cref="AuthorAlreadyReviewedViewModel" /> object.
        /// </returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public AuthorAlreadyReviewedViewModel Get(AuthorAlreadyReviewedGetRequest request)
        {
            var authorAlreadyCommented = CommentsUtilitiesReflector.GetCommentsByThreadForCurrentAuthorWithRating(request.ThreadKey);

            return new AuthorAlreadyReviewedViewModel() { AuthorAlreadyReviewed = authorAlreadyCommented };
        }

        #endregion

        #region Create comment/review

        /// <summary>
        /// Creates a comment or review.
        /// </summary>
        /// <param name="request">The create comment review post request.</param>
        /// <returns>
        /// <see cref="CreateCommentReviewViewModel" /> object.
        /// </returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public CreateCommentReviewViewModel Post(CreateCommentReviewPostRequest request)
        {
            CommentsWebServiceReflector.Validate(request);

            CreateCommentReviewViewModel result;
            try
            {
                var author = CommentsUtilitiesReflector.GetAuthor(request);
                var cs = SystemManager.GetCommentsService();
                var thread = cs.GetThread(request.ThreadKey);

                if (thread == null)
                {
                    request.Thread.Key = request.ThreadKey;

                    CommentsWebServiceReflector.Validate(request.Thread);
                    CommentsWebServiceReflector.ValidatePostRequest(request.Thread.Type, request.Captcha, false);

                    var group = cs.GetGroup(request.Thread.GroupKey);
                    if (group == null)
                    {
                        CommentsWebServiceReflector.Validate(request.Thread.Group);

                        request.Thread.Group.Key = request.Thread.GroupKey;

                        var groupProxy = new GroupProxy(request.Thread.Group.Name, request.Thread.Group.Description, author)
                        {
                            Key = request.Thread.Group.Key
                        };
                        group = cs.CreateGroup(groupProxy);
                    }

                    var threadProxy = new ThreadProxy(request.Thread.Title, request.Thread.Type, group.Key, author)
                    {
                        Key = request.Thread.Key,
                        Language = request.Thread.Language,
                        DataSource = request.Thread.DataSource,
                        Behavior = request.Thread.Behavior
                    };

                    thread = cs.CreateThread(threadProxy);
                }
                else
                {
                    if (thread.IsClosed)
                        throw new InvalidOperationException("Thread is closed.");

                    CommentsWebServiceReflector.ValidatePostRequest(thread.Type, request.Captcha, false);
                }

                result = this.SubmitCommentInternal(request, thread, author);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException("Comment cannot be submitted at the moment. Please refresh the page and try again.", ex);
            }

            ServiceUtility.DisableCache();

            return result;
        }

        private CreateCommentReviewViewModel SubmitCommentInternal(CreateCommentReviewPostRequest commentData, IThread thread, IAuthor author)
        {
            var cs = SystemManager.GetCommentsService();
            var currentConfig = CommentsUtilitiesReflector.GetThreadConfigByType(thread.Type);

            if (currentConfig.RequiresAuthentication)
            {
                if (author.Key.IsNullOrWhitespace() || author.Key == Guid.Empty.ToString())
                {
                    throw new InvalidOperationException("Comment cannot be submitted at the moment. Please refresh the page and try again.");
                }
            }

            if (currentConfig.EnableRatings && commentData.Rating == null)
            {
                throw new InvalidOperationException("A message displayed when ratings are allowed and a comment is submitted without rating.");
            }

            if (commentData.Rating != null)
            {
                if (CommentsUtilitiesReflector.GetCommentsByThreadForCurrentAuthorWithRating(thread.Key))
                {
                    throw new InvalidOperationException("Only one comment with rating is allowed per user.");
                }
            }

            var authorIp = CommentsUtilitiesReflector.GetIpAddressFromCurrentRequest();
            var commentProxy = new CommentProxy(commentData.Message, thread.Key, author, authorIp, commentData.Rating);
            commentProxy.CustomData = commentData.CustomData;

            if (currentConfig.RequiresApproval)
                commentProxy.Status = StatusConstants.WaitingForApproval;
            else
                commentProxy.Status = StatusConstants.Published;

            IComment newComment = cs.CreateComment(commentProxy);

            var result = CommentsUtilitiesReflector.GetCommentResponse(newComment, ClaimsManager.GetCurrentIdentity().IsBackendUser);
            
            ServiceUtility.DisableCache();

            return result;
        }

        #endregion
    }
}
