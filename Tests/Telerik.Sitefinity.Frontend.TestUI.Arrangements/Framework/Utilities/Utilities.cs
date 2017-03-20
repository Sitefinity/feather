using Telerik.Sitefinity.TestArrangementService.Attributes;
using Telerik.Sitefinity.TestArrangementService.Core;
using Telerik.Sitefinity.TestUI.Arrangements.Framework.Server;
using Telerik.Sitefinity.TestUtilities.CommonOperations;

namespace Telerik.Sitefinity.Frontend.TestUI.Arrangements
{
    /// <summary>
    /// SystemContext Arrangement
    /// </summary>
    public class SystemContext
    {
        /// <summary>
        /// Determines whether the system context is in multisite mode.
        /// </summary>
        [ServerArrangement]
        public void IsMultisiteMode()
        {
            var isMultisiteMode = ServerOperations.MultiSite().CheckIsMultisiteMode();

            ServerArrangementContext.GetCurrent().Values.Add("isMultisiteMode", isMultisiteMode.ToString());
        }

        /// <summary>
        /// Determines whether the system context is in multilingual mode.
        /// </summary>
        [ServerArrangement]
        public void IsMultilingualMode()
        {
            var isMultilingualMode = ServerOperations.Multilingual().IsCurrentSiteInMultilingual;

            ServerArrangementContext.GetCurrent().Values.Add("isMultilingualMode", isMultilingualMode.ToString());
        }

        /// <summary>
        /// Gets the default arrangement culture.
        /// </summary>
        [ServerArrangement]
        public void GetDefaultArrangementCulture()
        {
            var culture = ArrangementConfig.GetArrangementCulture();
            ServerArrangementContext.GetCurrent().Values.Add("defaultArrangementCulture", culture);
        }

        /// <summary>
        /// Gets the default arrangement site.
        /// </summary>
        [ServerArrangement]
        public void GetDefaultArrangementSite()
        {
            var site = ArrangementConfig.GetArrangementSite();
            ServerArrangementContext.GetCurrent().Values.Add("defaultArrangementSite", site);
        }
    }
}
