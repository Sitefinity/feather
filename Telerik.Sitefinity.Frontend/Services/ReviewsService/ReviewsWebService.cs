using System;
using System.Linq;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Security.Claims;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;
using Telerik.Sitefinity.Services.Comments.DTO;
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
        /// <see cref="AuthorReviewedViewModel" /> object.
        /// </returns>
        [AddHeader(ContentType = MimeTypes.Json)]
        public AuthorReviewedViewModel Get(AuthorReviewedGetRequest request)
        {
            var authorComments = CommentsUtilitiesReflector.GetCommentsByThreadForCurrentAuthorWithRating(request.ThreadKey);

            return new AuthorReviewedViewModel() { AuthorAlreadyReviewed = authorComments.Any() };
        }

        #endregion

        #region Create comment/review

        /// <summary>
        /// Posts the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thread is closed.
        /// or
        /// Comment cannot be submitted at the moment. Please refresh the page and try again.
        /// </exception>
        [AddHeader(ContentType = MimeTypes.Json)]
        public ReviewViewModel Post(ReviewCreateRequest request)
        {
            var commentCreateRequest = new CommentCreateRequest()
            {
                Captcha = request.Captcha,
                CustomData = request.CustomData,
                Email = request.Email,
                Message = request.Message,
                Name = request.Name,
                Rating = request.Rating,
                Thread = request.Thread,
                ThreadKey = request.ThreadKey
            };

            var commentResponse = this.PostInternal(commentCreateRequest);

            var result = new ReviewViewModel()
            {
                AuthorIpAddress = commentResponse.AuthorIpAddress,
                CustomData = commentResponse.CustomData,
                DateCreated = commentResponse.DateCreated,
                Email = commentResponse.Email,
                Key = commentResponse.Key,
                Message = commentResponse.Message,
                Name = commentResponse.Name,
                ProfilePictureThumbnailUrl = commentResponse.ProfilePictureThumbnailUrl,
                ProfilePictureUrl = commentResponse.ProfilePictureUrl,
                Rating = commentResponse.Rating,
                Status = commentResponse.Status,
                ThreadKey = commentResponse.ThreadKey
            };

            return result;
        }

        private CommentResponse PostInternal(CommentCreateRequest request)
        {
            CommentsWebServiceReflector.Validate(request);

            CommentResponse result;
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

        private CommentResponse SubmitCommentInternal(CommentCreateRequest commentData, IThread thread, IAuthor author)
        {
            var cs = SystemManager.GetCommentsService();
            var currentConfig = CommentsUtilitiesReflector.GetThreadConfigByType(thread.Type, thread.Key);

            if (currentConfig.RequiresAuthentication)
            {
                if (author.Key.IsNullOrWhitespace() || author.Key == Guid.Empty.ToString())
                {
                    throw new InvalidOperationException("Comment cannot be submitted at the moment. Please refresh the page and try again.");
                }
            }

            if (currentConfig.EnableRatings)
            {
                if (commentData.Rating == null)
                {
                    throw new InvalidOperationException("A message displayed when ratings are allowed and a comment is submitted without rating.");
                }

                if (CommentsUtilitiesReflector.GetCommentsByThreadForCurrentAuthorWithRating(thread.Key).Any())
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
