using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System;
using System.Resources;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("DotNetNuke Integrity Checker API")]
[assembly: AssemblyDescription("Core API library containing code which manages File Integrity Databases and verifies the integrity of files in a DotNetNuke environment")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("DotNetNuke.Integrity")]
[assembly: AssemblyCopyright("Copyright ©  2008")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("a0431c80-c414-45a0-aa8f-2ebe515d5fec")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: InternalsVisibleTo("DotNetNuke.Integrity.Tests.Unit")]
[assembly: InternalsVisibleTo("DotNetNuke.Integrity.Tests.Integration")]

[assembly: CLSCompliant(true)]
[assembly: NeutralResourcesLanguage("en-US")]