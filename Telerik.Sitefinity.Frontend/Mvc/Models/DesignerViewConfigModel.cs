using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Telerik.Sitefinity.Frontend.Mvc.Models
{
    [DataContract]
    public class DesignerViewConfigModel
    {
        [DataMember(Name = "scripts")]
        public IList<string> Scripts { get; set; }
    }
}
