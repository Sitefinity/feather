using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// A base URL params mapper that provides responsibility chain logic.
    /// </summary>
    public abstract class UrlParamsMapperBase : IUrlParamsMapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UrlParamsMapperBase"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public UrlParamsMapperBase(Controller controller)
        {
            this.Controller = controller;
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>
        /// The controller.
        /// </value>
        protected Controller Controller { get; private set; }

        /// <inheritdoc />
        public IUrlParamsMapper Next
        {
            get { return this.next; }
        }

        /// <inheritdoc />
        public IUrlParamsMapper SetNext(IUrlParamsMapper nextResolver)
        {
            this.next = nextResolver;
            return this.Next;
        }

        /// <inheritdoc />
        public void ResolveUrlParams(string[] urlParams, RequestContext requestContext)
        {
            this.ResolveUrlParamsInternal(urlParams, requestContext);

            if (!RouteHelper.GetUrlParametersResolved() && this.Next != null)
                this.Next.ResolveUrlParams(urlParams, requestContext);
        }

        /// <summary>
        /// Resolves the URL parameters. Should call RouteHelper.SetUrlParametersResolved if the mapping was successful. Does not fallback to next mappers.
        /// </summary>
        /// <param name="urlParams">The URL parameters.</param>
        /// <param name="requestContext">The request context.</param>
        protected abstract void ResolveUrlParamsInternal(string[] urlParams, RequestContext requestContext);

        /// <summary>
        /// The action name key for the RouteData values.
        /// </summary>
        protected const string ActionNameKey = "action";

        private IUrlParamsMapper next;
    }
}
