using System;
using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Classes that implement this interface can filter items by parent item resolved from URL
    /// </summary>
    public interface ICanFilterByParent
    {
        /// <summary>
        /// Gets the parent types.
        /// </summary>
        /// <returns>Collection of parent types to filter by.</returns>
        IEnumerable<Type> GetParentTypes();
    }
}
