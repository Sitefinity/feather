namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Classes that implement this interface should provide responsibility chain logic for resolving a specific route parameter.
    /// </summary>
    internal interface IRouteParamResolver
    {
        /// <summary>
        /// Gets the next parameter resolver.
        /// </summary>
        /// <value>
        /// The next parameter resolver.
        /// </value>
        IRouteParamResolver Next { get; }

        /// <summary>
        /// Sets the next parameter resolver.
        /// </summary>
        /// <param name="nextResolver">The next resolver.</param>
        /// <returns>The resolver that was just set.</returns>
        IRouteParamResolver SetNext(IRouteParamResolver nextResolver);

        /// <summary>
        /// Tries to resolve the URL parameter.
        /// </summary>
        /// <param name="urlParam">The URL parameter.</param>
        /// <param name="value">The resolved value.</param>
        /// <returns>Whether the resolver was able to get value from the URL parameter.</returns>
        bool TryResolveParam(string urlParam, out object value);
    }
}