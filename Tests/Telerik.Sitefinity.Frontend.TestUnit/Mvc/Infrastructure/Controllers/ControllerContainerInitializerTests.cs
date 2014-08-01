using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
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
        #region Initialize

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Verifies that Initialize will call RegisterVirtualPaths and InitializeControllers methods.")]
        public void Initialize_CallsRegisterVirtualPathsAndInitializeControllers()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();
            
            IEnumerable<Assembly> assemblies = null;
            initializer.GetAssembliesMock = () => 
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

            //Act
            initializer.Initialize();

            //Assert
            Assert.IsTrue(registerVirtualPathsCalled, "RegisterVirtualPaths was not called.");
            Assert.IsTrue(initializeControllersCalled, "InitializeControllers was not called.");
        }

        #endregion

        #region GetAssemblies

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetAssemblies will try to load all assemblies that are considered controller assemblies.")]
        public void GetAssemblies_ReturnsAllControllerAssemblies()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();

            var assemblyFiles = new string[] { "No Controllers", "No Controllers", "Controllers", "No Controllers", "Controllers" };
            initializer.GetAssemblyFileNamesMock = () => assemblyFiles;
            initializer.IsControllerContainerMock = asmFileName => asmFileName.Equals("Controllers");

            var triedToLoad = new List<string>(2);
            initializer.LoadAssemblyMock = asmFileName =>
            {
                triedToLoad.Add(asmFileName);
                return Assembly.GetExecutingAssembly();
            };
            
            var triedToInitializeContainers = new List<string>(2);
            initializer.InitializeControllerContainerMock = assembly =>
            {
                triedToInitializeContainers.Add(assembly.FullName);
            };

            //Act
            var result = initializer.GetAssembliesPublic();

            //Assert
            Assert.AreEqual(2, result.Count(), "Not all controller assemblies were returned.");
            Assert.AreEqual(2, triedToLoad.Count, "Not all controller assemblies were loaded.");
            Assert.AreEqual(2, triedToInitializeContainers.Count, "Not all controller assemblies were initialized.");
            Assert.IsFalse(triedToLoad.Any(asmFile => !asmFile.Equals("Controllers")), "Some assemblies were loaded that were not controller assemblies.");
        }

        #endregion

        #region GetControllers

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether GetControllers can recognize controllers from the current assembly.")]
        public void GetControllers_CurrentAssembly_GetsControllers()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();

            //Act
            var types = initializer.GetControllersPublic(new[] { AssemblyLoaderHelper.GetTestUtilitiesAssembly() });

            //Assert
            Assert.IsNotNull(types, "GetControllers returned null.");

            //Don't check for exact count. They can change while this assembly grows.
            Assert.IsTrue(types.Count() > 0, "No controllers were found.");
        }

        #endregion

        #region RegisterVirtualPaths

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterVirtualPaths method registers a virtual path and route for the given assemblies.")]
        public void RegisterVirtualPaths_CurrentAssembly_VirtualPathAndRouteRegistered()
        {
            //Arrange
            Assembly assembly = Assembly.GetExecutingAssembly();
            PathDefinition vpDefinition = null;
            var initializer = new DummyControllerContainerInitializer();

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                ObjectFactory.Container.RegisterType<IResourceResolverStrategy, DummyResolverStrategy>(new ContainerControlledLifetimeManager());
                
                var strategy = (DummyResolverStrategy)ObjectFactory.Container.Resolve<IResourceResolverStrategy>();
                strategy.ExistsMock = (def, vp) =>
                    {
                        vpDefinition = def;
                        return true;
                    };

                Config.RegisterSection<VirtualPathSettingsConfig>();
                Config.RegisterSection<ControlsConfig>();

                //Act
                initializer.RegisterVirtualPathsPublic(new[] { assembly });

                VirtualPathManager.FileExists("~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(assembly));
            }

            //Assert
            Assert.AreNotEqual(0, RouteTable.Routes.Count, "No routes were registered.");
            Assert.IsNotNull(RouteTable.Routes.OfType<Route>().FirstOrDefault(r => r.Url == "Frontend-Assembly/Telerik.Sitefinity.Frontend.TestUnit/{*Params}"));
            Assert.IsNotNull(vpDefinition, "Virtual path definition was not found.");
            Assert.AreEqual(vpDefinition.ResourceLocation, assembly.CodeBase, "The resolved virtual path definition was not expected.");
        }

        #endregion

        #region InitializeControllers

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether InitializeControllers registers all controllers and replaces the ControllerFactory.")]
        public void InitializeControllers_TwoControllers_BothAreRegisteredAndControllerFactoryIsPrepared()
        {
            //Arrange
            var registeredControllers = new List<Type>(2);
            var initializer = new DummyControllerContainerInitializer();
            using (new ObjectFactoryContainerRegion())
            {
                initializer.RegisterControllerFactoryMock = () =>
                    {
                        ObjectFactory.Container.RegisterType<ISitefinityControllerFactory, DummyControllerFactory>();
                    };

                initializer.RegisterControllerMock = (t) =>
                    {
                        registeredControllers.Add(t);
                    };

                //Act
                initializer.InitializeControllersPublic(new[] { typeof(DummyController), typeof(DummyControllerContainerInitializer) });
            }

            //Assert
            Assert.IsInstanceOfType(ControllerBuilder.Current.GetControllerFactory(), typeof(DummyControllerFactory), "Controller factory was not set.");
            Assert.AreEqual(2, registeredControllers.Count, "Not all widgets were registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyController)), "The first controller was not registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyControllerContainerInitializer)), "The second controller was not registered.");
        }

        #endregion

        #region RegisterController

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterController method registers the controller and its string resources, and in the same time no routes are registered.")]
        public void RegisterController_DummyController_IsRegisteredInStore()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();
            var controller = typeof(DummyController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), 
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();

                //Act
                initializer.RegisterControllerPublic(controller);

                //Assert
                var resourceRegistered = ObjectFactory.Container.IsRegistered(typeof(DummyControllerResoruces), Res.GetResourceClassId(typeof(DummyControllerResoruces)));
                Assert.IsTrue(resourceRegistered, "String resources were not registered for the controller.");
            }

            var registration = ControllerStore.Controllers().SingleOrDefault(c => c.ControllerType == controller);
            Assert.IsNotNull(registration, "Controller was not registered.");

            var route = RouteTable.Routes[controller.Name];
            Assert.IsNull(route, "Route was registered for the controller.");
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterController method registers the DesignerController without registering any routes.")]
        public void RegisterController_DesignerController_NotRegistersAnylRoutes()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();
            var controller = typeof(DesignerController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();

                //Act
                initializer.RegisterControllerPublic(controller);
            }

            //Assert
            var registration = ControllerStore.Controllers().SingleOrDefault(c => c.ControllerType == controller);
            Assert.IsNotNull(registration, "DesignerController was not registered.");

            var route = RouteTable.Routes[controller.Name];
            Assert.IsNull(route, "Route was registered for the controller.");
        }

        #endregion

        #region IsControllerContainer

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return false if null is passed as filename.")]
        public void IsControllerContainer_NullFileName_ReturnsFalse()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();

            //Act
            var result = initializer.IsControllerContainerPublic(null);

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return false for non-existing file.")]
        public void IsControllerContainer_NonExistingFileName_ReturnsFalse()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();

            //Act
            var result = initializer.IsControllerContainerPublic("C:\\NonExistingPath\\NonExistingAssembly.dll");

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerContainer will return true for the assembly where the Designer is at.")]
        public void IsControllerContainer_DesignerAssembly_ReturnTrue()
        {
            //Arrange
            var initializer = new DummyControllerContainerInitializer();

            //Act
            var result = initializer.IsControllerContainerPublic(typeof(DesignerController).Assembly.CodeBase);

            //Assert
            Assert.IsTrue(result);
        }

        #endregion
    }
}
