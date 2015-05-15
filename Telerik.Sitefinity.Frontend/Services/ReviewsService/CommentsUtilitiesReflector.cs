using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Modules.UserProfiles;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Services.Comments;
using Telerik.Sitefinity.Services.Comments.DTO;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService
{
    /// <summary>
    /// This class holds all methods internaly exposed by the CommentsUtilities class in sf.
    /// </summary>
    internal static class CommentsUtilitiesReflector
    {
        /// <summary>
        /// Gets the use captia setting.
        /// </summary>
        /// <returns></returns>
        public static bool GetUseCaptiaSetting()
        {
            var commentsModuleConfigType = Type.GetType("Telerik.Sitefinity.Modules.Comments.Configuration.CommentsModuleConfig, Telerik.Sitefinity");
            var configSection = Telerik.Sitefinity.Configuration.Config.Get(commentsModuleConfigType);
            var useSpamMeta = commentsModuleConfigType.GetProperty("UseSpamProtectionImage");
            bool useSpam = (bool)useSpamMeta.GetValue(configSection, null);

            return useSpam;
        }

        /// <summary>
        /// Gets the author.
        /// </summary>
        /// <param name="commentData">The comment data.</param>
        /// <returns></returns>
        public static IAuthor GetAuthor(CommentCreateRequest commentData)
        {
            var authorObject = CommentsUtilitiesReflector.Reflect("GetAuthor", commentData);
            var author = authorObject as IAuthor;
            return author;
        }

        /// <summary>
        /// Gets the ip address from current request.
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddressFromCurrentRequest()
        {
            var authorIpAddressObject = CommentsUtilitiesReflector.Reflect("GetIpAddressFromCurrentRequest");
            var authorIpAddress = authorIpAddressObject as string;

            return authorIpAddress;
        }

        /// <summary>
        /// Gets the comments by thread for current author with rating.
        /// </summary>
        /// <param name="threadKey">The thread key.</param>
        /// <returns></returns>
        public static IEnumerable<IComment> GetCommentsByThreadForCurrentAuthorWithRating(string threadKey)
        {
            object commentsObject = CommentsUtilitiesReflector.Reflect("GetCommentsByThreadForCurrentAuthorWithRating", threadKey, SystemManager.GetCommentsService());

            var comments = commentsObject as IEnumerable<IComment>;

            return comments;
        }
        
        /// <summary>
        /// Gets the comment response.
        /// </summary>
        /// <param name="comment">The comment.</param>
        /// <param name="includeSensitiveInformation">if set to <c>true</c> [include sensitive information].</param>
        /// <returns></returns>
        public static CommentResponse GetCommentResponse(IComment comment, bool includeSensitiveInformation = false)
        {
            var commentResponseObject = CommentsUtilitiesReflector.Reflect("GetCommentResponse", comment, includeSensitiveInformation);

            var commentResponse = commentResponseObject as CommentResponse;

            return commentResponse;
        }

        /// <summary>
        /// Gets the type of the thread configuration by.
        /// </summary>
        /// <param name="threadType">Type of the thread.</param>
        /// <returns></returns>
        public static CommentsSettingsElement GetThreadConfigByType(string threadType, string threadKey)
        {
            Type commentsSettingsElementType = Type.GetType("Telerik.Sitefinity.Modules.Comments.Configuration.CommentsSettingsElement, Telerik.Sitefinity");

            object commentsSettingsElementObject = CommentsUtilitiesReflector.Reflect("GetThreadConfigByType", threadType);

            var result = new CommentsSettingsElement()
            {
                AllowComments = (bool)commentsSettingsElementType.GetProperty("AllowComments").GetValue(commentsSettingsElementObject, null),
                RequiresAuthentication = (bool)commentsSettingsElementType.GetProperty("RequiresAuthentication").GetValue(commentsSettingsElementObject, null),
                RequiresApproval = (bool)commentsSettingsElementType.GetProperty("RequiresApproval").GetValue(commentsSettingsElementObject, null)
            };

            result.EnableRatings = threadKey.EndsWith("_review", StringComparison.Ordinal);

            return result;
        }

        private static object Reflect(string methodName, params object[] args)
        {
            var methodInfo = commentsUtilitiesType.Value.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            object result = methodInfo.Invoke(null, args);

            return result;
        }

        private static Lazy<Type> commentsUtilitiesType = new Lazy<Type>(() => Type.GetType("Telerik.Sitefinity.Modules.Comments.CommentsUtilities, Telerik.Sitefinity"));

        /// <summary>
        /// This class holds all needed properties for the <see cref="ReviewsWebService"/>.
        /// </summary>
        public class CommentsSettingsElement
        {
            /// <summary>
            /// Gets or sets a value indicating if content item supports comments.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if content item supports comments; otherwise, <c>false</c>.
            /// </value>
            public bool AllowComments { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether to restrict ratings to one from user by thread.
            /// </summary>
            /// <value>
            /// When set to <c>true</c> the comments are restricted to one from user by thread; otherwise, <c>false</c>.
            /// </value>
            public bool EnableRatings { get; set; }

            /// <summary>
            /// Gets or sets whether threads on the commentable type require authentication by default.
            /// </summary>
            public bool RequiresAuthentication { get; set; }

            /// <summary>
            /// Gets or sets whether threads on the commentable type require approval by default.
            /// </summary>
            public bool RequiresApproval { get; set; }
        }
    }
}
