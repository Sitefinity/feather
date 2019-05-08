using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Sitefinity.Frontend.Mvc.Helpers;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class resolve successor items by the URL.
    /// </summary>
    internal class SuccessorsActionParamsMapper : UrlParamsMapperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessorsActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="parentTypes">Parent types of the current type.</param>
        public SuccessorsActionParamsMapper(ControllerBase controller, IEnumerable<Type> parentTypes)
            : this(controller, parentTypes, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessorsActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="parentTypes">Parent types of the current type.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        public SuccessorsActionParamsMapper(ControllerBase controller, IEnumerable<Type> parentTypes, Func<string> providerNameResolver)
            : this(controller, parentTypes, providerNameResolver, SuccessorsActionParamsMapper.DefaultActionName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessorsActionParamsMapper"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="parentTypes">Parent types of the current type.</param>
        /// <param name="providerNameResolver">A function that returns provider name for the content. If null then default provider is used.</param>
        /// <param name="actionName">Name of the action.</param>
        /// <exception cref="System.ArgumentException">When the given controller does not contain a method corresponding to the action name.</exception>
        public SuccessorsActionParamsMapper(ControllerBase controller, IEnumerable<Type> parentTypes, Func<string> providerNameResolver, string actionName)
            : base(controller)
        {
            if (parentTypes == null)
                throw new ArgumentNullException("itemType");

            this.actionName = actionName;
            this.providerNameResolver = providerNameResolver;

            this.ActionMethod = new ReflectedControllerDescriptor(controller.GetType()).FindAction(controller.ControllerContext, actionName);
            if (this.ActionMethod == null)
                throw new ArgumentException("The controller {0} does not have action '{1}'.".Arrange(controller.GetType().Name, actionName));

            this.ParentTypes = parentTypes;
        }

        /// <inheritdoc />
        protected override bool TryMatchUrl(string[] urlParams, RequestContext requestContext, string urlKeyPrefix)
        {
            return this.TryMatchUrl(urlParams, requestContext, true);
        }

        /// <summary>
        /// Adds the parent content item to route data.
        /// </summary>
        /// <param name="requestContext">The request context.</param>
        /// <param name="parentItem">The parent item.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="setUrlParametersResolved">Whether to set the url parameters resolved</param>
        protected virtual void AddParentContentItemToRouteData(RequestContext requestContext, IDataItem parentItem, Type itemType, bool setUrlParametersResolved = true)
        {
            requestContext.RouteData.Values[UrlParamsMapperBase.ActionNameKey] = this.actionName;

            var parameters = this.ActionMethod.GetParameters();
            if (parameters.Length > 0 && parameters[0].ParameterType.IsAssignableFrom(itemType))
            {
                requestContext.RouteData.Values[parameters[0].ParameterName] = parentItem;
            }

            if (requestContext.HttpContext.Request["page"] != null)
                requestContext.RouteData.Values["page"] = int.Parse(requestContext.HttpContext.Request["page"]);

            if (setUrlParametersResolved)
            {
                RouteHelper.SetUrlParametersResolved();
            }
        }

        private bool TryMatchUrl(string[] urlParams, RequestContext requestContext, bool setUrlParametersResolved)
        {
            if (urlParams == null || urlParams.Length == 0)
                return false;

            var url = RouteHelper.GetUrlParameterString(urlParams);
            string redirectUrl;
            var providerName = this.providerNameResolver != null ? this.providerNameResolver() : null;
            var contentItemResolver = new ContentDataItemResolver();

            foreach (var parentType in this.ParentTypes)
            {
                var item = contentItemResolver.GetItemByUrl(url, parentType, providerName, out redirectUrl);

                if (item != null)
                {
                    this.AddParentContentItemToRouteData(requestContext, item, parentType, setUrlParametersResolved);

                    return true;
                }
            }

            if (urlParams.Length > 1)
            {
                this.TryMatchUrl(urlParams.Take(urlParams.Length - 1).ToArray(), requestContext, false);
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the parent types.
        /// </summary>
        /// <value>
        /// The parent types.
        /// </value>
        protected IEnumerable<Type> ParentTypes { get; set; }

        /// <summary>
        /// Gets the action method.
        /// </summary>
        /// <value>
        /// The action method.
        /// </value>
        protected ActionDescriptor ActionMethod { get; private set; }

        /// <summary>
        /// The default details action name.
        /// </summary>
        public const string DefaultActionName = "Successors";

        private string actionName;
        private Func<string> providerNameResolver;
    }
}
