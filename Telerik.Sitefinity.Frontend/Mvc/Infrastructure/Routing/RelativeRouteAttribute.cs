using System;
using System.Web.Mvc;
using System.Web.Mvc.Routing;

namespace Telerik.Sitefinity.Mvc
{
    /// <summary>
    /// Place on a controller or action to expose it via a route that is relative to the current page.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class RelativeRouteAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>
        /// The name of the route.
        /// </value>
        public string Name 
        { 
            get
            {
                return this.wrappedAttribute.Name;
            }

            set
            {
                this.wrappedAttribute.Name = value;
            }
        }

        /// <summary>
        /// Gets the order the route is applied.
        /// </summary>
        /// <value>
        /// The order the route is applied.
        /// </value>
        public int Order 
        { 
            get
            {
                return this.wrappedAttribute.Order;
            }
        }

        /// <summary>
        /// Gets the pattern for the route to match.
        /// </summary>
        /// <value>
        /// The pattern to match.
        /// </value>
        public string Template 
        { 
            get
            {
                return this.wrappedAttribute.Template;
            }
        }

        /// <summary>
        /// Gets the direct route factory corresponding to this attribute.
        /// </summary>
        /// <value>
        /// The direct route factory.
        /// </value>
        public IDirectRouteFactory DirectRouteFactory
        {
            get
            {
                return this.wrappedAttribute;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRouteAttribute"/> class.
        /// </summary>
        public RelativeRouteAttribute() : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRouteAttribute"/> class with the specified template.
        /// </summary>
        /// <param name="template">The pattern of the route to match.</param>
        /// <exception cref="System.ArgumentNullException">template</exception>
        public RelativeRouteAttribute(string template)
        {
            if (template == null)
                throw new ArgumentNullException("template");

            this.wrappedAttribute = new RouteAttribute(template);
        }

        private readonly RouteAttribute wrappedAttribute;
    }
}
