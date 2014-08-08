using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Layouts
{
    /// <summary>
    /// This class contains logic for registration and initialization of the layouts.
    /// </summary>
    public class LayoutInitializer
    {
        /// <summary>
        /// Registers the types and resolvers related to the layouts functionality.
        /// </summary>
        public virtual void Initialize()
        {
            ObjectFactory.Container.RegisterType<ILayoutResolver, LayoutResolver>(new ContainerControlledLifetimeManager());

            VirtualPathManager.AddVirtualFileResolver<LayoutVirtualFileResolver>(string.Format("{0}{1}{2}", "~/", LayoutVirtualFileResolver.ResolverPath, "*"), typeof(LayoutVirtualFileResolver).FullName);
        }
    }
}
