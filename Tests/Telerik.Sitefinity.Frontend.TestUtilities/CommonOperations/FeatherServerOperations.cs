using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telerik.Sitefinity.Frontend.TestUtilities.CommonOperations
{
    /// <summary>
    /// Provides common server operations
    /// </summary>
    public static class FeatherServerOperations
    {
        /// <summary>
        /// Entry point for Resource packages operations.
        /// </summary>
        /// <returnsResource>ResourcePackagesOperations instance.</returns>
        public static ResourcePackagesOperations ResourcePackages()
        {
            return new ResourcePackagesOperations();
        }

        /// <summary>
        /// Entry point for Pages operations.
        /// </summary>
        /// <returnsResource>PagesOperations instance.</returns>
        public static PagesOperations Pages()
        {
            return new PagesOperations();
        }
    }
}
