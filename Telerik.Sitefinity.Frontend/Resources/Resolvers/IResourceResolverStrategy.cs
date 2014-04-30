using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Resources.Resolvers
{
    /// <summary>
    /// Classes of that implement this interface should implement a strategy for handling resource resolving.
    /// </summary>
    public interface IResourceResolverStrategy : IVirtualFileResolver
    {
        /// <summary>
        /// Sets the first resolver in the chain.
        /// </summary>
        /// <param name="resolver">The first resolver in the chain.</param>
        /// <returns>The first resolver in the chain.</returns>
        IResourceResolverNode SetFirst(IResourceResolverNode resolver);

        /// <summary>
        /// Gets the first resolver.
        /// </summary>
        /// <value>
        /// The first resolver.
        /// </value>
        IResourceResolverNode First { get; }
    }
}
