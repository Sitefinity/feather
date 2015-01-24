using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Telerik.Sitefinity.Frontend")]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("11b4c603-bf5d-4ef3-8843-d3f047a07377")]

[assembly: ControllerContainer]

[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestUnit")]
[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestUtilities")]
[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestIntegration")]
