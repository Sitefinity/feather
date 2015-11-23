using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Localization;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// Configuration for the Feather module
    /// </summary>
    [ObjectInfo(typeof(InfrastructureResources), Title = "FeatherConfigCaption", Description = "FeatherConfigDescription")]
    public class FeatherConfig : ConfigSection
    {
        /// <summary>
        /// Gets or sets a value indicating whether to disable precompilation.
        /// </summary>
        /// <value>
        /// <c>true</c> if precompilation is disabled; otherwise, <c>false</c>.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precompilation")]
        [ConfigurationProperty("disablePrecompilation")]
        [ObjectInfo(typeof(InfrastructureResources), Title = "DisablePrecompilationCaption", Description = "DisablePrecompilationDescription")]
        public bool DisablePrecompilation
        {
            get
            {
                return (bool)this["disablePrecompilation"];
            }

            set
            {
                this["disablePrecompilation"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to always use the precompiled version of a resource regardless of changes to physical files.
        /// </summary>
        /// <value>
        /// <c>true</c> if precompiled version should be used regardless of changes to physical files; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("alwaysUsePrecompiledVersion")]
        [ObjectInfo(typeof(InfrastructureResources), Title = "AlwaysUsePrecompiledVersionCaption", Description = "AlwaysUsePrecompiledVersionDescription")]
        public bool AlwaysUsePrecompiledVersion
        {
            get
            {
                return (bool)this["alwaysUsePrecompiledVersion"];
            }

            set
            {
                this["alwaysUsePrecompiledVersion"] = value;
            }
        }
    }
}
