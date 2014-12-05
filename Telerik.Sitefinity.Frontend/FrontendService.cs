using System;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// A service that will be invoked by Sitefinity.
    /// </summary>
    [Obsolete("Initialization logic is moved to FrontendModule. This class will be deleted.")]
    public class FrontendService : ServiceBase
    {
        /// <summary>
        /// Gets the types of the service used to register and resolve the service from the Service Bus.
        /// </summary>
        /// <value>The type of the service.</value>
        public override Type[] Interfaces
        {
            get
            {
                return null;
            }
        }
    }
}
