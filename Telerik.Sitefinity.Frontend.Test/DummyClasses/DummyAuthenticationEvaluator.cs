using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Frontend.Security;

namespace Telerik.Sitefinity.Frontend.Test.DummyClasses
{
    /// <summary>
    /// This class simulates the behavior of an authentication evaluator and is intended to be used for testing purposes only.
    /// </summary>
    public class DummyAuthenticationEvaluator : AuthenticationEvaluator
    {
        /// <summary>
        /// Defines whether current user is backend user.
        /// </summary>
        public bool IsBackendUser { set; get; }

        /// <inheritdoc />
        public override void RequestBackendUserAuthentication()
        {
            if (!IsBackendUser)
                throw new UnauthorizedAccessException();
        }
    }
}
