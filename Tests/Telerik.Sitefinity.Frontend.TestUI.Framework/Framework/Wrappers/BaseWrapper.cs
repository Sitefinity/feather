using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtOfTest.WebAii.Core;
using Telerik.Sitefinity.Frontend.TestUI.Framework.ElementMap;

namespace Telerik.Sitefinity.Frontend.TestUI.Framework.Wrappers
{
    /// <summary>
    /// Base wrapper class.
    /// </summary>
    public class BaseWrapper
    {
        /// <summary>
        /// Gets the ActiveBrowser object.
        /// </summary>
        /// <value>The active browser.</value>
        public Browser ActiveBrowser
        {
            get
            {
                return Manager.Current.ActiveBrowser;
            }
        }

        /// <summary>
        /// Gets the feather element map and provides unified access to the EM object.
        /// </summary>
        /// <value>Feather element map.</value>
        public virtual FeatherElementMap EM
        {
            get
            {
                return new FeatherElementMap(this.ActiveBrowser.Find);
            }
        }

        /// <summary>
        /// Gets manager log object and provides unified access.
        /// </summary>
        /// <value>Current manager log.</value>
        public Log Log
        {
            get
            {
                return Manager.Current.Log;
            }
        }
    }
}
