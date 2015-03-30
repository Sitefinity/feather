using System;
using System.Web;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// Instances of this class should instantiate HTTP handlers of a specified type.
    /// </summary>
    /// <typeparam name="THttpHandler">The type of the HTTP handler.</typeparam>
    internal class GenericRouteHandler<THttpHandler> : IRouteHandler
        where THttpHandler : IHttpHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRouteHandler{THttpHandler}"/> class.
        /// </summary>
        /// <param name="construct">A method used to construct an HTTP handler.</param>
        /// <exception cref="System.ArgumentNullException">construct</exception>
        public GenericRouteHandler(Func<THttpHandler> construct)
        {
            if (construct == null)
                throw new ArgumentNullException("construct");

            this.createHandler = construct;
        }

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>
        /// An object that processes the request.
        /// </returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            if (this.handlerInstance != null)
            {
                return this.handlerInstance;
            }

            var handler = this.createHandler();
            if (handler.IsReusable)
            {
                this.handlerInstance = handler;
            }

            return handler;
        }

        private readonly Func<THttpHandler> createHandler;
        private IHttpHandler handlerInstance;
    }
}
