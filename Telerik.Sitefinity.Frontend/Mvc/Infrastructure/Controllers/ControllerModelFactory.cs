using Ninject;
using Ninject.Parameters;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class is used for models creation 
    /// </summary>
    public static class ControllerModelFactory
    {
        /// <summary>
        /// Gets the model instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assemblies">The assemblies where the module information can be found.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        /// <returns></returns>
        public static object GetModel<T>(IEnumerable<Assembly> assemblies, IDictionary<string, object> constructorParameters)
        {
            using (var kernel = new StandardKernel())
            {
                kernel.Load(assemblies);
                List<ConstructorArgument> parameters = new List<ConstructorArgument>();

                if (constructorParameters.Any())
                {
                    foreach (var param in constructorParameters)
                    {
                        parameters.Add(new ConstructorArgument(param.Key, param.Value));
                    }
                }

                return kernel.Get<T>(parameters.ToArray());
            }
        }
    }
}
