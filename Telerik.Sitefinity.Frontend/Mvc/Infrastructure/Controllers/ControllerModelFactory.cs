﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            using (var kernel = new StandardKernel())
            {
                var assemblies = ControllerModelFactory.GetTypeHierarchyAssemblies(controllerType).Distinct();
                kernel.Load(assemblies);

                var parameters = new List<ConstructorArgument>();
                if (constructorParameters != null && constructorParameters.Any())
                {
                    foreach (var param in constructorParameters)
                    {
                        parameters.Add(new ConstructorArgument(param.Key, param.Value));
                    }
                }

                return kernel.Get<T>(parameters.ToArray());
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
