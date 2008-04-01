// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Bnoerj.Audio.XapElements;

namespace Bnoerj.Audio.Xapper
{
	class Program
	{
		enum CompressionFormat
		{
			PCM = 0,
			ADPCM = 2,
			WMA = 353,
			XMA = 357,
		}

		const String Xact3BldFileName = "XactBld3.exe";

		static void Main(String[] args)
		{
			Parameters parameters = new Parameters();
			parameters.Parse(args);

			if (parameters.SkipLogo == false)
			{
				PrintLogo();
			}

			if (parameters.ProjectFiles.Length == 0)
			{
				PrintHelp();
				return;
			}

			// Process files
			foreach (String input in parameters.ProjectFiles)
			{
				String output = Path.ChangeExtension(input, ".xap3");

				// Only build XACT3 file if required/requested
				if (File.GetLastWriteTime(input) > File.GetLastWriteTime(output) ||
					parameters.ForceRebuild == true)
				{
					Console.WriteLine("Converting {0}...", input);
					ConvertProject(input, output);
				}
			}

			Console.WriteLine();

			//
			// Build XACT3 binaries
			//

			// Check if the output directory exists
			if (String.IsNullOrEmpty(parameters.OutputPath) == false &&
				File.Exists(parameters.OutputPath) == false)
			{
				Directory.CreateDirectory(parameters.OutputPath);
			}

			try
			{
				String dxSDKDir = Environment.GetEnvironmentVariable("DXSDK_DIR");
				String path = "";
				if (String.IsNullOrEmpty(dxSDKDir) == false)
				{
					path = Path.Combine(dxSDKDir, @"Utilities\Bin\x86\");
				}
				else
				{
					Console.WriteLine("WARNING: DXSDK_DIR environment variable not found. Trying PATH.");
				}
				String exeFileName = Path.Combine(path, Xact3BldFileName);

				Process xact3BldProcess = new Process();
				xact3BldProcess.StartInfo = new ProcessStartInfo(exeFileName, parameters.XactBld3Commandline);
				xact3BldProcess.StartInfo.UseShellExecute = false;
				xact3BldProcess.StartInfo.CreateNoWindow = true;
				xact3BldProcess.StartInfo.RedirectStandardError = true;
				xact3BldProcess.StartInfo.RedirectStandardOutput = true;
				xact3BldProcess.Start();
				do
				{
					Console.Write(xact3BldProcess.StandardError.ReadToEnd());
					Console.Write(xact3BldProcess.StandardOutput.ReadToEnd());
				} while (xact3BldProcess.WaitForExit(500) == false);
			}
			catch (Win32Exception)
			{
				Console.WriteLine("{0} not found. Please ensure that the DXSDK_DIR environment variable is set.", Xact3BldFileName);
			}
		}

		static void ConvertProject(String input, String output)
		{
			XapFile xapFile = new XapFile();
			String content = File.ReadAllText(input);
			xapFile.Open(content);
			xapFile.ContentVersion = "44";
			xapFile.Release = "March 2008";

			ElementCollection presets = xapFile.GetElements("Compression Preset");
			if (presets.Count > 0)
			{
				/*
				Change
				Compression Preset
				{
					Name = Compression Preset;
					Xbox Format Tag = 357;
					Target Sample Rate = 48000;
					Quality = 65;
					Find Best Quality = 0;
					High Freq Cut = 0;
					Loop = 0;
					PC Format Tag = 2;
					Samples Per Block = 128;
				}
				to
				Compression Preset
				{
					Name = Compression Preset;
					Xbox Format Tag = 357;
					Target Sample Rate = 48000;
					XMA Quality = 65;
					Find Best Quality = 0;
					High Freq Cut = 0;
					Loop = 0;
					PC Format Tag = 353;
					WMA Quality = 65;
				}
				*/

				// Add WMA format to all compression presets that define XMA compression,
				// using the same quality setting
				for (int c = 0; c < presets.Count; c++)
				{
					SectionElement preset = presets[c] as SectionElement;
					ScalarElement xboxFormatTag = preset.Value["Xbox Format Tag"] as ScalarElement;
					int xboxFormatTagValue = 0;
					if (Int32.TryParse(xboxFormatTag.Value, out xboxFormatTagValue) == true &&
						(CompressionFormat)xboxFormatTagValue == CompressionFormat.XMA)
					{
						preset.Value["PC Format Tag"].Value = String.Format("{0}", (int)CompressionFormat.WMA);
						ScalarElement xmaQuality = preset.Value["Quality"] as ScalarElement;
						preset.Value["Samples Per Block"] = null;
						preset.Value.Add(new ScalarElement("WMA Quality", xmaQuality.Value));
						//REVIEW: Check Wave size: file too small to encode, must have at least 352256 bytes of data
						//REVIEW: Rename "Quality" element to "XMA Quality"?
					}
				}
			}

			using (FileStream outputStream = new FileStream(output, FileMode.Create))
			{
				xapFile.Save(outputStream);
			}
		}

		static void PrintLogo()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			Console.WriteLine("Bjoerns XACT2 to XACT3 Converter and Build Tool (build {0})", assembly.GetName().Version.ToString());
			Console.WriteLine("Copyright (C) 2008 Bjoern Graf.\n");

			Console.WriteLine("Converts XACT2 to XACT3 projects.\n");
		}

		static void PrintHelp()
		{
			Console.WriteLine("Usage: XAPPER [options] <project> [project 2...] [path]\n");
			Console.WriteLine("   project         Specifies the project file to build.");
			Console.WriteLine("   project 2...    Specifies additional projects to import and build.");
			Console.WriteLine("                   Up to 1023 additional projects may be specified.");
			Console.WriteLine("   path            Specifies the path where output files should be");
			Console.WriteLine("                   written.  If a path is not specified, the paths stored");
			Console.WriteLine("                   in the project file will be used.");
			Console.WriteLine("   /L              Do not print the banner or status.");
			Console.WriteLine("   /F              Force a rebuild of all files.");
			Console.WriteLine("                   Cannot be used with do not overwrite flag.");
			Console.WriteLine("   /N              Do not overwrite files if they already exist.");
			Console.WriteLine("                   Cannot be used with force rebuild flag.");
			Console.WriteLine("   /R:VERBOSE      Build a verbose report (overrides /R:CONCISE).");
			Console.WriteLine("                   Default is project specific.");
			Console.WriteLine("   /R:CONCISE      Build a concise report. Default is project specific.");
			Console.WriteLine("   /X:WAVEBANK     Do not build wave banks.");
			Console.WriteLine("   /X:SOUNDBANK    Do not build sound banks.");
			Console.WriteLine("   /X:HEADER       Do not build the C/C++ header.");
			Console.WriteLine("   /X:CUELIST      Do not export the cue list.");
			Console.WriteLine("   /X:REPORT       Do not generate a report.");
			Console.WriteLine("   /V              Validate the content, but do not actually build.");
		}
	}
}
