﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc
{
    /// <summary>
    /// This class represents dummy extension of  <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.ControllerContainerInitializer"/> used for test purposes only.
    /// </summary>
    public class DummyControllerContainerInitializer : ControllerContainerInitializer
    {
        #region Public Properties and fields

        /// <summary>
        /// A function that will be called through <see cref="GetAssemblies"/> method.
        /// </summary>
        public Func<IEnumerable<Assembly>> GetAssembliesMock;

        /// <summary>
        /// A function that will be called through <see cref="GetControllers"/> method.
        /// </summary>
        public Func<IEnumerable<Assembly>, IEnumerable<Type>> GetControllersMock;

        /// <summary>
        /// An action that will be called through <see cref="RegisterVirtualPaths"/> method.
        /// </summary>
        public Action<IEnumerable<Assembly>> RegisterVirtualPathsMock;

        /// <summary>
        /// An action that will be called through <see cref="InitializeControllers"/> method.
        /// </summary>
        public Action<IEnumerable<Type>> InitializeControllersMock;

        /// <summary>
        /// An action that will be called through <see cref="InitializeControllerContainer"/> method.
        /// </summary>
        public Action<Assembly> InitializeControllerContainerMock;

        /// <summary>
        /// A function that will be called through <see cref="GetAssemblyFileNames"/> method.
        /// </summary>
        public Func<IEnumerable<string>> GetAssemblyFileNamesMock;

        /// <summary>
        /// A function that will be called through <see cref="LoadAssembly"/> method.
        /// </summary>
        public Func<string, Assembly> LoadAssemblyMock;

        /// <summary>
        /// A function that will be called through <see cref="IsControllerContainer"/> method.
        /// </summary>
        public Func<string, bool> IsControllerContainerMock;

        /// <summary>
        /// An action that will be called through <see cref="RegisterController"/> method.
        /// </summary>
        public Action<Type> RegisterControllerMock;

        /// <summary>
        /// An action that will be called through <see cref="RegisterControllerFactory"/> method.
        /// </summary>
        public Action RegisterControllerFactoryMock;

        #endregion

        #region Public methods

        /// <summary>
        /// Exposes <see cref="GetAssemblies"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssembliesPublic()
        {
            return this.GetAssemblies();
        }

        /// <summary>
        /// Exposes <see cref="GetControllers"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetControllersPublic(IEnumerable<Assembly> assemblies)
        {
            return this.GetControllers(assemblies);
        }

        /// <summary>
        /// Exposes <see cref="RegisterVirtualPaths"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public void RegisterVirtualPathsPublic(IEnumerable<Assembly> assemblies)
        {
            this.RegisterVirtualPaths(assemblies);
        }

        /// <summary>
        /// Exposes <see cref="InitializeControllers"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public void InitializeControllersPublic(IEnumerable<Type> controllers)
        {
            this.InitializeControllers(controllers);
        }

        /// <summary>
        /// Exposes <see cref="RegisterController"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public void RegisterControllerPublic(Type widgetType)
        {
            this.RegisterController(widgetType);
        }

        /// <summary>
        /// Exposes <see cref="IsControllerContainer"/> for test purposes.
        /// </summary>
        /// <returns></returns>
        public bool IsControllerContainerPublic(string assemblyFileName)
        {
            return this.IsControllerContainer(assemblyFileName);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override IEnumerable<Assembly> GetAssemblies()
        {
            if (this.GetAssembliesMock != null)
                return this.GetAssembliesMock();
            else
                return base.GetAssemblies(); ;
        }

        /// <inheritdoc />
        protected override void RegisterVirtualPaths(IEnumerable<Assembly> assemblies)
        {
            if (this.RegisterVirtualPathsMock != null)
                this.RegisterVirtualPathsMock(assemblies);
            else
                base.RegisterVirtualPaths(assemblies);
        }

        /// <inheritdoc />
        protected override IEnumerable<Type> GetControllers(IEnumerable<Assembly> assemblies)
        {
            if (this.GetControllersMock != null)
                return this.GetControllersMock(assemblies);
            else
                return base.GetControllers(assemblies);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override void RegisterController(Type controller)
        {
            if (this.RegisterControllerMock != null)
                this.RegisterControllerMock(controller);
            else
                base.RegisterController(controller);
        }

        #endregion
    }
}
