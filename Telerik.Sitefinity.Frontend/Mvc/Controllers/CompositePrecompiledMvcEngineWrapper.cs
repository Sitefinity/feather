using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;
using RazorGenerator.Mvc;
using Telerik.Sitefinity.Configuration;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// View engine that serves precompiled views from several assemblies.
    /// </summary>
    public class CompositePrecompiledMvcEngineWrapper : CompositePrecompiledMvcEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        public CompositePrecompiledMvcEngineWrapper(params PrecompiledViewAssemblyWrapper[] viewAssemblies)
            : this(viewAssemblies, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        public CompositePrecompiledMvcEngineWrapper(IEnumerable<PrecompiledViewAssemblyWrapper> viewAssemblies, IViewPageActivator viewPageActivator)
            : this(viewAssemblies, viewPageActivator, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        /// <param name="packageName">Name of the package.</param>
        public CompositePrecompiledMvcEngineWrapper(IEnumerable<PrecompiledViewAssemblyWrapper> viewAssemblies, IViewPageActivator viewPageActivator, string packageName)
            : base(viewAssemblies.Select(a => a.PrecompiledViewAssembly), viewPageActivator)
        {
            this.precompiledAssemblies = viewAssemblies.ToArray();
            this.packageName = packageName;
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        public string PackageName
        {
            get
            {
                return this.packageName;
            }
        }

        /// <summary>
        /// Creates a new instance that is a clone of this one.
        /// </summary>
        /// <returns>The new instance.</returns>
        public CompositePrecompiledMvcEngineWrapper Clone()
        {
            return new CompositePrecompiledMvcEngineWrapper(this.precompiledAssemblies, null, this.PackageName);
        }

        /// <summary>
        /// Computes the hash of the data in the given stream in the expected format for the precompiled assemblies to compare resources.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Base64 string of the MD5 hash of the data in the stream.</returns>
        internal static string ComputeHash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(stream));
            }
        }

        /// <summary>
        /// Files the exists.
        /// </summary>
        /// <param name="controllerContext">The controller context.</param>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
        {
            return !Config.Get<FeatherConfig>().DisablePrecompilation && base.FileExists(controllerContext, virtualPath) && (Config.Get<FeatherConfig>().AlwaysUsePrecompiledVersion || this.ShouldServe(virtualPath));
        }

        private bool ShouldServe(string virtualPath)
        {
            string precompiledFileHash = null;
            foreach (var asm in this.precompiledAssemblies)
            {
                precompiledFileHash = asm.GetFileHash(virtualPath);
                if (precompiledFileHash != null)
                    break;
            }

            if (precompiledFileHash == null)
                return true;

            string virtualResourceHash = null;
            if (precompiledFileHash != null)
            {
                virtualResourceHash = this.VirtualResourceHash(virtualPath);
            }

            return virtualResourceHash == null || virtualResourceHash == precompiledFileHash;
        }

        private string VirtualResourceHash(string virtualPath)
        {
            CompositePrecompiledMvcEngineWrapper.HashCacheRecord hashRecord;
            if (CompositePrecompiledMvcEngineWrapper.HashCache.ContainsKey(virtualPath))
            {
                hashRecord = CompositePrecompiledMvcEngineWrapper.HashCache[virtualPath];
                if (hashRecord.Dependency == null || !hashRecord.Dependency.HasChanged)
                    return hashRecord.Hash;
            }

            lock (CompositePrecompiledMvcEngineWrapper.HashCache)
            {
                if (HostingEnvironment.VirtualPathProvider.FileExists(virtualPath))
                {
                    using (var stream = HostingEnvironment.VirtualPathProvider.GetFile(virtualPath).Open())
                    {
                        hashRecord = new CompositePrecompiledMvcEngineWrapper.HashCacheRecord()
                        {
                            Hash = CompositePrecompiledMvcEngineWrapper.ComputeHash(stream),
                            Dependency = HostingEnvironment.VirtualPathProvider.GetCacheDependency(virtualPath, new object[0], DateTime.UtcNow)
                        };
                    }
                }
                else
                {
                    hashRecord = new CompositePrecompiledMvcEngineWrapper.HashCacheRecord()
                    {
                        Hash = null,
                        Dependency = null
                    };
                }

                CompositePrecompiledMvcEngineWrapper.HashCache[virtualPath] = hashRecord;
            }

            return hashRecord.Hash;
        }

        private class HashCacheRecord
        {
            public string Hash { get; set; }

            public CacheDependency Dependency { get; set; }
        }

        private static readonly ConcurrentDictionary<string, CompositePrecompiledMvcEngineWrapper.HashCacheRecord> HashCache = new ConcurrentDictionary<string, CompositePrecompiledMvcEngineWrapper.HashCacheRecord>(StringComparer.OrdinalIgnoreCase);

        private readonly PrecompiledViewAssemblyWrapper[] precompiledAssemblies;
        private readonly string packageName;
    }
}
