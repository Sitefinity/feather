using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class represents a JSON configuration for designer views.
    /// </summary>
    [DataContract]
    public class DesignerViewConfigModel
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
        /// Gets or sets the scripts that the view requires to be loaded.
        /// </summary>
        /// <value>
        /// The scripts.
        /// </value>
        [DataMember(Name = "scripts")]
        public IList<string> Scripts { get; set; }
    }
}
