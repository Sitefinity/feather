using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Modules.GenericContent;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve specific content item by the URL.
    /// </summary>
    /// <typeparam name="TContent">The type of the content that should be resolved.</typeparam>
    public class DetailActionParamsMapper<TContent> : UrlParamsMapperBase
        where TContent : class, IDataItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public DetailActionParamsMapper(Controller controller)
            : this(controller, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DetailActionParamsMapper{TContent}"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public DetailActionParamsMapper(Controller controller, Func<string> providerNameResolver, string actionName = "Details")
            : base(controller)
        {
            this.actionName = actionName;
            this.providerNameResolver = providerNameResolver;

            this.ActionMethod = this.Controller.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
            if (this.ActionMethod == null)
                throw new ArgumentException("The controller {0} does not have action '{1}'.".Arrange(controller.GetType().Name, actionName));

            try
            {
                this.Manager = this.ResolveManager();
            }
            catch (Exception) { }
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext)
        {
            if (urlParams == null)
                return false;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var item = this.GetItemByUrl(url, out redirectUrl);

            if (item != null)
            {
                this.AddContentItemToRouteData(requestContext, redirectUrl, item);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the content item to route data.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <param name="item">The item.</param>
        protected void AddContentItemToRouteData(RequestContext requestContext, string redirectUrl, TContent item)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

            var parameters = this.ActionMethod.GetParameters();
            if (parameters.Length > 0 && typeof(TContent).IsAssignableFrom(parameters[0].ParameterType))
            {
                requestContext.RouteData.Values[parameters[0].Name] = item;
            }

            if (redirectUrl.IsNullOrEmpty() == false && parameters.Length > 1 && parameters[1].ParameterType == typeof(string))
            {
                requestContext.RouteData.Values[parameters[1].Name] = redirectUrl;
            }

            RouteHelper.SetUrlParametersResolved();
        }

        /// <summary>
        /// Gets content item by URL.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="redirectUrl">The redirect URL.</param>
        /// <returns></returns>
        protected TContent GetItemByUrl(string url, out string redirectUrl)
        {
            if (this.Manager == null)
            {
                redirectUrl = null;
                return default(TContent);
            }

            if (this.Manager is IContentManager)
            {
                return ((IContentManager)this.Manager).GetItemFromUrl(typeof(TContent), url, out redirectUrl) as TContent;
            }
            else
            {
                return ((IUrlProvider)this.Manager.Provider).GetItemFromUrl(typeof(TContent), url, out redirectUrl) as TContent;
            }
        }

        /// <summary>
        /// Resolves the manager.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Items of the {0} type cannot be found by URL..Arrange(typeof(TContent).FullName)</exception>
        protected IManager ResolveManager()
        {
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var manager = ManagerBase.GetMappedManager(typeof(TContent), providerName);

            if (!(manager is IContentManager) && !(manager.Provider is IUrlProvider))
                throw new InvalidOperationException("Items of the {0} type cannot be found by URL.".Arrange(typeof(TContent).FullName));

            return manager;
        }

        protected IManager Manager { get; private set; }
        protected MethodInfo ActionMethod { get; private set; }
        private string actionName;
        private Func<string> providerNameResolver;
    }
}
