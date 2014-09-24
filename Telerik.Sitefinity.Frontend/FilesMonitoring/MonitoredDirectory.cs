using System;
using System.Collections.Generic;

namespace Telerik.Sitefinity.Frontend.FilesMonitoring
{
    /// <summary>
    /// This class represents DTO for working with monitored directories.
    /// </summary>
    internal class MonitoredDirectory : IEqualityComparer<MonitoredDirectory>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoredDirectory"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="isPackage">if set to <c>true</c> [is package].</param>
        public MonitoredDirectory(string path, bool isPackage)
        {
            this.Path = path;
            this.IsPackage = isPackage;
        }

        /// <summary>
        /// Gets or sets the directory path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the directory wraps the packages.
        /// </summary>
        /// <value>
        ///   <c>true</c> whether it is package directory; otherwise, <c>false</c>.
        /// </value>
        public bool IsPackage { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" }, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this.Equals(this, obj as MonitoredDirectory);
        }

        #region IEqualityComparer Members

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// true if the specified objects are equal; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Equals(MonitoredDirectory x, MonitoredDirectory y)
        {
            return x.Path == y.Path;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetHashCode(MonitoredDirectory obj)
        {
            /// TODO: Implement this
            throw new NotImplementedException();
        }

        #endregion 
    }
}
