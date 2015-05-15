using System;
using System.Linq;
using System.Reflection;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Services.Comments.DTO;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService
{
    /// <summary>
    /// This class holds all methods privatly exposed by the CommentWebService class in sf.
    /// </summary>
    internal static class CommentsWebServiceReflector
    {
        /// <summary>
        /// Validates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public static void Validate(CommentCreateRequest request)
        {
            CommentsWebServiceReflector.ValidateReflect(request);
        }

        /// <summary>
        /// Validates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public static void Validate(ThreadCreateRequest request)
        {
            CommentsWebServiceReflector.ValidateReflect(request);
        }

        /// <summary>
        /// Validates the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        public static void Validate(GroupCreateRequest request)
        {
            CommentsWebServiceReflector.ValidateReflect(request);
        }

        /// <summary>
        /// Validates the post request.
        /// </summary>
        /// <param name="threadType">Type of the thread.</param>
        /// <param name="captchaInfo">The captcha information.</param>
        /// <param name="skipCaptia">if set to <c>true</c> [skip captia].</param>
        public static void ValidatePostRequest(string threadType, CaptchaInfo captchaInfo, bool skipCaptia)
        {
            CommentsWebServiceReflector.ValidatePostRequestReflect(threadType, captchaInfo, skipCaptia);
        }

        private static void ValidateReflect(params object[] args)
        {
            var argsTypes = args.Select(a => a.GetType()).ToArray();

            var methodInfo = commentsWebServiceType.Value.GetMethod("Validate", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Any, argsTypes, null);
            methodInfo.Invoke(commentsWebServiceInstance.Value, args);
        }

        private static void ValidatePostRequestReflect(params object[] args)
        {
            var methodInfo = commentsWebServiceType.Value.GetMethod("ValidatePostRequest", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(commentsWebServiceInstance.Value, args);
        }

        private static Lazy<object> commentsWebServiceInstance = new Lazy<object>(() => commentsWebServiceType.Value.GetConstructor(Type.EmptyTypes).Invoke(new object[] { }));
        private static Lazy<Type> commentsWebServiceType = new Lazy<Type>(() => Type.GetType("Telerik.Sitefinity.Services.Comments.CommentWebService, Telerik.Sitefinity"));
    }
}
