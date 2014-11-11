using System.Web.Routing;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Modules.ControlTemplates.Web.UI;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Modules.ControlTemplates.Web.UI;
using Telerik.Sitefinity.Services;
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
        }
    }
}
