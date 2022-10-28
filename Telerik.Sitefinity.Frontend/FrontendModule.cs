using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using Ninject;
using Ninject.Modules;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data;
using Telerik.Sitefinity.Frontend;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Services;

[assembly: SitefinityModule(FrontendModule.ModuleName,
                            typeof(FrontendModule),
                            "Feather",
                            "Modern, intuitive, convention based, mobile-first UI for Progress Sitefinity CMS.",
                            StartupType.OnApplicationStart)]

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
                return (FrontendModule)SystemManager.GetModule(FrontendModule.ModuleName);
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
            get { return ManagerTypes; }
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
                return ninjectDependencyResolver;
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
        /// Initializes the module with specified settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public override void Initialize(ModuleSettings settings)
        {
            base.Initialize(settings);

            App.WorkWith()
                .Module(settings.Name)
                    .Initialize()
                    .Configuration<FeatherConfig>();

            this.PreloadControllerTypeCacheAsync();
        }

        /// <summary>
        /// Integrate the module into the system.
        /// </summary>
        public override void Load()
        {
            base.Load();

            this.InitializeDependencyResolver();

            FrontendModuleInstaller.Initialize();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
            Bootstrapper.Initialized += this.Bootstrapper_Initialized;
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
            FrontendModuleUninstaller.Uninstall(this.initializers.Value);
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
            FrontendModuleUpgrader.Upgrade(upgradeFrom, initializer);
        }

        /// <summary>
        /// Gets the module configuration.
        /// </summary>
        protected override ConfigSection GetModuleConfig()
        {
            return Config.Get<FeatherConfig>();
        }

        /// <summary>
        /// Gets the meta data aggregation mode of the module persistence.
        /// </summary>
        protected override ManagersInitializationMode ManagersInitializationMode
        {
            get
            {
                return ManagersInitializationMode.OnStartup;
            }
        }

        /// <summary>
        /// Creates Ninject kernel.
        /// </summary>
        /// <returns></returns>
        protected virtual IKernel CreateKernel()
        {
            var bootstrapper = new Ninject.Web.Common.Bootstrapper();
            IKernel kernel;
            if (bootstrapper.Kernel != null)
                kernel = bootstrapper.Kernel;
            else
            {
                var ninjectSettings = new NinjectSettings();
                ninjectSettings.LoadExtensions = Config.Get<FeatherConfig>().NinjectLoadExtensions;
                kernel = new SitefinityKernel(ninjectSettings);
            }

            return kernel;
        }

        /// <summary>
        /// Handles the Initialized event of the Bootstrapper.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Sitefinity.Data.ExecutedEventArgs"/> instance containing the event data.</param>
        protected virtual void Bootstrapper_Initialized(object sender, ExecutedEventArgs e)
        {
            if (e.CommandName == "Bootstrapped")
            {
                using (new HealthMonitoring.MethodPerformanceRegion("Feather"))
                {
                    FrontendModuleInstaller.Bootstrapper_Initialized(this.initializers.Value);
                }
            }
        }

        // Called both by Unload and Uninstall
        private void Uninitialize()
        {
            this.UninitializeDependencyResolver();

            Bootstrapper.Initialized -= this.Bootstrapper_Initialized;
        }

        /// <summary>
        /// Triggers the initialization of the controller type cache for the default controller factory asynchroniously.
        /// </summary>
        private void PreloadControllerTypeCacheAsync()
        {
            Task.Run(() => this.InitializeControllerTypeCache());
        }

        /// <summary>
        /// Initializes the controller type cache of the default controller factory.
        /// </summary>
        private void InitializeControllerTypeCache()
        {
            DefaultControllerFactory defaultControllerFactory =
                System.Web.Mvc.DependencyResolver.Current.GetService<IControllerFactory>() as DefaultControllerFactory ??
                (ControllerBuilder.Current.GetControllerFactory() as DefaultControllerFactory ?? new DefaultControllerFactory());

            MethodInfo getControllerTypesMethod = defaultControllerFactory.GetType().GetMethod("GetControllerTypes", BindingFlags.Instance | BindingFlags.NonPublic);
            getControllerTypesMethod.Invoke(defaultControllerFactory, null);
        }

        private void InitializeDependencyResolver()
        {
            if (ninjectDependencyResolver != null)
                return;

            ninjectDependencyResolver = this.CreateKernel();
            var assemblies = new ControllerContainerInitializer().ControllerContainerAssemblies;
            var loadedModules = ninjectDependencyResolver.GetModules();

            foreach (var assembly in assemblies)
            {
                var assemblyModules = this.GetNinjectModules(assembly);

                // check assembly for already registered ninject modules
                var registeredAssemblyModules = assemblyModules.Where(module => loadedModules.Where(loadedModule => loadedModule.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)).Any());
                if (registeredAssemblyModules.Any())
                {
                    foreach (var module in assemblyModules)
                    {
                        if (!registeredAssemblyModules.Any(registeredModule => registeredModule.Name.Equals(module.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            ninjectDependencyResolver.Load(module);
                        }
                    }
                }
                else
                {
                    ninjectDependencyResolver.Load(assembly);
                }
            }
        }

        private void UninitializeDependencyResolver()
        {
            if (ninjectDependencyResolver != null && !ninjectDependencyResolver.IsDisposed && ninjectDependencyResolver is SitefinityKernel)
            {
                ninjectDependencyResolver.Dispose();
                ninjectDependencyResolver = null;
            }
        }

        private Lazy<IEnumerable<IInitializer>> initializers = new Lazy<IEnumerable<IInitializer>>(() =>
        {
            try
            {
                return typeof(FrontendModule).Assembly.GetTypes().Where(t => typeof(IInitializer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract).Select(t => Activator.CreateInstance(t) as IInitializer).ToList();
            }
            catch (System.Reflection.ReflectionTypeLoadException ex)
            {
                string message = string.Join(" ", ex.LoaderExceptions.Select(e => e.Message));
                throw new InvalidOperationException(message, ex);
            }
            catch (Exception)
            {
                throw;
            }
        });

        private IEnumerable<INinjectModule> GetNinjectModules(Assembly assembly)
        {
            return assembly.GetExportedTypes().Where<Type>(new Func<Type, bool>(this.IsLoadableModule)).Select<Type, INinjectModule>((Type type) => Activator.CreateInstance(type) as INinjectModule);
        }

        private bool IsLoadableModule(Type type)
        {
            if (!typeof(INinjectModule).IsAssignableFrom(type) || type.IsAbstract || type.IsInterface)
            {
                return false;
            }

            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        /// The <see cref="FrontendModule"/> name.
        /// </summary>
        public const string ModuleName = "Feather";

        private static IKernel ninjectDependencyResolver;
        private static readonly Type[] ManagerTypes = new Type[] { typeof(FilesMonitoring.Data.FileMonitorDataManager) };

        private class SitefinityKernel : StandardKernel
        {
            public SitefinityKernel()
                : base()
            {
            }

            public SitefinityKernel(INinjectSettings settings)
                : base(settings)
            {
            }
        }
    }
}
