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

            this.actionMethod = this.Controller.GetType().GetMethod(actionName, BindingFlags.Public | BindingFlags.Instance);
            if (this.actionMethod == null)
                throw new ArgumentException("The controller {0} does not have action '{1}'.".Arrange(controller.GetType().Name, actionName));

            this.manager = this.ResolveManager();
        }

        /// <inheritdoc />
        protected override void ResolveUrlParamsInternal(string[] urlParams, RequestContext requestContext)
        {
            if (urlParams == null)
                return;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var item = this.GetItemByUrl(url, out redirectUrl);

            if (item != null)
            {
                requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

                var parameters = this.actionMethod.GetParameters();
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
        }

        private TContent GetItemByUrl(string url, out string redirectUrl)
        {
            if (this.manager is IContentManager)
            {
                return ((IContentManager)this.manager).GetItemFromUrl(typeof(TContent), url, out redirectUrl) as TContent;
            }
            else
            {
                return ((IUrlProvider)this.manager.Provider).GetItemFromUrl(typeof(TContent), url, out redirectUrl) as TContent;
            }
        }

        private IManager ResolveManager()
        {
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var manager = ManagerBase.GetMappedManager(typeof(TContent), providerName);

            if (!(manager is IContentManager) && !(manager.Provider is IUrlProvider))
                throw new InvalidOperationException("Items of the {0} type cannot be found by URL.".Arrange(typeof(TContent).FullName));

            return manager;
        }

        private IManager manager;
        private string actionName;
        private Func<string> providerNameResolver;
        private MethodInfo actionMethod;
    }
}
