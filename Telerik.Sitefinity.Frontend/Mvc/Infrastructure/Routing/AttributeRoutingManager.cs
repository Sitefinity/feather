using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing
{
    /// <summary>
    /// Instances of this class manage attribute routing for controllers.
    /// </summary>
    internal sealed class AttributeRoutingManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeRoutingManager"/> class.
        /// </summary>
        public AttributeRoutingManager()
        {
            this.routesPerController = new Dictionary<string, RouteBase>();
        }

        /// <summary>
        /// Maps the attribute-defined routes for the application.
        /// </summary>
        public void MapMvcAttributeRoutes()
        {
            //// Map direct routes in the global route table.
            RouteTable.Routes.MapMvcAttributeRoutes();

            var routeCollection = new RouteCollection();
            routeCollection.MapMvcAttributeRoutes(new RelativeDirectRouteProvider());

            this.OrganizeRoutes(routeCollection);
        }

        /// <summary>
        /// Determines whether the controller in the specified route data has attribute routing.
        /// </summary>
        /// <param name="routeData">The route data.</param>
        /// <exception cref="System.ArgumentNullException">routeData</exception>
        public bool HasAttributeRouting(RouteData routeData)
        {
            if (routeData == null)
                throw new ArgumentNullException("routeData");

            var controllerName = routeData.Values[FeatherActionInvoker.ControllerNameKey] as string;
            return this.routesPerController.ContainsKey(controllerName);
        }

        /// <summary>
        /// Updates the route data using the applicable routes for the given context. Returns false if there are no applicable routes. Infers controller name from the given route data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="routeData">The route data.</param>
        /// <returns>True if any attribute route matches the request of the given context.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// context
        /// or
        /// routeData
        /// </exception>
        public bool UpdateRouteData(HttpContextBase context, RouteData routeData)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (routeData == null)
                throw new ArgumentNullException("routeData");

            if (context.Request == null || context.Request.RequestContext == null || !routeData.Values.ContainsKey(FeatherActionInvoker.ControllerNameKey))
                return false;

            var controllerName = routeData.Values[FeatherActionInvoker.ControllerNameKey] as string;
            if (this.routesPerController.ContainsKey(controllerName))
            {
                RouteData newRouteData;
                using (new PageRelativePathContextRegion(context))
                {
                    newRouteData = this.routesPerController[controllerName].GetRouteData(context);
                }

                if (newRouteData != null)
                {
                    foreach (KeyValuePair<string, object> value in newRouteData.Values)
                    {
                        if (value.Key != FeatherActionInvoker.ControllerNameKey)
                            routeData.Values[value.Key] = value.Value;
                    }

                    return true;
                }
            }

            return false;
        }

        private void OrganizeRoutes(RouteCollection routeCollection)
        {
            if (routeCollection.Count == 0)
                return;

            var compoundRoute = routeCollection[0] as IEnumerable<RouteBase>;
            if (compoundRoute == null)
                return;

            var controllerRoutes = new Dictionary<string, List<RouteBase>>();
            foreach (var route in compoundRoute)
            {
                var directRoute = route as Route;
                if (directRoute != null)
                {
                    var controllerName = directRoute.Defaults[FeatherActionInvoker.ControllerNameKey] as string;

                    if (directRoute.Defaults.ContainsKey(FeatherActionInvoker.ControllerNameKey))
                    {
                        if (!controllerRoutes.ContainsKey(controllerName))
                            controllerRoutes.Add(controllerName, new List<RouteBase>());

                        controllerRoutes[controllerName].Add(directRoute);
                    }
                }
            }

            foreach (var controllerRoutesList in controllerRoutes)
            {
                if (controllerRoutesList.Value.Count > 0)
                {
                    var collectionRoute = (RouteBase)Activator.CreateInstance(compoundRoute.GetType(), controllerRoutesList.Value);
                    this.routesPerController[controllerRoutesList.Key] = collectionRoute;
                }
            }
        }

        private readonly Dictionary<string, RouteBase> routesPerController;
    }
}
