using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Personalization;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Personalization
{
    internal class PersonalizationInitializer : IInitializer
    {
        /// <inheritdoc />
        public void Initialize()
        {
            ObjectFactory.Container.RegisterType<IPersonalizedWidgetResolver, PersonalizedMvcWidgetResolver>(new ContainerControlledLifetimeManager());
        }

        /// <inheritdoc />
        public void Uninitialize()
        {
        }
    }
}
