using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    /// <summary>
    /// This class represents a JSON configuration for component definitions.
    /// </summary>
    internal class ScriptDependencyConfigModel
    {
        /// <summary>
        /// Gets or sets the scripts.
        /// </summary>
        /// <value>
        /// the scripts.
        /// </value>
        [DataMember(Name = "scripts")]
        public IEnumerable<string> Scripts { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// the components.
        /// </value>
        [DataMember(Name = "components")]
        public IEnumerable<string> Components { get; set; }

        /// <summary>
        /// Gets or sets the dependant modules.
        /// </summary>
        /// <value>
        /// The dependant modules.
        /// </value>
        [DataMember(Name = "dependantmodules")]
        public IEnumerable<string> DependantModules { get; set; }
    }
}