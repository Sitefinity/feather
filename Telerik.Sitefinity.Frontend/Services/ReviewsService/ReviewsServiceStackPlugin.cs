using System;
using ServiceStack;
using Telerik.Sitefinity.Frontend.Services.ReviewsService.DTO;
using Telerik.Sitefinity.Services.Comments.DTO;

namespace Telerik.Sitefinity.Frontend.Services.ReviewsService
{
    /// <summary>
    /// Represents a ServiceStack plug-in for the reviews web service.
    /// </summary>
    internal class ReviewsServiceStackPlugin : IPlugin
    {
        /// <summary>
        /// Adding the comments reviews routes
        /// </summary>
        /// <param name="appHost">The service stack appHost</param>
        public void Register(IAppHost appHost)
        {
            if (appHost == null)
                throw new ArgumentNullException("appHost");

            appHost.RegisterService<ReviewsWebService>();
            appHost.Routes.Add<AuthorReviewedGetRequest>(ReviewsServiceStackPlugin.ReviewsServiceUrl, ApplyTo.Get);
            appHost.Routes.Add<ReviewCreateRequest>(ReviewsServiceStackPlugin.ReviewsServiceUrl, ApplyTo.Post);
        }

        private const string ReviewsServiceUrl = "/reviews-api";
    }
}
