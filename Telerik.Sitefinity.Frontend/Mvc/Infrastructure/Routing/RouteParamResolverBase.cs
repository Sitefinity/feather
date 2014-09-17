namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// A base route param resolver that provides responsibility chain logic.
    /// </summary>
    internal abstract class RouteParamResolverBase : IRouteParamResolver
    {
        /// <inheritdoc />
        public IRouteParamResolver Next
        {
            get 
            { 
                return this.next; 
            }
        }

        /// <inheritdoc />
        public IRouteParamResolver SetNext(IRouteParamResolver nextResolver)
        {
            this.next = nextResolver;
            return this.Next;
        }

        /// <inheritdoc />
        public bool TryResolveParam(string urlParam, out object value)
        {
            var val = this.TryResolveParamInternal(urlParam, out value);
            return val || (this.Next != null && this.Next.TryResolveParam(urlParam, out value));
        }

        /// <summary>
        /// Tries to resolve the URL parameter. Does not fallback to the next resolver.
        /// </summary>
        /// <param name="urlParam">The URL parameter.</param>
        /// <param name="value">The resolved value.</param>
        /// <returns>Whether the resolver was able to get value from the URL parameter.</returns>
        protected abstract bool TryResolveParamInternal(string urlParam, out object value);

        private IRouteParamResolver next;
    }
}
