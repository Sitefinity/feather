using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using global::Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Microsoft.Practices.Unity;
using Telerik.Sitefinity.Abstractions;
using Telerik.Sitefinity.Abstractions.VirtualPath;
using Telerik.Sitefinity.Abstractions.VirtualPath.Configuration;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Configuration.Data;
using Telerik.Sitefinity.Frontend.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Configs;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.LocalizationResources;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.ResourceResolvers;
using Telerik.Sitefinity.Frontend.TestUtilities.Mvc.Controllers;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// Ensures that ControllerContainerInitializer class is working correctly.
    /// </summary>
    [TestClass]
    public class ControllerContainerInitializerTests
    {
        #region Public Methods and Operators

        /// <summary>
        /// The get assemblies_ returns all controller assemblies.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetAssemblies will try to load all assemblies that are considered controller assemblies.")]
        public void GetAssemblies_ReturnsAllControllerAssemblies()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            var assemblyFiles = new[] { "No Controllers", "No Controllers", "Controllers", "No Controllers", "Controllers" };

            initializer.RetrieveAssembliesFileNamesMock = () => assemblyFiles;
            initializer.IsControllerContainerMock = asmFileName => asmFileName.Equals("Controllers");

            var triedToLoad = new List<string>(2);
            initializer.LoadAssemblyMock = asmFileName =>
                {
                    triedToLoad.Add(asmFileName);
                    return Assembly.GetExecutingAssembly();
                };

            var triedToInitializeContainers = new List<string>(2);
            initializer.InitializeControllerContainerMock = assembly => triedToInitializeContainers.Add(assembly.FullName);

            // Act
            var result = initializer.GetAssembliesPublic();

            // Assert
            Assert.AreEqual(2, result.Count(), "Not all controller assemblies were returned.");
            Assert.AreEqual(2, triedToLoad.Count, "Not all controller assemblies were loaded.");
            Assert.AreEqual(2, triedToInitializeContainers.Count, "Not all controller assemblies were initialized.");
            Assert.IsFalse(triedToLoad.Any(asmFile => !asmFile.Equals("Controllers")), "Some assemblies were loaded that were not controller assemblies.");
        }

        /// <summary>
        /// The get controllers_ current assembly_ gets controllers.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetControllers can recognize controllers from the current assembly.")]
        public void GetControllers_CurrentAssembly_GetsControllers()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            // Act
            var types = initializer.GetControllersPublic(new[] { AssemblyLoaderHelper.GetTestUtilitiesAssembly() });

            // Assert
            Assert.IsNotNull(types, "GetControllers returned null.");

            // Don't check for exact count. They can change while this assembly grows.
            Assert.IsTrue(types.Any(), "No controllers were found.");
        }

        /// <summary>
        /// The initialize controllers_ two controllers_ both are registered and controller factory is prepared.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether InitializeControllers registers all controllers and replaces the ControllerFactory.")]
        public void InitializeControllers_TwoControllers_BothAreRegisteredAndControllerFactoryIsPrepared()
        {
            // Arrange
            var registeredControllers = new List<Type>(2);
            var initializer = new DummyControllerContainerInitializer();

            using (new ObjectFactoryContainerRegion())
            {
                initializer.RegisterControllerFactoryMock = () => ObjectFactory.Container.RegisterType<ISitefinityControllerFactory, DummyControllerFactory>();

                initializer.RegisterControllerMock = registeredControllers.Add;

                // Act
                initializer.InitializeControllersPublic(new[] { typeof(DummyController), typeof(DummyControllerContainerInitializer) });
            }

            // Assert
            Assert.IsInstanceOfType(ControllerBuilder.Current.GetControllerFactory(), typeof(DummyControllerFactory), "Controller factory was not set.");
            Assert.AreEqual(2, registeredControllers.Count, "Not all widgets were registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyController)), "The first controller was not registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyControllerContainerInitializer)), "The second controller was not registered.");
        }

        /// <summary>
        /// The initialize_ calls register virtual paths and initialize controllers.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Verifies that Initialize will call RegisterVirtualPaths and InitializeControllers methods.")]
        public void Initialize_CallsRegisterVirtualPathsAndInitializeControllers()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            IEnumerable<Assembly> assemblies = null;
            initializer.RetrieveAssembliesMock = () =>
                {
                    assemblies = new List<Assembly>();
                    return assemblies;
                };

            bool registerVirtualPathsCalled = false;
            initializer.RegisterVirtualPathsMock = asm =>
                {
                    registerVirtualPathsCalled = true;
                    Assert.AreSame(assemblies, asm, "RegisterVirtualPaths was not called with the expected arguments.");
                };

            IEnumerable<Type> controllerTypes = null;
            initializer.GetControllersMock = asm =>
                {
                    Assert.AreSame(assemblies, asm, "GetControllerTypes was not called with the expected arguments.");
                    controllerTypes = new List<Type>();
                    return controllerTypes;
                };

            bool initializeControllersCalled = false;
            initializer.InitializeControllersMock = types =>
                {
                    initializeControllersCalled = true;
                    Assert.AreSame(controllerTypes, types, "InitializeControllers was not called with the expected arguments");
                };

            // Act
            initializer.Initialize(null);

            // Assert
            Assert.IsTrue(registerVirtualPathsCalled, "RegisterVirtualPaths was not called.");
            Assert.IsTrue(initializeControllersCalled, "InitializeControllers was not called.");
        }

        /// <summary>
        /// The is controller container_ designer assembly_ return true.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return true for the assembly where the Designer is at.")]
        public void IsControllerContainer_DesignerAssembly_ReturnTrue()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            // Act
            var result = initializer.IsControllerContainerPublic(typeof(DesignerController).Assembly.CodeBase);

            // Assert
            Assert.IsTrue(result);
        }

        /// <summary>
        /// The is controller container_ non existing file name_ returns false.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return false for non-existing file.")]
        public void IsControllerContainer_NonExistingFileName_ReturnsFalse()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            // Act
            var result = initializer.IsControllerContainerPublic("C:\\NonExistingPath\\NonExistingAssembly.dll");

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The is controller container_ null file name_ returns false.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return false if null is passed as filename.")]
        public void IsControllerContainer_NullFileName_ReturnsFalse()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();

            // Act
            var result = initializer.IsControllerContainerPublic(null);

            // Assert
            Assert.IsFalse(result);
        }

        /// <summary>
        /// The register controller_ designer controller_ not registers anyl routes.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterController method registers the DesignerController without registering any routes.")]
        public void RegisterController_DesignerController_NotRegistersAnyRoutes()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();
            Type controller = typeof(DesignerController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));

                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();

                // Act
                initializer.RegisterControllerPublic(controller);
            }

            // Assert
            ControllerInfo registration = ControllerStore.Controllers().SingleOrDefault(c => c.ControllerType == controller);
            Assert.IsNotNull(registration, "DesignerController was not registered.");

            RouteBase route = RouteTable.Routes[controller.Name];
            Assert.IsNull(route, "Route was registered for the controller.");
        }

        /// <summary>
        /// The register controller_ dummy controller_ is registered in store.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterController method registers the controller and its string resources, and in the same time no routes are registered.")]
        public void RegisterController_DummyController_IsRegisteredInStore()
        {
            // Arrange
            var initializer = new DummyControllerContainerInitializer();
            Type controller = typeof(DummyController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();

                // Act
                initializer.RegisterControllerPublic(controller);

                // Assert
                var resourceRegistered = ObjectFactory.Container.IsRegistered(typeof(DummyControllerResources), Res.GetResourceClassId(typeof(DummyControllerResources)));
                Assert.IsTrue(resourceRegistered, "String resources were not registered for the controller.");
            }

            ControllerInfo registration = ControllerStore.Controllers().SingleOrDefault(c => c.ControllerType == controller);
            Assert.IsNotNull(registration, "Controller was not registered.");

            var route = RouteTable.Routes[controller.Name];
            Assert.IsNull(route, "Route was registered for the controller.");
        }

        /// <summary>
        /// The register virtual paths_ current assembly_ virtual path and route registered.
        /// </summary>
        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterVirtualPaths method registers a virtual path and route for the given assemblies.")]
        public void RegisterVirtualPaths_CurrentAssembly_VirtualPathAndRouteRegistered()
        {
            // Arrange
            Assembly assembly = Assembly.GetExecutingAssembly();
            PathDefinition definition = null;
            var initializer = new DummyControllerContainerInitializer();

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());

                var strategy = (DummyResolverStrategy)ObjectFactory.Container.Resolve<IResourceResolverStrategy>();
                strategy.ExistsMock = (def, vp) =>
                    {
                        definition = def;
                        return true;
                    };

                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();

                // Act
                initializer.RegisterVirtualPathsPublic(new[] { assembly });

                VirtualPathManager.FileExists("~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(assembly));
            }

            // Assert
            Assert.AreNotEqual(0, RouteTable.Routes.Count, "No routes were registered.");
            Assert.IsNotNull(RouteTable.Routes.OfType<Route>().FirstOrDefault(r => r.Url == "Frontend-Assembly/Telerik.Sitefinity.Frontend.TestUnit/{*Params}"));
            Assert.IsNotNull(definition, "Virtual path definition was not found.");
            Assert.AreEqual(definition.ResourceLocation, assembly.CodeBase, "The resolved virtual path definition was not expected.");
        }

        #endregion
    }
}