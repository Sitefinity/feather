using System;
using Telerik.Sitefinity.Frontend.Resources;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    /// <summary>
    /// This class extends the functionality of <see cref="Telerik.Sitefinity.Frontend.Resources.ServerContextHandler"/> . Used for test purposes.
    /// </summary>
    internal class DummyServerContextHandler : ServerContextHandler
    {
        /// <summary>
        /// Function to override the original GetRawScript method.
        /// </summary>
        public Func<string> GetRawScriptOverride;

        /// <summary>
        /// Function to override the original GetApplicationPath method.
        /// </summary>
        public Func<string> GetApplicationPathOverride;

        /// <summary>
        /// Gets the processed script.
        /// </summary>
        /// <returns>
        /// The processed script.
        /// </returns>
        /// <remarks>
        /// The result will be cached.
        /// </remarks>
        public string PublicGetScript()
        {
            return base.GetScript();
        }

        /// <inheritdoc />
        protected override string GetRawScript()
        {
            if (this.GetRawScriptOverride == null)
            {
                return base.GetRawScript();
            }
            else
            {
                return this.GetRawScriptOverride();
            }
        }

        /// <inheritdoc />
        protected override string GetApplicationPath()
        {
            if (this.GetApplicationPathOverride == null)
            {
                return base.GetApplicationPath();
            }
            else
            {
                return this.GetApplicationPathOverride();
            }
        }
    }
}
