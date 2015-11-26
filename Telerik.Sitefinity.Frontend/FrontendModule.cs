using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ninject;
using Telerik.OpenAccess;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Services;

namespace Telerik.Sitefinity.Frontend
{
    /// <summary>
    /// A module that will be invoked by Sitefinity.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Field is disposed on Unload.")]
    public class FrontendModule : ModuleBase
    {
        /// <summary>
        /// Gets the current instance of the module.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static FrontendModule Current
        {
            get
            {
                return (FrontendModule)SystemManager.GetModule("Feather");
            }
        }

        /// <summary>
        /// Gets the landing page id for each module inherit from <see cref="SecuredModuleBase"/> class.
        /// </summary>
        /// <value>The landing page id.</value>
        public override Guid LandingPageId
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// Gets the CLR types of all data managers provided by this module.
        /// </summary>
        /// <value>An array of <see cref="Type"/> objects.</value>
        public override Type[] Managers
        {
            get { return new Type[0]; }
        }

        /// <summary>
        /// Gets the dependency resolver. Can be used for overriding the default implementations of some interfaces.
        /// </summary>
        /// <value>
        /// The dependency resolver.
        /// </value>
        public IKernel DependencyResolver
        {
            get
            {
                return this.ninjectDependencyResolver;
            }
        }

        /// <summary>
        /// Installs the specified initializer.
        /// </summary>
        /// <param name="initializer">The initializer.</param>
        public override void Install(SiteInitializer initializer)
        {
            FrontendModuleInstaller.Install(initializer);
        }

        /// <summary>
        /// Initializes the service with specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
            Bootstrapper.Initialized += this.Bootstrapper_Initialized;

            this.ninjectDependencyResolver = new StandardKernel();
            FrontendModuleInstaller.Initialize(this.DependencyResolver);
        }

        /// <summary>
        /// This method is invoked during the unload process of an active module from Sitefinity, e.g. when a module is deactivated. For instance this method is also invoked for every active module during a restart of the application. 
        /// Typically you will use this method to unsubscribe the module from all events to which it has subscription.
        /// </summary>
        public override void Unload()
        {
            this.Uninitialize();
            FrontendModuleUninstaller.Unload(this.initializers.Value);
            base.Unload();
        }

        /// <summary>
        /// Uninstall the module from Sitefinity system. Deletes the module artifacts added with Install method.
        /// </summary>
        /// <param name="initializer">The site initializer instance.</param>
        public override void Uninstall(SiteInitializer initializer)
        {
            this.Uninitialize();
            FrontendModuleUninstaller.Uninstall(this.initializers.Value, initializer);
            base.Uninstall(initializer);
        }

        /// <summary>
        /// Upgrades this module from the specified version.
        /// </summary>
        /// <param name="initializer">The Site Initializer. A helper class for installing Sitefinity modules.</param>
        /// <param name="upgradeFrom">The version this module us upgrading from.</param>
        public override void Upgrade(SiteInitializer initializer, Version upgradeFrom)
        {
            base.Upgrade(initializer, upgradeFrom);
            FrontendModuleUpgrader.Upgrade(upgradeFrom);
        }
        
        /// <summary>
        /// Handles the Initialized event of the Bootstrapper.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sitefinity.Data.ExecutedEventArgs"/> instance containing the event data.</param>
        protected virtual void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
                FrontendModuleInstaller.Bootstrapper_Initialized(this.initializers.Value);
        }

        /// <summary>
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            return null;
        }

        // Called both by Unload and Uninstall
        private void Uninitialize()
        {
            if (this.ninjectDependencyResolver != null && !this.ninjectDependencyResolver.IsDisposed)
                this.ninjectDependencyResolver.Dispose();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
        }

        internal Lazy<IEnumerable<IInitializer>> initializers = new Lazy<IEnumerable<IInitializer>>(() =>
            typeof(FrontendModule).Assembly.GetTypes().Where(t => typeof(IInitializer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).Select(t => Activator.CreateInstance(t) as IInitializer).ToList());
        
        internal IKernel ninjectDependencyResolver;
        internal const string FrontendServiceName = "Telerik.Sitefinity.Frontend";
    }
}
