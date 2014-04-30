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
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;
using Telerik.Sitefinity.Frontend.Resources.Resolvers;
using Telerik.Sitefinity.Frontend.Test.DummyClasses;
using Telerik.Sitefinity.Frontend.Test.Mvc.Controllers;
using Telerik.Sitefinity.Frontend.Test.TestUtilities;
using Telerik.Sitefinity.Localization;
using Telerik.Sitefinity.Localization.Configuration;
using Telerik.Sitefinity.Mvc;
using Telerik.Sitefinity.Mvc.Store;
using Telerik.Sitefinity.Project.Configuration;
using Telerik.Sitefinity.Web.Configuration;

namespace Telerik.Sitefinity.Frontend.Test.Mvc.Infrastructure
{
    [TestClass]
    public class ControllerInitializerTest
    {
        #region Initialize

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Verifies that Initialize will call RegisterVirtualPaths and InitializeControllers methods.")]
        public void Initialize_CallsRegisterVirtualPathsAndInitializeControllers()
        {
            var initializer = new DummyInitializer();
            
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

            initializer.Initialize();
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
            var initializer = new DummyInitializer();

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

            var result = initializer.GetAssembliesPublic();
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
            var initializer = new DummyInitializer();
            var types = initializer.GetControllersPublic(new[] { Assembly.GetExecutingAssembly() });

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
            Assembly assembly = Assembly.GetExecutingAssembly();
            PathDefinition vpDefinition = null;

            var initializer = new DummyInitializer();
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

                initializer.RegisterVirtualPathsPublic(new[] { assembly });

                VirtualPathManager.FileExists("~/" + FrontendManager.VirtualPathBuilder.GetVirtualPath(assembly));
            }

            Assert.AreNotEqual(0, RouteTable.Routes.Count, "No routes were registered.");
            Assert.IsNotNull(RouteTable.Routes.OfType<Route>().FirstOrDefault(r => r.Url == "Frontend-Assembly/Telerik.Sitefinity.Frontend.Test/{*Params}"));
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
            var registeredControllers = new List<Type>(2);
            var initializer = new DummyInitializer();
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
                initializer.InitializeControllersPublic(new[] { typeof(DummyController), typeof(DummyInitializer) });
            }

            Assert.IsInstanceOfType(ControllerBuilder.Current.GetControllerFactory(), typeof(DummyControllerFactory), "Controller factory was not set.");
            Assert.AreEqual(2, registeredControllers.Count, "Not all widgets were registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyController)), "The first controller was not registered.");
            Assert.IsTrue(registeredControllers.Contains(typeof(DummyInitializer)), "The second controller was not registered.");
        }

        #endregion

        #region RegisterController

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether RegisterController method registers the controller and its string resources, and in the same time no routes are registered.")]
        public void RegisterController_DummyController_IsRegisteredInStore()
        {
            var initializer = new DummyInitializer();
            var controller = typeof(DummyController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(), 
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();
                Config.RegisterSection<ResourcesConfig>();
                Config.RegisterSection<ProjectConfig>();

                initializer.RegisterControllerPublic(controller);

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
            var initializer = new DummyInitializer();
            var controller = typeof(DesignerController);

            using (new ObjectFactoryContainerRegion())
            {
                ObjectFactory.Container.RegisterType<ConfigManager, ConfigManager>(typeof(XmlConfigProvider).Name.ToUpperInvariant(),
                    new InjectionConstructor(typeof(XmlConfigProvider).Name));
                ObjectFactory.Container.RegisterType<XmlConfigProvider, DummyConfigProvider>();

                initializer.RegisterControllerPublic(controller);
            }

            var registration = ControllerStore.Controllers().SingleOrDefault(c => c.ControllerType == controller);
            Assert.IsNotNull(registration, "DesignerController was not registered.");

            var route = RouteTable.Routes[controller.Name];
            Assert.IsNull(route, "Route was registered for the controller.");
        }

        #endregion

        #region IsControllerAssembly

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerAssembly will return false if null is passed as filename.")]
        public void IsControllerAssembly_NullFileName_ReturnsFalse()
        {
            var initializer = new DummyInitializer();
            var result = initializer.IsControllerContainerPublic(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerAssembly will return false for non-existing file.")]
        public void IsControllerAssembly_NonExistingFileName_ReturnsFalse()
        {
            var initializer = new DummyInitializer();
            var result = initializer.IsControllerContainerPublic("C:\\NonExistingPath\\NonExistingAssembly.dll");

            Assert.IsFalse(result);
        }

        [TestMethod]
        [Owner("Boyko-Karadzhov")]
        [Description("Checks whether IsControllerAssembly will return true for the assembly where the Designer is at.")]
        public void IsControllerAssembly_DesignerAssembly_ReturnTrue()
        {
            var initializer = new DummyInitializer();
            var result = initializer.IsControllerContainerPublic(typeof(DesignerController).Assembly.CodeBase);

            Assert.IsTrue(result);
        }

        #endregion

        #region Mocks

        private class DummyInitializer : ControllerContainerInitializer
        {
            public Func<IEnumerable<Assembly>> GetAssembliesMock;
            public Func<IEnumerable<Assembly>, IEnumerable<Type>> GetControllersMock;
            public Action<IEnumerable<Assembly>> RegisterVirtualPathsMock;
            public Action<IEnumerable<Type>> InitializeControllersMock;
            public Action<Assembly> InitializeControllerContainerMock;
            public Func<IEnumerable<string>> GetAssemblyFileNamesMock;
            public Func<string, Assembly> LoadAssemblyMock;
            public Func<string, bool> IsControllerContainerMock;
            public Action<Type> RegisterControllerMock;
            public Action RegisterControllerFactoryMock;

            public IEnumerable<Assembly> GetAssembliesPublic()
            {
                return this.GetAssemblies();
            }

            public IEnumerable<Type> GetControllersPublic(IEnumerable<Assembly> assemblies)
            {
                return this.GetControllers(assemblies);
            }

            public void RegisterVirtualPathsPublic(IEnumerable<Assembly> assemblies)
            {
                this.RegisterVirtualPaths(assemblies);
            }

            public void InitializeControllersPublic(IEnumerable<Type> controllers)
            {
                this.InitializeControllers(controllers);
            }

            public void RegisterControllerPublic(Type widgetType)
            {
                this.RegisterController(widgetType);
            }

            public bool IsControllerContainerPublic(string assemblyFileName)
            {
                return this.IsControllerContainer(assemblyFileName);
            }

            protected override IEnumerable<Assembly> GetAssemblies()
            {
                if (this.GetAssembliesMock != null)
                    return this.GetAssembliesMock();
                else
                    return base.GetAssemblies();;
            }

            protected override void RegisterVirtualPaths(IEnumerable<Assembly> assemblies)
            {
                if (this.RegisterVirtualPathsMock != null)
                    this.RegisterVirtualPathsMock(assemblies);
                else
                    base.RegisterVirtualPaths(assemblies);
            }

            protected override IEnumerable<Type> GetControllers(IEnumerable<Assembly> assemblies)
            {
                if (this.GetControllersMock != null)
                    return this.GetControllersMock(assemblies);
                else
                    return base.GetControllers(assemblies);
            }

            protected override void InitializeControllers(IEnumerable<Type> controllers)
            {
                if (this.InitializeControllersMock != null)
                    this.InitializeControllersMock(controllers);
                else
                    base.InitializeControllers(controllers);
            }

            protected override void RegisterControllerFactory()
            {
                if (this.RegisterControllerFactoryMock != null)
                    this.RegisterControllerFactoryMock();
                else
                    base.RegisterControllerFactory();
            }

            protected override void InitializeControllerContainer(Assembly container)
            {
                if (this.InitializeControllerContainerMock != null)
                    this.InitializeControllerContainerMock(container);
                else
                base.InitializeControllerContainer(container);
            }

            protected override IEnumerable<string> GetAssemblyFileNames()
            {
                if (this.GetAssemblyFileNamesMock != null)
                    return this.GetAssemblyFileNamesMock();
                else
                    return base.GetAssemblyFileNames();
            }

            protected override Assembly LoadAssembly(string assemblyFileName)
            {
                if (this.LoadAssemblyMock != null)
                    return this.LoadAssemblyMock(assemblyFileName);
                else
                    return base.LoadAssembly(assemblyFileName);
            }

            protected override bool IsControllerContainer(string assemblyFileName)
            {
                if (this.IsControllerContainerMock != null)
                    return this.IsControllerContainerMock(assemblyFileName);
                else
                    return base.IsControllerContainer(assemblyFileName);
            }

            protected override void RegisterController(Type controller)
            {
                if (this.RegisterControllerMock != null)
                    this.RegisterControllerMock(controller);
                else
                    base.RegisterController(controller);
            }
        }

        #endregion
    }
}
