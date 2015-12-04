using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.DesignerToolbox;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Determines which controls should be visible in the toolbox of the <see cref="ZoneEditor"/> when Feather is deactivated.
    /// </summary>
    internal class FeatherEnabledToolboxFilter : IToolboxFilter
    {
        /// <inheritdoc />
        public virtual bool IsSectionVisible(IToolboxSection section)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual bool IsToolVisible(IToolboxItem tool)
        {
            var isFeatherDeactivated = SystemManager.GetModule("Feather") == null;
            if (isFeatherDeactivated)
            {
                var isFeatherWidget = tool.ControllerType.StartsWith("Telerik.Sitefinity.Frontend", StringComparison.Ordinal);
                if (isFeatherWidget)
                    return false;
            }
            
            return true;
        }
    }
}
