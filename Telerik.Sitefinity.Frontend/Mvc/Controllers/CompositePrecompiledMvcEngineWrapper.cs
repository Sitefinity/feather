using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RazorGenerator.Mvc;

namespace Telerik.Sitefinity.Frontend.Mvc.Controllers
{
    /// <summary>
    /// View engine that serves precompiled views from several assemblies.
    /// </summary>
    public class CompositePrecompiledMvcEngineWrapper : CompositePrecompiledMvcEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        public CompositePrecompiledMvcEngineWrapper(params PrecompiledViewAssembly[] viewAssemblies)
            : this(viewAssemblies, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        public CompositePrecompiledMvcEngineWrapper(IEnumerable<PrecompiledViewAssembly> viewAssemblies, IViewPageActivator viewPageActivator)
            : this(viewAssemblies, viewPageActivator, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositePrecompiledMvcEngineWrapper"/> class.
        /// </summary>
        /// <param name="viewAssemblies">The view assemblies.</param>
        /// <param name="viewPageActivator">The view page activator.</param>
        /// <param name="packageName">Name of the package.</param>
        public CompositePrecompiledMvcEngineWrapper(IEnumerable<PrecompiledViewAssembly> viewAssemblies, IViewPageActivator viewPageActivator, string packageName)
            : base(viewAssemblies, viewPageActivator)
        {
            this.precompiledAssemblies = viewAssemblies.ToArray();
            this.packageName = packageName;
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>
        /// The name of the package.
        /// </value>
        public string PackageName
        {
            get
            {
                return this.packageName;
            }
        }

        /// <summary>
        /// Creates a new instance that is a clone of this one.
        /// </summary>
        /// <returns>The new instance.</returns>
        public CompositePrecompiledMvcEngineWrapper Clone()
        {
            return new CompositePrecompiledMvcEngineWrapper(this.precompiledAssemblies, null, this.PackageName);
        }

        private readonly PrecompiledViewAssembly[] precompiledAssemblies;
        private readonly string packageName;
    }
}
