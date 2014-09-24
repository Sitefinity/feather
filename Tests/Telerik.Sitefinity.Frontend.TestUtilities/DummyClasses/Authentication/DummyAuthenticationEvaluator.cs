using System;
using Telerik.Sitefinity.Frontend.Security;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Authentication
{
    /// <summary>
    /// This class simulates the behavior of an <see cref="Telerik.Sitefinity.Frontend.Security.AuthenticationEvaluator"/> and is intended to be used for testing purposes only.
    /// </summary>
    internal class DummyAuthenticationEvaluator : AuthenticationEvaluator
    {
        /// <summary>
        /// Get Defines whether current user is backend user.
        /// </summary>
        public bool IsBackendUser { get; set; }

        /// <inheritdoc />
        public override void RequestBackendUserAuthentication()
        {
            if (!this.IsBackendUser)
                throw new UnauthorizedAccessException();
        }
    }
}
