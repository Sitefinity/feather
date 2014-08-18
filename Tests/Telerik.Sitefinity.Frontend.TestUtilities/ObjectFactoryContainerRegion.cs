using System;
using System.Reflection;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;

namespace Telerik.Sitefinity.Frontend.Test.TestUtilities
{
    /// <summary>
    /// Sets the Container and QueryableExtension properties of the ObjectFactory. Can be used to avoid the initialization.
    /// </summary>
    public sealed class ObjectFactoryContainerRegion : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFactoryContainerRegion"/> class.
        /// </summary>
        public ObjectFactoryContainerRegion() :
            this(new UnityContainer(), new QueryableContainerExtension())
        {
            this.container.AddExtension(this.queryableExtension);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectFactoryContainerRegion"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="queryableExtension">The queryable extension.</param>
        public ObjectFactoryContainerRegion(IUnityContainer container, QueryableContainerExtension queryableExtension)
        {
            this.container = container;
            this.queryableExtension = queryableExtension;

            this.containerField = typeof(ObjectFactory).GetField("container", BindingFlags.Static | BindingFlags.NonPublic);
            this.previousContainer = (IUnityContainer)this.containerField.GetValue(null);
            this.containerField.SetValue(null, container);

            this.queryableQueryableExtensionField = typeof(ObjectFactory).GetField("queryableContainerExtension", BindingFlags.Static | BindingFlags.NonPublic);
            this.previousQueryableExtension = (QueryableContainerExtension)this.queryableQueryableExtensionField.GetValue(null);
            this.queryableQueryableExtensionField.SetValue(null, queryableExtension);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.containerField.SetValue(null, this.previousContainer);
            this.queryableQueryableExtensionField.SetValue(null, this.previousQueryableExtension);
        }

        private FieldInfo containerField;
        private FieldInfo queryableQueryableExtensionField;
        private IUnityContainer previousContainer;
        private QueryableContainerExtension previousQueryableExtension;

        private IUnityContainer container;
        private QueryableContainerExtension queryableExtension;
    }
}
