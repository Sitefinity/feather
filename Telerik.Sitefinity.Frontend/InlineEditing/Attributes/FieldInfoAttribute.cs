using System;

namespace Telerik.Sitefinity.Frontend.InlineEditing.Attributes
{
    /// <summary>
    /// This class is used as attribute which specifies the meta data required to enable inlineEditing for the field
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class FieldInfoAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
       
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string FieldType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FieldInfoAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
         public FieldInfoAttribute(string name, string type)
        {
            this.FieldType = type;
            this.Name = name;
        }
    }
}