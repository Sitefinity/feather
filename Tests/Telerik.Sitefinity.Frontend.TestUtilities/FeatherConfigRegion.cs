using System;
using System.Diagnostics.CodeAnalysis;
using Telerik.Sitefinity.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUtilities
{
    /// <summary>
    /// Temporary set configuration for the Feather config. Cofniguration is restored to previous state after the region is disposed.
    /// </summary>
    public sealed class FeatherConfigRegion : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatherConfigRegion"/> class.
        /// </summary>
        /// <param name="disablePrecompilation">Value for the DisablePrecompilation option.</param>
        /// <param name="alwaysUsePrecompiled">Value for the AlwaysUsePrecompiledVersion option.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Precompilation")]
        public FeatherConfigRegion(bool disablePrecompilation, bool alwaysUsePrecompiled)
        {
            this.previousDisablePrecompilation = Config.Get<FeatherConfig>().DisablePrecompilation;
            this.previousAlwaysUsePrecompiled = Config.Get<FeatherConfig>().AlwaysUsePrecompiledVersion;

            this.SetConfigOptions(disablePrecompilation, alwaysUsePrecompiled);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.SetConfigOptions(this.previousDisablePrecompilation, this.previousAlwaysUsePrecompiled);
        }

        private void SetConfigOptions(bool disablePrecompilation, bool alwaysUsePrecompiled)
        {
            var manager = ConfigManager.GetManager();
            var config = manager.GetSection<FeatherConfig>();
            config.DisablePrecompilation = disablePrecompilation;
            config.AlwaysUsePrecompiledVersion = alwaysUsePrecompiled;

            manager.SaveSection(config);
        }

        private readonly bool previousDisablePrecompilation;
        private readonly bool previousAlwaysUsePrecompiled;
    }
}
