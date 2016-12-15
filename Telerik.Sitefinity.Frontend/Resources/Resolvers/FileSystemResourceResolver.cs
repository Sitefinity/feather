using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;

using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a resource resolver node that gets resources from the file system.
    /// </summary>
    internal class FileSystemResourceResolver : ResourceResolverNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemResourceResolver"/> class.
        /// </summary>
        /// <remarks>By default resource virtual paths are rooted to the web application root folder.</remarks>
        public FileSystemResourceResolver() :
            this(() => "~/")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemResourceResolver"/> class.
        /// </summary>
        /// <param name="rootPathResolver">This delegate is used to root the resource virtual paths to a custom folder.</param>
        public FileSystemResourceResolver(Func<string> rootPathResolver) :
            base()
        {
            this.rootPathResolver = rootPathResolver;
        }

        /// <inheritdoc />
        protected override CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            var fn = this.GetFileName(definition, virtualPath);
            if (string.IsNullOrWhiteSpace(fn))
            {
                return null;
            }

            var dir = Path.GetDirectoryName(fn);
            if (!string.IsNullOrWhiteSpace(dir))
            {
                // We can't monitor file changes on non-existing directories.
                if (Directory.Exists(dir))
                {
                    return new CacheDependency(fn, utcStart);
                }

                var parentDir = Path.GetDirectoryName(dir);
                while (!parentDir.IsNullOrEmpty() && !Directory.Exists(parentDir))
                {
                    dir = parentDir;
                    parentDir = Path.GetDirectoryName(dir);
                }

                if (!string.IsNullOrWhiteSpace(parentDir))
                {
                    return new CacheDependency(dir, utcStart);
                }
            }

            return null;
        }

        /// <inheritdoc />
        protected override bool CurrentExists(PathDefinition definition, string virtualPath)
        {
            var fn = this.GetFileName(definition, virtualPath);
            return fn != null && File.Exists(fn);
        }

        /// <inheritdoc />
        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            return File.OpenRead(this.GetFileName(definition, virtualPath));
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetCurrentFiles(PathDefinition definition, string path)
        {
            var mappedPath = this.GetFileName(definition, path);
            if (mappedPath != null && Directory.Exists(mappedPath))
            {
                return Directory.GetFiles(mappedPath)
                                .Select(f => f.Replace(mappedPath, path));
            }

            return null;
        }

        /// <summary>
        /// Gets the filename of the requested resource.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        protected virtual string GetFileName(PathDefinition definition, string virtualPath)
        {
            var rootPath = this.rootPathResolver();
            if (rootPath == null)
                return null;

            var vp = VirtualPathUtility.ToAppRelative(virtualPath);
            var definitionVp = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.ToAppRelative(definition.VirtualPath));

            if (!vp.StartsWith(definitionVp, StringComparison.OrdinalIgnoreCase))
                return null;

            var relativePath = vp.Substring(definitionVp.Length).Replace('/', '\\');

            var mappedPath = Path.Combine(HostingEnvironment.MapPath(rootPath), relativePath);

            return mappedPath;
        }

        private Func<string> rootPathResolver;
    }
}
