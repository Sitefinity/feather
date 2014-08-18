using ArtOfTest.WebAii.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers
{
    public class BaseWrapper
    {
        /// <summary>
        /// Provides unified access to the ActiveBrowser object
        /// </summary>
        public Browser ActiveBrowser
        {
            get
            {
                return Manager.Current.ActiveBrowser;
            }
        }

        /// <summary>
        /// Provides unified access to the EM object
        /// </summary>
        public virtual FeatherElementMap EM
        {
            get
            {
                return new FeatherElementMap(ActiveBrowser.Find);
            }
        }

        /// <summary>
        /// Provides unified access to the Log object
        /// </summary>
        public Log Log
        {
            get
            {
                return Manager.Current.Log;
            }
        }
    }
}
