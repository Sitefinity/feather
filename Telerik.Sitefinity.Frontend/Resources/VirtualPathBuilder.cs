using System;
using System.IO;
using System.Reflection;
using Telerik.Sitefinity.Abstractions.VirtualPath;

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
            {
                throw new ArgumentNullException("assembly");
            }

            return VirtualPathBuilder.FrontendAssemblyBasePath.Arrange(assembly.GetName().Name);
        }

        /// <summary>
        /// Gets the path definition for the given assembly that is used by the virtual file resolvers.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public PathDefinition GetPathDefinition(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var name = assembly.GetName().Name;
            var result = new PathDefinition
            {
                IsWildcard = true,
                ResolverName = name,
                ResourceLocation = assembly.CodeBase,
                VirtualPath =
                    "~/" + VirtualPathBuilder.FrontendAssemblyBasePath.Arrange(name),
            };

            result.Items.Add("Assembly", assembly);
            return result;
        }

        /// <summary>
        /// Removes the parameters from the virtual path. This will strip everything after the "#".
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns></returns>
        public string RemoveParams(string virtualPath)
        {
            var idx = virtualPath.IndexOf('#');

            return idx == -1 ? virtualPath : virtualPath.Substring(0, idx);
        }

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="pathParams">The path parameters.</param>
        /// <returns></returns>
        public string AddParams(string virtualPath, string pathParams)
        {
            if (!pathParams.IsNullOrEmpty())
                virtualPath += "#" + pathParams + Path.GetExtension(virtualPath);

            return virtualPath;
        }

        private const string FrontendAssemblyBasePath = "Frontend-Assembly/{0}/";
    }
}
