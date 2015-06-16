using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Ninject;
using Ninject.Parameters;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers
{
    /// <summary>
    /// This class is used for models creation.
    /// </summary>
    public static class ControllerModelFactory
    {
        /// <summary>
        /// Gets the model instance.
        /// </summary>
        /// <typeparam name="T">Type of the model.</typeparam>
        /// <param name="controllerType">The type of the controller for which the model is being resolved.</param>
        /// <param name="constructorParameters">The constructor parameters.</param>
        /// <returns>The model instance.</returns>
        public static T GetModel<T>(Type controllerType, IDictionary<string, object> constructorParameters = null)
        {
            if (controllerType == null)
                throw new ArgumentNullException("controllerType");

            ConstructorArgument[] parameters;
            if (constructorParameters != null && constructorParameters.Count > 0)
            {
                parameters = new ConstructorArgument[constructorParameters.Count];

                int i = 0;
                foreach (var param in constructorParameters)
                {
                    parameters[i] = new ConstructorArgument(param.Key, param.Value);
                    i++;
                }
            }
            else
            {
                parameters = new ConstructorArgument[0];
            }

            if (FrontendModule.Current != null)
            {
                return FrontendModule.Current.DependencyResolver.Get<T>(parameters);
            }
            else
            {
                using (var kernel = new StandardKernel())
                {
                    kernel.Load(ControllerModelFactory.GetTypeHierarchyAssemblies(controllerType));
                    return kernel.Get<T>(parameters);
                }
            }
        }

        private static IEnumerable<Assembly> GetTypeHierarchyAssemblies(Type type)
        {
            var result = new List<Assembly>();
            var currentType = type;

            while (currentType != null && currentType != typeof(Controller) && currentType != typeof(object))
            {
                result.Add(currentType.Assembly);
                currentType = currentType.BaseType;
            }

            return result;
        }
    }
}
