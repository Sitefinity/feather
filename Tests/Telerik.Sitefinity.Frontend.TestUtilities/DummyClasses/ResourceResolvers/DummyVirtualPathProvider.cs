using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Caching;
using System.Web.Hosting;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Cache;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers
{
    public class DummyVirtualPathProvider : VirtualPathProvider
    {
        /// <summary>
        /// Gets a value that indicates whether a file exists in the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>
        /// true if the file exists in the virtual file system; otherwise, false.
        /// </returns>
        public override bool FileExists(string virtualPath)
        {
            return this.Content.ContainsKey(virtualPath);
        }

        /// <summary>
        /// Gets a virtual file from the virtual file system.
        /// </summary>
        /// <param name="virtualPath">The path to the virtual file.</param>
        /// <returns>
        /// A descendent of the <see cref="T:System.Web.Hosting.VirtualFile" /> class that represents a file in the virtual file system.
        /// </returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            return new DummyVirtualFile(virtualPath, this.Content[virtualPath]);
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <param name="virtualPathDependencies">An array of paths to other resources required by the primary virtual resource.</param>
        /// <param name="utcStart">The UTC time at which the virtual resources were read.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency" /> object for the specified virtual resources.
        /// </returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var dependency = new DummyCacheDependency();
            this.Dependencies[virtualPath] = dependency;

            return dependency;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public readonly IDictionary<string, DummyCacheDependency> Dependencies = new Dictionary<string, DummyCacheDependency>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        public readonly IDictionary<string, string> Content = new Dictionary<string, string>();

        private class DummyVirtualFile : VirtualFile
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DummyVirtualFile" /> class.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <param name="content">The content.</param>
            public DummyVirtualFile(string path, string content)
                : base(path)
            {
                this.content = content;
            }

            /// <summary>
            /// When overridden in a derived class, returns a read-only stream to the virtual resource.
            /// </summary>
            /// <returns>
            /// A read-only stream to the virtual file.
            /// </returns>
            public override Stream Open()
            {
                return new MemoryStream(Encoding.UTF8.GetBytes(this.content));
            }

            private readonly string content;
        }
    }
}
