// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.IO;

namespace Bnoerj.Audio.Xapper
{
	class Parameters
	{
		public enum ReportType
		{
			None,
			Concise,
			Verbose
		}

		enum WriteMode
		{
			None,
			ForceRebuild,
			NoOverwrite
		}

		String commandLine;

		bool skipLogo;
		WriteMode writeMode;
		bool validateOnly;
		ReportType reportType;

		List<String> inputFiles;
		String outputPath;

		public bool SkipLogo
		{
			get { return skipLogo; }
		}

		public bool ForceRebuild
		{
			get { return (writeMode & WriteMode.ForceRebuild) != WriteMode.None; }
		}

		public bool NoOverwrite
		{
			get { return (writeMode & WriteMode.NoOverwrite) != WriteMode.None; }
		}

		public bool ValidateOnly
		{
			get { return validateOnly; }
		}

		public ReportType Report
		{
			get { return reportType; }
		}

		public String[] ProjectFiles
		{
			get { return inputFiles.ToArray(); }
		}

		public String OutputPath
		{
			get { return outputPath; }
		}

		public String XactBld3Commandline
		{
			get { return commandLine; }
		}

		public void Parse(String[] args)
		{
			List<String> xactBld3Args = new List<String>();
			xactBld3Args.Add("/WINDOWS");

			skipLogo = false;
			writeMode = WriteMode.None;
			validateOnly = false;
			reportType = ReportType.None;

			inputFiles = new List<String>();
			outputPath = null;

			for (int i = 0; i < args.Length; i++)
			{
				String arg = args[i];
				if (arg[0] == '/')
				{
					xactBld3Args.Add(arg);

					arg = arg.Substring(1);
					if (arg.Equals("L", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						skipLogo = true;
					}
					else if (arg.Equals("F", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						writeMode = WriteMode.ForceRebuild;
					}
					else if (arg.Equals("N", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						writeMode = WriteMode.NoOverwrite;
					}
					else if (arg.Equals("V", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						validateOnly = true;
					}
					else if (arg.StartsWith("R:", StringComparison.InvariantCultureIgnoreCase) == true)
					{
						arg = arg.Substring(2);
						reportType = (ReportType)Enum.Parse(typeof(ReportType), arg, true);
					}
				}
				else if (arg.EndsWith(".xap", StringComparison.InvariantCultureIgnoreCase) == true)
				{
					inputFiles.Add(Path.GetFullPath(arg));
					xactBld3Args.Add(Path.GetFullPath(arg).Replace(".xap", ".xap3"));
				}
				else if (i == args.Length - 1)
				{
					outputPath = Path.GetFullPath(arg);
					xactBld3Args.Add(outputPath);
				}
			}

			commandLine = String.Join(" ", xactBld3Args.ToArray());
		}
	}
}
