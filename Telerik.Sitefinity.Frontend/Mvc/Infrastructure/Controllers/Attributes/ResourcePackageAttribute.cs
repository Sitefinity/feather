using System;

namespace Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes
{
    /// <summary>
    /// This attribute is used to mark assemblies that contain a precompiled resource package.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class ResourcePackageAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePackageAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the package.</param>
        public ResourcePackageAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the package.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
    }
}
