using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Web;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;
using Telerik.Sitefinity.Frontend.TestUI.Arrangements;
using Telerik.Sitefinity.TestUI.Arrangements.Core.Framework.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Telerik.Sitefinity.Frontend.TestUI.Arrangements")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("ea1806b3-fb25-4b38-afd4-307184cdfa6a")]

[assembly: PreApplicationStartMethod(typeof(ApplicationPreStart), "Init")]
[assembly: UIArrangement]
[assembly: ControllerContainer]
