using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Security.CSRF;

namespace Telerik.Sitefinity.Frontend.Security
{
    /// <summary>
    /// Helper methods for validating CSRF tokens in controllers.
    /// </summary>
    public class AntiCsrfHelpers
    {
        /// <summary>
        /// Validates the token in the form submitted with the given request.
        /// </summary>
        /// <param name="formCollection">The request with the form values.</param>
        /// <returns>True if valid token, else false.</returns>
        public static bool IsValidCsrfToken(NameValueCollection formCollection)
        {
            var antiCsrf = ObjectFactory.IsTypeRegistered<IAntiCsrf>() ? ObjectFactory.Resolve<IAntiCsrf>() : null;
            if (antiCsrf != null)
            {
                var xsrfToken = formCollection[antiCsrf.HiddenFieldName];
                return antiCsrf.IsTokenValidForCurrentSession(xsrfToken);
            }

            return true;
        }
    }
}
