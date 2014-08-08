using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class contains logic for configuring the functionality related to the resolving of resources.
    /// </summary>
    public class ResourcesInitiliazer
    {
        public void Initialize()
        {
            ObjectFactory.Container.RegisterType<IResourceResolverStrategy, ResourceResolverStrategy>(new ContainerControlledLifetimeManager());
        }
    }
}
