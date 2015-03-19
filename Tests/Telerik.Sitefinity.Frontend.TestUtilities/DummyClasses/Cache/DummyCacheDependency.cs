using System;
using System.Web.Caching;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache
{
    /// <summary>
    /// This class inherits <see cref="System.Web.Caching.CacheDependency"/> for test purposes.
    /// </summary>
    public class DummyCacheDependency : CacheDependency
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyCacheDependency"/> class.
        /// </summary>
        public DummyCacheDependency()
        {
        }

        /// <summary>
        /// Gets or sets the key that is used to identify the current cache dependency.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Notifies the dependency of a change.
        /// </summary>
        public void Change()
        {
            this.NotifyDependencyChanged(this, new EventArgs());
        }
    }
}
