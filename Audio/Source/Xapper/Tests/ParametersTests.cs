// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using Xunit;
using System.IO;

namespace Bnoerj.Audio.Xapper.Tests
{
	public class ParametersTests
	{
		[Fact]
		public void EmptyArgShouldReturnDefaults()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Empty(parameters.ProjectFiles);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void SkipLogo()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "/L" });

			Assert.True(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.Empty(parameters.ProjectFiles);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void OneProject()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "Test.xap" });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.NotEmpty(parameters.ProjectFiles);
			Assert.Equal(Path.GetFullPath("Test.xap"), parameters.ProjectFiles[0]);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void OneProjectAndOutputPath()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "Test.xap", "." });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.NotEmpty(parameters.ProjectFiles);
			Assert.Equal(Path.GetFullPath("Test.xap"), parameters.ProjectFiles[0]);
			Assert.Equal(Path.GetFullPath("."), parameters.OutputPath);
		}

		[Fact]
		public void TwoProject()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "Test.xap", "Test2.xap" });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.NotEmpty(parameters.ProjectFiles);
			Assert.Equal(Path.GetFullPath("Test.xap"), parameters.ProjectFiles[0]);
			Assert.Equal(Path.GetFullPath("Test2.xap"), parameters.ProjectFiles[1]);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void TwoProjectAndOutputPath()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "Test.xap", "Test2.xap", "." });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.NotEmpty(parameters.ProjectFiles);
			Assert.Equal(Path.GetFullPath("Test.xap"), parameters.ProjectFiles[0]);
			Assert.Equal(Path.GetFullPath("Test2.xap"), parameters.ProjectFiles[1]);
			Assert.Equal(Path.GetFullPath("."), parameters.OutputPath);
		}

		[Fact]
		public void ExcludeWaveBank()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "/X:WAVEBANK" });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.Empty(parameters.ProjectFiles);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void ExcludeSoundBank()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "/X:SOUNDBANK" });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.Empty(parameters.ProjectFiles);
			Assert.Null(parameters.OutputPath);
		}

		[Fact]
		public void ExcludeWaveBankAndSoundBank()
		{
			Parameters parameters = new Parameters();
			parameters.Parse(new String[] { "/X:WAVEBANK", "/X:SOUNDBANK" });

			Assert.False(parameters.SkipLogo);
			Assert.False(parameters.ForceRebuild);
			Assert.False(parameters.NoOverwrite);
			Assert.False(parameters.ValidateOnly);
			Assert.Equal(Parameters.ReportType.None, parameters.Report);
			Assert.Empty(parameters.ProjectFiles);
			Assert.Null(parameters.OutputPath);
		}
	}
}
