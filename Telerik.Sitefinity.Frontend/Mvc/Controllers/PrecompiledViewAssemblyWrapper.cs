using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RazorGenerator.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// This class wraps a PrecompiledViewAssembly to 
    /// </summary>
    internal class PrecompiledViewAssemblyWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrecompiledViewAssemblyWrapper"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="package">The package.</param>
        public PrecompiledViewAssemblyWrapper(Assembly assembly, string package)
        {
            this.assembly = assembly;
            this.basePath = package == null ? "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(assembly) : "~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(typeof(PrecompiledViewAssemblyWrapper));
            this.precompiledViewAssembly = new PrecompiledViewAssembly(assembly, this.basePath)
            {
                UsePhysicalViewsIfNewer = false
            };

            this.embeddedResourceHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var name in this.assembly.GetManifestResourceNames())
            {
                this.embeddedResourceHashes[name] = this.ComputeHashForResource(name);
            }
        }

        /// <summary>
        /// Gets the precompiled view assembly.
        /// </summary>
        /// <value>
        /// The precompiled view assembly.
        /// </value>
        public PrecompiledViewAssembly PrecompiledViewAssembly
        { 
            get
            {
                return this.precompiledViewAssembly;
            }
        }

        /// <summary>
        /// Gets the hash of the resource on the virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the resource.</param>
        /// <returns>Hash of the resource.</returns>
        public string GetFileHash(string virtualPath)
        {
            if (virtualPath.StartsWith(this.basePath, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = virtualPath.Right(virtualPath.Length - this.basePath.Length);
                var relativePathWithoutSpaces = Regex.Replace(relativePath, @"[ \-]", "_").Replace('/', '.');
                var resourcePathWithoutSpaces = this.assembly.GetName().Name + "." + relativePathWithoutSpaces;
                var relativePathWithSpaces = Regex.Replace(relativePath, @"[\-]", "_").Replace('/', '.');
                var resourcePathWithSpaces = this.assembly.GetName().Name + "." + relativePathWithSpaces;

                if (this.embeddedResourceHashes.ContainsKey(resourcePathWithoutSpaces))
                {
                    return this.embeddedResourceHashes[resourcePathWithoutSpaces];
                }
                else if (this.embeddedResourceHashes.ContainsKey(resourcePathWithSpaces))
                {
                    return this.embeddedResourceHashes[resourcePathWithSpaces];
                }
            }

            return null;
        }

        public IEnumerable<string> GetViews(string virtualFolderPath)
        {
            if (virtualFolderPath.StartsWith(this.basePath, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = virtualFolderPath.Right(virtualFolderPath.Length - this.basePath.Length);
                var relativePathWithSpaces = Regex.Replace(relativePath, @"[\-]", "_").Replace('/', '.');
                var resourcePathWithSpaces = this.assembly.GetName().Name + "." + relativePathWithSpaces;

                var resources = this.embeddedResourceHashes.Keys
                    .Where(resourceKey => resourceKey.ToUpperInvariant().Contains(resourcePathWithSpaces.ToUpperInvariant()))
                    .Select(key => Path.GetFileNameWithoutExtension(Regex.Replace(key, resourcePathWithSpaces, string.Empty, RegexOptions.IgnoreCase)));

                return resources;
            }

            return new List<string>();
        }

        private string ComputeHashForResource(string resourceName)
        {
            using (var stream = this.assembly.GetManifestResourceStream(resourceName))
            {
                return CompositePrecompiledMvcEngineWrapper.ComputeHash(stream);
            }
        }

        private readonly PrecompiledViewAssembly precompiledViewAssembly;
        private readonly Assembly assembly;
        private readonly string basePath;
        private readonly Dictionary<string, string> embeddedResourceHashes;
    }
}
