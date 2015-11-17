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
            : base(viewAssemblies, viewPageActivator)
        {
            this.precompiledAssemblies = viewAssemblies.ToArray();
        }

        /// <summary>
        /// Creates a new instance that is a clone of this one.
        /// </summary>
        /// <returns>The new instance.</returns>
        public CompositePrecompiledMvcEngineWrapper Clone()
        {
            return new CompositePrecompiledMvcEngineWrapper(this.precompiledAssemblies);
        }

        private readonly PrecompiledViewAssembly[] precompiledAssemblies;
    }
}
