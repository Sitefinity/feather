using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Data.Events;
using Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Modules.ControlTemplates.Web.UI;
using Telerik.Sitefinity.Modules.Pages;
using Telerik.Sitefinity.Pages.Model;
using Telerik.Sitefinity.Services;
using Telerik.Sitefinity.Utilities.TypeConverters;
using Telerik.Sitefinity.Web.UI;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class contains logic for configuring the functionality related to the resolving of resources.
    /// </summary>
    internal class ResourcesInitializer
    {
        public void Initialize()
        {
            ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());

            SystemManager.RegisterRoute(
                            "ServerContext",
                            new Route(
                                      "Telerik.Sitefinity.Frontend/ServerContext.js",
                                      new RouteHandler<ServerContextHandler>()),
                                      typeof(ResourcesInitializer).Assembly.GetName().Name,
                                      requireBasicAuthentication: false);

            ObjectFactory.Container.RegisterType<DialogBase, MvcControlTemplateEditor>(typeof(ControlTemplateEditor).Name);

            EventHub.Subscribe<IDataEvent>(x => this.HandleIDataEvent(x));

            var resourceClass = typeof(InfrastructureResources);
            var resourceClassId = Res.GetResourceClassId(resourceClass);
            if (!ObjectFactory.Container.IsRegistered(resourceClass, resourceClassId))
            {
                Res.RegisterResource(resourceClass);
            }
        }

        /// <summary>
        /// Modifies newly created <see cref="ControlPresentation"/> by adding suffix, when creating a widget template for MVC Widget.
        /// </summary>
        /// <param name="eventArgs">The event args.</param>
        public void HandleIDataEvent(IDataEvent eventArgs)
        {
            var action = eventArgs.Action;
            var contentType = eventArgs.ItemType;

            if (action == DataEventAction.Created && contentType.BaseType == typeof(PresentationData))
            {
                var itemId = eventArgs.ItemId;
                var providerName = eventArgs.ProviderName;
                var manager = PageManager.GetManager(providerName);
                var controlPresentationItem = manager.GetPresentationItem<ControlPresentation>(itemId);
                var controlType = TypeResolutionService.ResolveType(controlPresentationItem.ControlType, throwOnError: false);

                if (controlType != null && typeof(IController).IsAssignableFrom(controlType) && !controlPresentationItem.FriendlyControlName.Contains(MvcConstants.MvcSuffix))
                    controlPresentationItem.FriendlyControlName = string.Format(CultureInfo.InvariantCulture, MvcConstants.MvcFieldControlNameTemplate, controlPresentationItem.FriendlyControlName);

                manager.SaveChanges();
            }
        }
    }
}
