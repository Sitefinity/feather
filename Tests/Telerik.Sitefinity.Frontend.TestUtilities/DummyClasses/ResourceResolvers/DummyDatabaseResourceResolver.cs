using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This class extends the functionality of <see cref="Telerik.Sitefinity.Frontend.Resources.Resolvers.DatabaseResourceResolver"/> . Used for test purposes.
    /// </summary>
    internal class DummyDatabaseResourceResolver : DatabaseResourceResolver
    {
        /// <summary>
        /// The control presentation result
        /// </summary>
        public readonly Dictionary<string, ControlPresentation> ControlPresentationResult = new Dictionary<string, ControlPresentation>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc />
        protected override ControlPresentation GetControlPresentation(PathDefinition virtualPathDefinition, string virtualPath, out bool isValidControlPresentationPath)
        {
            isValidControlPresentationPath = true;
            if (this.ControlPresentationResult.ContainsKey(virtualPath))
                return this.ControlPresentationResult[virtualPath];
            else
                return null;
        }

        /// <inheritdoc />
        protected override Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager GetCacheManager()
        {
            return new DummyCacheManager();
        }

        /// <inheritdoc />
        protected override Microsoft.Practices.EnterpriseLibrary.Caching.ICacheItemExpiration[] GetControlPresentationsCacheExpirations()
        {
            return null;
        }
    }
}
