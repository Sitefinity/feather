﻿using System;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Frontend.Resources;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Modules.Pages.Web.Services;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Web;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class contains logic for registration and initialization of the layouts.
    /// </summary>
    internal class LayoutInitializer : IInitializer
    {
        /// <summary>
        /// Registers the types and resolvers related to the layouts functionality.
        /// </summary>
        public virtual void Initialize()
        {
            ObjectFactory.Container.RegisterType<ILayoutResolver, LayoutResolver>(new ContainerControlledLifetimeManager());
            ObjectFactory.Container.RegisterType<IVirtualFileResolver, LayoutMvcPageResolver>("PureMvcPageResolver", new ContainerControlledLifetimeManager(), new InjectionConstructor());

            VirtualPathManager.AddVirtualFileResolver<LayoutVirtualFileResolver>(string.Format(System.Globalization.CultureInfo.InvariantCulture, "~/{0}*", LayoutVirtualFileResolver.ResolverPath), typeof(LayoutVirtualFileResolver).FullName);
            ObjectFactory.Container.RegisterType<PageRouteHandler, MvcPageRouteHandler>();
            ObjectFactory.Container.RegisterType<PageEditorRouteHandler, MvcPageEditorRouteHandler>();
            ObjectFactory.Container.RegisterType<TemplateEditorRouteHandler, MvcTemplateEditorRouteHandler>();

            this.mvcVersioningRoute = new System.Web.Routing.Route("Sitefinity/Versioning/{itemId}/{VersionNumber}", ObjectFactory.Resolve<MvcVersioningRouteHandler>());
            System.Web.Routing.RouteTable.Routes.Insert(1, this.mvcVersioningRoute);
        }

        /// <summary>
        /// Uninitializes the functionality related to the layouts.
        /// </summary>
        public virtual void Uninitialize()
        {
            System.Web.Routing.RouteTable.Routes.Remove(this.mvcVersioningRoute);
        }

        private System.Web.Routing.Route mvcVersioningRoute;
    }
}
