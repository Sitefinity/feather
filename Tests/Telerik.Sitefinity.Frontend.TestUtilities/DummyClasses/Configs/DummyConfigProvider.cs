using System;
using Telerik.Sitefinity.Configuration.Data;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs
{
    /// <summary>
    /// This class represents dummy implementation of <see cref="Telerik.Sitefinity.Configuration.Data.XmlConfigProvider"/> used for test purposes only.
    /// </summary>
    public class DummyConfigProvider : XmlConfigProvider
    {
        /// <inheritdoc />
        public override bool LoadSection(Configuration.ConfigSection section, Configuration.ConfigPolicyHandler policyHandler, string policyName)
        {
            return true;
        }

        /// <inheritdoc />
        public override void SaveSection(Configuration.ConfigSection section)
        {
        }

        /// <inheritdoc />
        protected override void Initialize(string providerName, System.Collections.Specialized.NameValueCollection config, Type managerType)
        {
        }
    }
}
