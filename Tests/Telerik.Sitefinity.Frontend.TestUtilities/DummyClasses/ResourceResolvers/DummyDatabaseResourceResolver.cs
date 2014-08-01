using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Pages.Model;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    /// <summary>
    /// This class extends the functionality of <see cref="Telerik.Sitefinity.Frontend.Resources.Resolvers.DatabaseResourceResolver"/> . Used for test purposes.
    /// </summary>
    public class DummyDatabaseResourceResolver : DatabaseResourceResolver
    {
        /// <summary>
        /// The control presentation result
        /// </summary>
        public readonly Dictionary<string, ControlPresentation> ControlPresentationResult = new Dictionary<string, ControlPresentation>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the control presentation that contains the requested resource from the database.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        protected override ControlPresentation GetControlPresentation(PathDefinition definition, string virtualPath)
        {
            if (this.ControlPresentationResult.ContainsKey(virtualPath))
                return this.ControlPresentationResult[virtualPath];
            else
                return null;
        }
    }

}
