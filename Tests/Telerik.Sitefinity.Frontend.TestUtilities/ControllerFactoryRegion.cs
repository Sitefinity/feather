using System;
using System.Web.Mvc;
using Telerik.Sitefinity.Mvc;

namespace Telerik.Sitefinity.Frontend.Test.TestUtilities
{
    /// <summary>
    /// Replaces the current controller factory of the MVC framework.
    /// </summary>
    /// <typeparam name="TFactory">The type of the factory.</typeparam>
    public sealed class ControllerFactoryRegion<TFactory> : IDisposable where TFactory : ISitefinityControllerFactory, new()
    {
        private IControllerFactory prevFactory;

        private TFactory factory;

        /// <summary>
        /// Gets the current controller factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        public TFactory Factory
        {
            get
            {
                return this.factory;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerFactoryRegion{TFactory}"/> class.
        /// </summary>
        public ControllerFactoryRegion()
        {
            this.factory = new TFactory();

            this.prevFactory = ControllerBuilder.Current.GetControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(this.factory);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            ControllerBuilder.Current.SetControllerFactory(this.prevFactory);
        }
    }
}
