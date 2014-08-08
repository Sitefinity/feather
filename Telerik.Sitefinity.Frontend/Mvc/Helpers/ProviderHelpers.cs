using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.DynamicModules;

namespace Telerik.Sitefinity.Frontend.Mvc.Helpers
{
    /// <summary>
    /// Helper methods for the Sitefinity providers
    /// </summary>
    public static class ProviderHelpers
    {
        /// <summary>
        /// Gets the name of the default provider for the particular manager.
        /// </summary>
        /// <param name="helper">The helper.</param>
        /// <param name="manager">The manager.</param>
        /// <param name="dynamicModuleName">Name of the dynamic module.</param>
        /// <returns></returns>
        public static string DefaultProviderName(this HtmlHelper helper, IManager manager, string dynamicModuleName = "")
        {
            var defaultProviderName = string.Empty;

            if (manager is IProviderResolver)
            {
                if (manager is DynamicModuleManager)
                {
                    if (!dynamicModuleName.IsNullOrEmpty())
                        defaultProviderName = DynamicModuleManager.GetDefaultProviderName(dynamicModuleName);
                }
                else
                {
                    defaultProviderName = ((IProviderResolver)manager).GetDefaultContextProvider().Name;
                }
            }
            else if (manager != null)
            {
                defaultProviderName = manager.Provider.Name;
            }

            return defaultProviderName;
        }
    }
}
