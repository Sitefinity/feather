using System;
using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers;

namespace Telerik.Sitefinity.Frontend.TestUnit.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// Contains tests for the <see cref="ControllerContainerAttribute"/>.
    /// </summary>
    [TestClass]
    public class ControllerContainerAttributeTests
    {
        #region Initialize

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and no Initialize and no Uninitialize methods are specified, they are not called.")]
        public void AssemblyWithoutInitializeOrUninitializeMethodShouldNotCallMethodsOnInitialize()
        {
            var assembly = this.CreateAssembly(false, false);

            new DummyControllerContainerInitializer().InitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and Initialize method and no Uninitialize method specified, only initialized is called.")]
        public void AssemblyWithInitializeAndWithoutUninitializeMethodShouldCallOnlyInitializeMethodOnInitialize()
        {
            var assembly = this.CreateAssembly(true, false);

            new DummyControllerContainerInitializer().InitializeControllerContainer(assembly);

            Assert.IsTrue(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and no Initialize method and Uninitialize method specified, only uninitialized is called.")]
        public void AssemblyWithNoInitializeAndWithUninitializeMethodShouldCallOnlyUninitializeMethodOnInitialize()
        {
            var assembly = this.CreateAssembly(false, true);

            new DummyControllerContainerInitializer().InitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and both Initialize and Uninitialize methods specified, both are called.")]
        public void AssemblyWithInitializeAndUninitializeMethodShouldCallBothMethodsOnInitialize()
        {
            var assembly = this.CreateAssembly(true, true);

            new DummyControllerContainerInitializer().InitializeControllerContainer(assembly);

            Assert.IsTrue(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        #endregion

        #region Uninitialize

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and no Initialize and no Uninitialize methods are specified, they are not called.")]
        public void AssemblyWithoutInitializeOrUninitializeMethodShouldNotCallMethodsOnUninitialize()
        {
            var assembly = this.CreateAssembly(false, false);

            new DummyControllerContainerInitializer().UninitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and Initialize method and no Uninitialize method specified, only initialized is called.")]
        public void AssemblyWithInitializeAndWithoutUninitializeMethodShouldCallOnlyInitializeMethodOnUninitialize()
        {
            var assembly = this.CreateAssembly(true, false);

            new DummyControllerContainerInitializer().UninitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsFalse(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and no Initialize method and Uninitialize method specified, only uninitialized is called.")]
        public void AssemblyWithNoInitializeAndWithUninitializeMethodShouldCallOnlyUninitializeMethodOnUninitialize()
        {
            var assembly = this.CreateAssembly(false, true);

            new DummyControllerContainerInitializer().UninitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsTrue(this.IsUninitializedCalled(assembly));
        }

        [TestMethod]
        [Owner("dzhenko")]
        [Description("Ensures that when an assembly has attribute and both Initialize and Uninitialize methods specified, both are called.")]
        public void AssemblyWithInitializeAndUninitializeMethodShouldCallBothMethodsOnUninitialize()
        {
            var assembly = this.CreateAssembly(true, true);

            new DummyControllerContainerInitializer().UninitializeControllerContainer(assembly);

            Assert.IsFalse(this.IsInitializedCalled(assembly));
            Assert.IsTrue(this.IsUninitializedCalled(assembly));
        }

        #endregion

        #region Private Methods

        private bool IsInitializedCalled(Assembly assembly)
        {
            var boolObj = assembly.GetTypes().FirstOrDefault().GetProperty("Initialized", BindingFlags.Static | BindingFlags.Public).GetValue(null) as bool?;
            return boolObj.HasValue && boolObj.Value;
        }

        private bool IsUninitializedCalled(Assembly assembly)
        {
            var boolObj = assembly.GetTypes().FirstOrDefault().GetProperty("Uninitialized", BindingFlags.Static | BindingFlags.Public).GetValue(null) as bool?;
            return boolObj.HasValue && boolObj.Value;
        }

        /// <summary>
        /// Compiles assembly in memory with the <see cref="ControllerContainerAttribute"/> that has Initialize and Uninitialize methods.
        /// </summary>
        /// <param name="hasInitialize">if set to <c>true</c> the attribute has initialize method name.</param>
        /// <param name="hasUninitialize">if set to <c>true</c> the attribute has initialize method name.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Could not compile assembly</exception>
        private Assembly CreateAssembly(bool hasInitialize, bool hasUninitialize)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            var frontendAssemblyName = Assembly.GetExecutingAssembly().GetReferencedAssemblies().FirstOrDefault(a => a.Name == "Telerik.Sitefinity.Frontend");
            var frontendAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == frontendAssemblyName.FullName);
            parameters.ReferencedAssemblies.Add(frontendAssembly.Location);

            var source = @"
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

[assembly: ControllerContainer(typeof(ControllerContainerAttributeTestsAssembly.Initializer), " +
    (hasInitialize ? "\"Initialize\"" : "\"\"") + 
@", typeof(ControllerContainerAttributeTestsAssembly.Initializer), " +
    (hasUninitialize ? "\"Uninitialize\"" : "\"\"") +
@")]

namespace ControllerContainerAttributeTestsAssembly
{
    public static class Initializer
    {
        public static void Initialize()
        {
            Initialized = true;
        }

        public static void Uninitialize()
        {
            Uninitialized = true;
        }

        public static bool Initialized { get;private set; }

        public static bool Uninitialized { get;private set; }
    }
}
";

            var compilerResults = provider.CompileAssemblyFromSource(parameters, source);

            if (compilerResults.Errors.HasErrors)
                throw new ArgumentException("Could not compile assembly");

            return compilerResults.CompiledAssembly;
        }

        private class DummyControllerContainerInitializer : ControllerContainerInitializer
        {
            public void InitializeControllerContainer(Assembly assembly)
            {
                base.InitializeControllerContainer(assembly);
            }

            public void UninitializeControllerContainer(Assembly assembly)
            {
                base.UninitializeControllerContainer(assembly);
            }
        }

        #endregion
    }
}
