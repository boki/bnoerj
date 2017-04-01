**Bnoerj.Audio** [release:Current release](12163)

This library is a code compatible replacement for the Micrsoft.Xna.Framework.Audio namespace to give Windows based XNA games access to the new XACT3 engine. The advantage of XACT3 over XACT2 is the ability to use WMA compression on Windows, resulting in smaller xwb and hence smaller distributions.

**How to use Bnoerj.Audio**

First off all, you need the March 2008 release of the DirectX SDK to get the XACT3 tools.

To use Bnoerj.Audio in your Windows XNA Game Studio game the only code changes are:

1. Reference the **Bnoerj.Audio.dll** assembly
2. Replace all {{using Microsoft.XNA.Framework.Audio}} with {{using Bnoerj.Audio}}, e.g.
{{#if XBOX
using Microsoft.XNA.Framework.Audio
#else
using Bnoerj.Audio
#endif}}

**XAP Convertion**

You can use the Xapper.exe to convert your XACT2 projects to the XACT3. The command line arguments are the same as for XactBld3.exe and each compression preset that defines XMA compression will be changed to use WMA compression on Windows.

Another way to build the XACT3 runtime binary files from your existing xap project(s) is to open and edit them in XACT3.exe and manually add the WMA compression for the Windows target. Once the compression preset(s) have been updated you might want to save the xap file using a different name (e.g. MyProject.xap3) to not break the build of your Xbox 360 project. To generate the runtime xgs, xsb and xwb files you can either use the XACT3 GUI or the command line tool xact3bld.exe, e.g. "{{XactBld3.exe /L X:HEADER /WINDOWS MyProject.xap3 .}}" (without the quotes) to build the binaries into the current directory (ignoring the output directories defined in the xap file itself).

In both cases the binaries can then be used inside the games content project instead of the xap file.