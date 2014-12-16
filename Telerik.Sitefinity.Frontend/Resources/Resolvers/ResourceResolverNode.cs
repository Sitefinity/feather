using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a node in a resource resolver chain.
    /// </summary>
    internal abstract class ResourceResolverNode : IResourceResolverNode
    {
        #region IResourceResolverNode

        #region IVirtualFileResolver

        /// <summary>
        /// Determines whether a file with the specified virtual path exists.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path to check.</param>
        public virtual bool Exists(PathDefinition definition, string virtualPath)
        {
            return this.CurrentExists(definition, virtualPath) ||
                (this.Next != null && this.Next.Exists(definition, virtualPath));
        }

        /// <summary>
        /// Creates a cache dependency based on the specified virtual paths.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The path to the primary virtual resource.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        public virtual CacheDependency GetCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            CacheDependency result = this.GetCurrentCacheDependency(definition, virtualPath, virtualPathDependencies, utcStart);

            // If the requested file does not exist in the current resolver then keep these dependencies and add dependencies of the next resolver.
            // This will allow the resolvers higher in the chain to be pinged again when the file might be available to them.
            if (this.Next != null && !this.CurrentExists(definition, virtualPath))
            {
                var nextDependencies = this.Next.GetCacheDependency(definition, virtualPath, virtualPathDependencies, utcStart);
                if (nextDependencies != null)
                {
                    if (result != null)
                    {
                        var aggr = new AggregateCacheDependency();
                        aggr.Add(result, nextDependencies);
                        result = aggr;
                    }
                    else
                    {
                        result = nextDependencies;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Opens the the file with the specified virtual path.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        /// <returns></returns>
        public virtual Stream Open(PathDefinition definition, string virtualPath)
        {
            if (this.Next != null && !this.CurrentExists(definition, virtualPath))
                return this.Next.Open(definition, virtualPath);

            return this.CurrentOpen(definition, virtualPath);
        }

        #endregion

        /// <summary>
        /// Sets the next resolver in the chain.
        /// </summary>
        /// <param name="resolver">The next resolver.</param>
        /// <returns>
        /// The next resolver.
        /// </returns>
        public virtual IResourceResolverNode SetNext(IResourceResolverNode resolver)
        {
            this.next = resolver;
            return this.next;
        }

        /// <summary>
        /// Gets the next resolver in the chain.
        /// </summary>
        /// <value>
        /// The next resolver in the chain.
        /// </value>
        public virtual IResourceResolverNode Next
        {
            get 
            {
                return this.next;
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<string> GetFiles(PathDefinition definition, string virtualPath)
        {
            var result = this.GetCurrentFiles(definition, virtualPath);
            if (this.Next != null)
            {
                var nextFiles = this.Next.GetFiles(definition, virtualPath);
                if (nextFiles != null)
                    result = result != null ? result.Union(nextFiles) : nextFiles;
            }

            return result;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Determines whether a file with the specified virtual path exists in the current resolver node.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path to check.</param>
        protected abstract bool CurrentExists(PathDefinition definition, string virtualPath);

        /// <summary>
        /// Creates a cache dependency based on the specified virtual path and on the current resolver node.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The path to the virtual resource.</param>
        /// <param name="virtualPathDependencies">The virtual path dependencies.</param>
        /// <param name="utcStart">The UTC start.</param>
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency" /> object for the specified virtual resources.
        /// </returns>
        protected abstract CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart);

        /// <summary>
        /// Opens the the file with the specified virtual path using the current resolver node.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        /// <returns></returns>
        protected abstract Stream CurrentOpen(PathDefinition definition, string virtualPath);

        /// <summary>
        /// Gets the available files in the given path using the current resolver node.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="path">The path.</param>
        protected abstract IEnumerable<string> GetCurrentFiles(PathDefinition definition, string path);

        #endregion

        #region Protected methods

        /// <summary>
        /// Gets the assembly which is specified in the PathDefinition.
        /// </summary>
        /// <param name="definition">The path definition.</param>
        /// <exception cref="System.InvalidOperationException">Invalid PathDefinition.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        protected virtual Assembly GetAssembly(PathDefinition definition)
        {
            object assembly;
            if (!definition.Items.TryGetValue("Assembly", out assembly))
            {
                lock (this)
                {
                    if (!definition.Items.TryGetValue("Assembly", out assembly))
                    {
                        assembly = Assembly.LoadFrom(definition.ResourceLocation);
                        definition.Items.Add("Assembly", assembly);
                    }
                }
            }

            return (Assembly)assembly;
        }

        #endregion

        #region Private fields

        private IResourceResolverNode next;

        #endregion
    }
}
