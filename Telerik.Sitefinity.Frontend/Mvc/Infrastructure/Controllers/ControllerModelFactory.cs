using System;
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

            var parameters = new List<ConstructorArgument>();
            if (constructorParameters != null && constructorParameters.Any())
            {
                foreach (var param in constructorParameters)
                {
                    parameters.Add(new ConstructorArgument(param.Key, param.Value));
                }
            }

            return FrontendModule.Current.DependencyResolver.Get<T>(parameters.ToArray());
        }
    }
}
