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

        /// <summary>
        /// Gets or sets a value indicating whether to log when precompiled views are used.
        /// </summary>
        /// <value>
        /// <c>true</c> if usage of precompiled views should be logged; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("logPrecompiledViewUsage")]
        [ObjectInfo(typeof(InfrastructureResources), Title = "LogPrecompiledViewUsageCaption", Description = "LogPrecompiledViewUsageDescription")]
        public bool LogPrecompiledViewUsage
        {
            get
            {
                return (bool)this["logPrecompiledViewUsage"];
            }

            set
            {
                this["logPrecompiledViewUsage"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to require parameter naming in the widget routings.
        /// </summary>
        /// <value>
        /// <c>true</c> if the routes will work only with named params (e.g /tag/london/page/2); otherwise, <c>false</c> when the route will be /london/2.
        /// </value>
        [ConfigurationProperty("useNamedParametersRouting")]
        [ObjectInfo(typeof(InfrastructureResources), Title = "UseNamedParametersRoutingCaption", Description = "UseNamedParametersRoutingDescription")]
        public bool UseNamedParametersRouting
        {
            get
            {
                return (bool)this["useNamedParametersRouting"];
            }

            set
            {
                this["useNamedParametersRouting"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether cached controller assemblies to be used
        /// </summary>
        /// <value>
        /// <c>true</c> if the cached controller assemblies are used; otherwise, <c>false</c>
        /// </value>
        [ConfigurationProperty("useCachedControllerContainerAssemblies", DefaultValue = true)]
        [ObjectInfo(typeof(InfrastructureResources), Title = "UseCachedControllerContainerAssembliesTitle", Description = "UseCachedControllerContainerAssembliesDescription")]
        public bool UseCachedControllerContainerAssemblies
        {
            get
            {
                return (bool)this["useCachedControllerContainerAssemblies"];
            }

            set
            {
                this["useCachedControllerContainerAssemblies"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Ninject kernel should automatically load extensions at startup
        /// </summary>
        /// <value>
        /// <c>true</c> if the Ninject kernel should automatically load extensions at startup; otherwise, <c>false</c>
        /// </value>
        [ConfigurationProperty("ninjectLoadExtensions", DefaultValue = true)]
        [ObjectInfo(typeof(InfrastructureResources), Title = "NinjectLoadExtensionsTitle", Description = "NinjectLoadExtensionsDescription")]
        public bool NinjectLoadExtensions
        {
            get
            {
                return (bool)this["ninjectLoadExtensions"];
            }

            set
            {
                this["ninjectLoadExtensions"] = value;
            }
        }
    }
}
