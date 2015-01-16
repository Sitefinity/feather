using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Telerik.Sitefinity.Frontend.Mvc.Infrastructure.Controllers.Attributes;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Telerik.Sitefinity.Frontend")]
[assembly: AssemblyCompany("Telerik")]
[assembly: AssemblyProduct("Telerik Sitefinity CMS")]
[assembly: AssemblyCopyright("Copyright © Telerik 2015")]
[assembly: AssemblyTrademark("Sitefinity")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("11b4c603-bf5d-4ef3-8843-d3f047a07377")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.1.40.0")]
[assembly: AssemblyFileVersion("1.1.40.0")]

[assembly: ControllerContainer]

[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestUnit")]
[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestUtilities")]
[assembly: InternalsVisibleTo("Telerik.Sitefinity.Frontend.TestIntegration")]
