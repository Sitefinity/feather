using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.Sitefinity.Configuration.Data;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses
{
    public class DummyConfigProvider : XmlConfigProvider
    {
        protected override void Initialize(string providerName, System.Collections.Specialized.NameValueCollection config, Type managerType)
        {
        }

        public override bool LoadSection(Configuration.ConfigSection section, Configuration.ConfigPolicyHandler policyHandler, string policyName)
        {
            return true;
        }
    }
}
