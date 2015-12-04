using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This attribute is used to mark assemblies that may contain <see cref="IController"/> types.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ControllerContainerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerContainerAttribute"/> class.
        /// </summary>
        public ControllerContainerAttribute()
            : this(null, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerContainerAttribute"/> class.
        /// </summary>
        /// <param name="initializationType">Type containing the initialization method.</param>
        /// <param name="initializationMethodName">Name of the initialization method.</param>
        public ControllerContainerAttribute(Type initializationType, string initializationMethodName)
            : this(initializationType, initializationMethodName, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerContainerAttribute"/> class.
        /// </summary>
        /// <param name="initializationType">Type containing the initialization method.</param>
        /// <param name="initializationMethodName">Name of the initialization method.</param>
        /// <param name="uninitializationType">Type containing the uninitialization method.</param>
        /// <param name="uninitializationMethodName">Name of the uninitialization method.</param>
        public ControllerContainerAttribute(Type initializationType, string initializationMethodName, Type uninitializationType, string uninitializationMethodName)
        {
            this.InitializationType = initializationType;
            this.InitializationMethod = initializationMethodName;
            this.UninitializationType = uninitializationType;
            this.UninitializationMethod = uninitializationMethodName;
        }

        /// <summary>
        /// Gets the type containing the initialization method.
        /// </summary>
        /// <value>
        /// The type containing the initialization method.
        /// </value>
        public Type InitializationType { get; private set; }

        /// <summary>
        /// Gets the initialization method name.
        /// </summary>
        /// <value>
        /// The name of the initialization method.
        /// </value>
        public string InitializationMethod { get; private set; }

        /// <summary>
        /// Gets the type containing the uninitialization method.
        /// </summary>
        /// <value>
        /// The type containing the uninitialization method.
        /// </value>
        public Type UninitializationType { get; private set; }

        /// <summary>
        /// Gets the uninitialization method name.
        /// </summary>
        /// <value>
        /// The name of the uninitialization method.
        /// </value>
        public string UninitializationMethod { get; private set; }
    }
}
