using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend.FilesMonitoring;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Routing;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Security;
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

            PageManager.Executing -= LayoutInitializer.Provider_Executing;
            PageManager.Executing += LayoutInitializer.Provider_Executing;

            PageManager.Executed -= LayoutInitializer.Provider_Executed;
            PageManager.Executed += LayoutInitializer.Provider_Executed;
        }

        /// <summary>
        /// Uninitializes the functionality related to the layouts.
        /// </summary>
        public virtual void Uninitialize()
        {
            System.Web.Routing.RouteTable.Routes.Remove(this.mvcVersioningRoute);

            PageManager.Executed -= LayoutInitializer.Provider_Executed;
            PageManager.Executing -= LayoutInitializer.Provider_Executing;
        }

        private System.Web.Routing.Route mvcVersioningRoute;
        private const string CreatedPageTemplatesCategoryIds = "created-page-templates";

        private static void Provider_Executing(object sender, ExecutingEventArgs args)
        {
            if (!(args.CommandName == "CommitTransaction" || args.CommandName == "FlushTransaction"))
                return;

            var provider = sender as PageDataProvider;

            var dirtyItems = provider.GetDirtyItems();
            if (dirtyItems.Count == 0)
                return;

            var createdPageTemplates = provider.GetExecutionStateData(CreatedPageTemplatesCategoryIds) as HashSet<string>;
            if (createdPageTemplates == null)
                createdPageTemplates = new HashSet<string>();

            foreach (var item in dirtyItems)
            {
                SecurityConstants.TransactionActionType itemStatus = provider.GetDirtyItemStatus(item);
                var pageTemplate = item as PageTemplate;
                if (pageTemplate != null)
                {
                    if (itemStatus == SecurityConstants.TransactionActionType.New && pageTemplate.Framework == PageTemplateFramework.Mvc)
                    {
                        // the template name is formulated as Bootstrap.default
                        var nameParts = pageTemplate.Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (nameParts.Length > 1)
                        {
                            var package = nameParts[0];

                            // always create the template, desipite if the package is not there
                            // in case the client first performs a sync and then deploys the files by mistake
                            createdPageTemplates.Add(package);
                        }
                    }
                }
            }

            if (createdPageTemplates.Any())
            {
                provider.SetExecutionStateData(CreatedPageTemplatesCategoryIds, createdPageTemplates);
            }
        }

        private static void Provider_Executed(object sender, ExecutedEventArgs args)
        {
            if (args.CommandName != "CommitTransaction")
                return;

            var provider = sender as PageDataProvider;
            var packageNames = provider.GetExecutionStateData(CreatedPageTemplatesCategoryIds) as HashSet<string>;
            if (packageNames != null)
            {
                provider.SetExecutionStateData(CreatedPageTemplatesCategoryIds, null);
                var layoutFileManager = ObjectFactory.Resolve<IFileManager>(Enum.GetName(typeof(ResourceType), ResourceType.Layouts)) as LayoutFileManager;
                layoutFileManager.CreateTemplateCategories(packageNames, true);
            }
        }
    }
}
