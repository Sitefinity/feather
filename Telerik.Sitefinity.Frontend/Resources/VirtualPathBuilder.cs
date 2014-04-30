using System;
using System.Reflection;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure;

namespace Telerik.Sitefinity.Frontend.Resources
{
    /// <summary>
    /// This class contains methods that define the virtual paths of assemblies.
    /// </summary>
    public class VirtualPathBuilder
    {
        /// <summary>
        /// Gets the virtual path of the assembly of a type.
        /// </summary>
        /// <param name="controllerName">Type.</param>
        /// <exception cref="System.ArgumentNullException">type</exception>
        public string GetVirtualPath(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            var assembly = Assembly.GetAssembly(type);
            return this.GetVirtualPath(assembly);
        }

        /// <summary>
        /// Gets the virtual path of the specified assembly.
        /// </summary>
        /// <param name="assembly">The controller container.</param>
        public string GetVirtualPath(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            return VirtualPathBuilder.frontendAssemblyBasePath.Arrange(assembly.GetName().Name);
        }

        private const string frontendAssemblyBasePath = "Frontend-Assembly/{0}/";
    }
}
