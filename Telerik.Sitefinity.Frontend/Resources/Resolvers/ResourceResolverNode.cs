using System;
using System.Collections;
using System.IO;
using System.Web.Caching;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// This class implements a node in a resource resolver chain.
    /// </summary>
    public abstract class ResourceResolverNode : IResourceResolverNode
    {
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
        /// <returns>
        /// A <see cref="T:System.Web.Caching.CacheDependency"/> object for the specified virtual resources.
        /// </returns>
        protected abstract CacheDependency GetCurrentCacheDependency(PathDefinition definition, string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart);

        /// <summary>
        /// Opens the the file with the specified virtual path using the current resolver node.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        protected abstract Stream CurrentOpen(PathDefinition definition, string virtualPath);

        #endregion

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

            //If the requested file does not exist in the current resolver then keep these dependencies and add dependencies of the next resolver.
            //This will allow the resolvers higher in the chain to be pinged again when the file might be available to them.
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
        /// <param name="virtualPath">The virtual path of the file to open.</param>
        public virtual Stream Open(PathDefinition definition, string virtualPath)
        {
            if (this.Next != null && !this.CurrentExists(definition, virtualPath))
            {
                return this.Next.Open(definition, virtualPath);
            }
            else
            {
                return this.CurrentOpen(definition, virtualPath);
            }
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

        #endregion

        #region Private fields

        private IResourceResolverNode next;

        #endregion
    }
}
