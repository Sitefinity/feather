using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a resource resolver node that gets embedded resources from a specified assembly.
    /// </summary>
    internal class EmbeddedResourceResolver : ResourceResolverNode
    {
        /// <inheritdoc />
        protected override CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            // Embedded resources cannot change therefore no dependency is needed.
            return null;
        }

        /// <inheritdoc />
        protected override bool CurrentExists(PathDefinition definition, string virtualPath)
        {
            if (definition.ResourceLocation.IsNullOrEmpty())
                return false;

            Assembly assembly = this.GetAssembly(definition);
            var resourceName = this.GetResourceName(definition, virtualPath);
            var resources = assembly.GetManifestResourceNames();
            
            return resources.Contains(resourceName, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        protected override Stream CurrentOpen(PathDefinition definition, string virtualPath)
        {
            Assembly assembly = this.GetAssembly(definition);
            var resourceName = this.GetResourceName(definition, virtualPath);
            var resources = assembly.GetManifestResourceNames();
            var caseSensitiveName = resources.First(r => string.Equals(resourceName, r, StringComparison.OrdinalIgnoreCase));
            var stream = assembly.GetManifestResourceStream(caseSensitiveName);
            return stream;
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetCurrentFiles(PathDefinition definition, string path)
        {
            var resourceName = this.GetResourceName(definition, path);
            if (resourceName != null)
            {
                var regEx = new Regex(resourceName, RegexOptions.IgnoreCase);
                var assembly = this.GetAssembly(definition);
                return assembly.GetManifestResourceNames()
                    .Where(r => r.StartsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                    .Select(r => regEx.Replace(r, path));
            }

            return null;
        }

        /// <summary>
        /// Gets the resource name based on the virtual path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path.</param>
        protected virtual string GetResourceName(PathDefinition definition, string virtualPath)
        {
            string assemblyName = this.GetAssembly(definition).GetName().Name;
            string path;
            if (definition.IsWildcard)
            {
                var definitionVp = VirtualPathUtility.AppendTrailingSlash(VirtualPathUtility.ToAppRelative(definition.VirtualPath));
                var vp = VirtualPathUtility.ToAppRelative(virtualPath);

                if (!vp.StartsWith(definitionVp, StringComparison.OrdinalIgnoreCase))
                    return null;

                var dir = !vp.EndsWith("/", StringComparison.OrdinalIgnoreCase) ? VirtualPathUtility.GetDirectory(vp) : vp;
                vp = Regex.Replace(dir, @"[ \-]", "_") + Path.GetFileName(vp);
                path = assemblyName + "." + vp.Substring(definitionVp.Length).Replace('/', '.');
            }
            else
            {
                path = assemblyName;
            }

            return path;
        }
    }
}
