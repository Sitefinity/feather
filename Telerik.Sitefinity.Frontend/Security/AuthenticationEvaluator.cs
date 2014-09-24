using Telerik.Sitefinity.Web.Services;

namespace Telerik.Sitefinity.Frontend.Security
{
    /// <summary>
    /// This class provides logic for evaluating whether a user is authenticated.
    /// </summary>
    internal class AuthenticationEvaluator
    {
        /// <summary>
        /// Makes sure that the current user is logged on and has rights to access the Sitefinity backend. If not, an exception will be thrown.
        /// </summary>
        /// <exception cref="WebProtocolException">
        /// Thrown when the user is not authenticated or does not have permissions to access the Sitefinity backend.
        /// </exception>
        public virtual void RequestBackendUserAuthentication()
        {
            ServiceUtility.RequestBackendUserAuthentication();
        }
    }
}
