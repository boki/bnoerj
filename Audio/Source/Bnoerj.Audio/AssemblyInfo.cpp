// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly:AssemblyTitleAttribute("Bnoerj.Audio")];
[assembly:AssemblyDescriptionAttribute("XACT3 support for the Microsoft XNA Framework")];
[assembly:AssemblyConfigurationAttribute("")];
[assembly:AssemblyCompanyAttribute("Björn Graf")];
[assembly:AssemblyProductAttribute("Bnoerj.Audio")];
[assembly:AssemblyCopyrightAttribute("Copyright (c) Björn Graf")];
[assembly:AssemblyTrademarkAttribute("Microsoft is a registered trademark of Microsoft Corporation. Windows is a registered trademark of Microsoft Corporation. XNA is a trademark of Microsoft Corporation.")];
[assembly:AssemblyCultureAttribute("")];

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the value or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly:AssemblyVersionAttribute("1.0.0.0")];

[assembly:RuntimeCompatibility(WrapNonExceptionThrows = true)];

[assembly:ComVisible(false)];

[assembly:CLSCompliantAttribute(true)];

[assembly:SecurityPermission(SecurityAction::RequestMinimum, UnmanagedCode = true)];
[assembly:SecurityPermission(SecurityAction::RequestMinimum, Execution = true)];
