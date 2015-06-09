using System;
using System.Collections.Generic;
using System.Reflection;

using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUtilities.DummyClasses.Mvc
{
    /// <summary>
    /// This class represents dummy extension of  <see cref="Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.ControllerContainerInitializer"/> used for test purposes only.
    /// </summary>
    internal class DummyControllerContainerInitializer : ControllerContainerInitializer
    {
        /// <summary>
        /// A function that will be called through <see cref="RetrieveAssembies"/> method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Func<IEnumerable<Assembly>> RetrieveAssembliesMock { get; set; }

        #region Public Properties and fields
        /// <summary>
        /// A function that will be called through <see cref="GetControllers"/> method.
        /// </summary>
        public Func<IEnumerable<Assembly>, IEnumerable<Type>> GetControllersMock { get; set; }

        /// <summary>
        /// An action that will be called through <see cref="RegisterVirtualPaths"/> method.
        /// </summary>
        public Action<IEnumerable<Assembly>> RegisterVirtualPathsMock { get; set; }

        /// <summary>
        /// An action that will be called through <see cref="InitializeControllers"/> method.
        /// </summary>
        public Action<IEnumerable<Type>> InitializeControllersMock { get; set; }

        /// <summary>
        /// An action that will be called through <see cref="InitializeControllerContainer"/> method.
        /// </summary>
        public Action<Assembly> InitializeControllerContainerMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="RetrieveAssembliesFileNames"/> method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public Func<IEnumerable<string>> RetrieveAssembliesFileNamesMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="LoadAssembly"/> method.
        /// </summary>
        public Func<string, Assembly> LoadAssemblyMock { get; set; }

        /// <summary>
        /// A function that will be called through <see cref="IsControllerContainer"/> method.
        /// </summary>
        public Func<string, bool> IsControllerContainerMock { get; set; }

        /// <summary>
        /// An action that will be called through <see cref="RegisterController"/> method.
        /// </summary>
        public Action<Type> RegisterControllerMock { get; set; }

        /// <summary>
        /// An action that will be called through <see cref="RegisterControllerFactory"/> method.
        /// </summary>
        public Action RegisterControllerFactoryMock { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Exposes <see cref="Assemblies"/> for test purposes.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Assembly> GetAssembliesPublic()
        {
            return this.RetrieveAssemblies();
        }

        /// <summary>
        /// Exposes <see cref="GetControllers"/> for test purposes.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<Type> GetControllersPublic(IEnumerable<Assembly> assemblies)
        {
            return this.GetControllers(assemblies);
        }

        /// <summary>
        /// Exposes <see cref="RegisterVirtualPaths"/> for test purposes.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies.
        /// </param>
        public void RegisterVirtualPathsPublic(IEnumerable<Assembly> assemblies)
        {
            this.RegisterVirtualPaths(assemblies);
        }

        /// <summary>
        /// Exposes <see cref="InitializeControllers"/> for test purposes.
        /// </summary>
        /// <param name="controllers">
        /// The controllers.
        /// </param>
        public void InitializeControllersPublic(IEnumerable<Type> controllers)
        {
            this.InitializeControllers(controllers);
        }

        /// <summary>
        /// Exposes <see cref="RegisterController"/> for test purposes.
        /// </summary>
        /// <param name="widgetType">
        /// The widget Type.
        /// </param>
        public void RegisterControllerPublic(Type widgetType)
        {
            this.RegisterController(widgetType);
        }

        /// <summary>
        /// Exposes <see cref="IsControllerContainer"/> for test purposes.
        /// </summary>
        /// <param name="assemblyFileName">
        /// The assembly File Name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsControllerContainerPublic(string assemblyFileName)
        {
            return this.IsControllerContainer(assemblyFileName);
        }

        /// <inheritdoc />
        public override IEnumerable<Assembly> RetrieveAssemblies()
        {
            if (this.RetrieveAssembliesMock != null)
                return this.RetrieveAssembliesMock();
            else
                return base.RetrieveAssemblies();
        }

        #endregion

        #region Overrides

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

        /// <summary>
        /// The register controller factory.
        /// </summary>
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

        /// <summary>
        /// The get assembly file names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        protected override IEnumerable<string> RetrieveAssembliesFileNames()
        {
            if (this.RetrieveAssembliesFileNamesMock != null)
                return this.RetrieveAssembliesFileNamesMock();

            return base.RetrieveAssembliesFileNames();
        }

        /// <inheritdoc />
        protected override Assembly LoadAssembly(string assemblyFileName)
        {
            if (this.LoadAssemblyMock != null)
                return this.LoadAssemblyMock(assemblyFileName);

            return base.LoadAssembly(assemblyFileName);
        }

        /// <summary>
        /// The is controller container.
        /// </summary>
        /// <param name="assemblyFileName">
        /// The assembly file name.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool IsControllerContainer(string assemblyFileName)
        {
            if (this.IsControllerContainerMock != null)
                return this.IsControllerContainerMock(assemblyFileName);

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

        /// <inheritdoc />
        protected override void InitializeCustomRouting()
        {
            // Skip registering types.
        }

        #endregion
    }
}
