using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class represents a JSON configuration for designer views.
    /// </summary>
    [DataContract]
    internal class DesignerViewConfigModel
    {
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        /// <value>
        /// The priority.
        /// </value>
        [DataMember(Name = "priority")]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this view is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the scripts that the view requires to be loaded.
        /// </summary>
        /// <value>
        /// The scripts.
        /// </value>
        [DataMember(Name = "scripts")]
        public IList<string> Scripts { get; set; }
        
        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        [DataMember(Name = "components")]
        public IList<string> Components { get; set; }
    }
}
