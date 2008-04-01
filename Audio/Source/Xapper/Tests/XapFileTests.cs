// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Bnoerj.Audio.XapElements;

namespace Bnoerj.Audio.Tests
{
	public class XapFileTests
	{
		const String XactFileHeader = "Signature = XACT2;\r\nVersion = 16;\r\nContent Version = 43;\r\nRelease = August 2007;\r\n";
		const String XactEmptySection = "\r\nSection\r\n{\r\n}\r\n";
		const String XactEmptySectionWithEmptyChild = "\r\nSection\r\n{\r\n\r\n    SubSection\r\n    {\r\n    }\r\n}\r\n";
		const String XactSectionWithScalars = "\r\nSection\r\n{\r\n    Foo = Bar;\r\n    Baz = Moo;\r\n}\r\n";
		const String XactSectionWithScalarsAndSubSections = "\r\nSection\r\n{\r\n    Foo = Bar;\r\n    Baz = Moo;\r\n\r\n    SubSection\r\n    {\r\n    }\r\n}\r\n";

		[Fact]
		public void FileHeader()
		{
			XapFile file = new XapFile();
			file.Open(XactFileHeader);

			Assert.Equal("XACT2", file.Signature);
			Assert.Equal("16", file.Version);
			Assert.Equal("43", file.ContentVersion);
			Assert.Equal("August 2007", file.Release);
		}

		[Fact]
		public void EmptySection()
		{
			XapFile file = new XapFile();
			file.Open(XactEmptySection);

			SectionElement root = file["Section"] as SectionElement;
			Assert.NotNull(root);
		}

		[Fact]
		public void EmptySectionWithEmptyChild()
		{
			XapFile file = new XapFile();
			file.Open(XactEmptySectionWithEmptyChild);

			SectionElement root = file["Section"] as SectionElement;
			SectionElement child = root.Value["SubSection"] as SectionElement;
			Assert.NotNull(child);
		}

		[Fact]
		public void SectionWithScalars()
		{
			XapFile file = new XapFile();
			file.Open(XactSectionWithScalars);

			SectionElement root = file["Section"] as SectionElement;
			Assert.NotNull(root);
			Assert.Equal(2, root.Value.ElementCount);

			Assert.IsType(typeof(ScalarElement), root.Value["Foo"]);
			Assert.IsType(typeof(ScalarElement), root.Value["Baz"]);

			Assert.Equal("Bar", (root.Value["Foo"] as ScalarElement).Value);
			Assert.Equal("Moo", (root.Value["Baz"] as ScalarElement).Value);
		}

		[Fact]
		public void SectionWithScalarsAndSubSections()
		{
			XapFile file = new XapFile();
			file.Open(XactSectionWithScalarsAndSubSections);

			SectionElement root = file["Section"] as SectionElement;
			Assert.NotNull(root);
			Assert.Equal(3, root.Value.ElementCount);

			Assert.IsType(typeof(ScalarElement), root.Value["Foo"]);
			Assert.IsType(typeof(ScalarElement), root.Value["Baz"]);
			Assert.IsType(typeof(SectionElement), root.Value["SubSection"]);
		}

		[Fact]
		public void SaveEmptySectionFile()
		{
			XapFile file = new XapFile();
			file.Open(XactEmptySection);

			MemoryStream stream = new MemoryStream();
			file.Save(stream);
			byte[] buffer = stream.ToArray();
			char[] written = Encoding.ASCII.GetChars(buffer);
			Assert.Equal(XactEmptySection.ToCharArray(), written);
		}

		[Fact]
		public void SaveSimpleSectionFile()
		{
			XapFile file = new XapFile();
			file.Open(XactSectionWithScalars);

			MemoryStream stream = new MemoryStream();
			file.Save(stream);
			byte[] buffer = stream.ToArray();
			char[] written = Encoding.ASCII.GetChars(buffer);
			Assert.Equal(XactSectionWithScalars.ToCharArray(), written);
		}

		[Fact]
		public void SaveComplexSectionFile()
		{
			XapFile file = new XapFile();
			file.Open(XactSectionWithScalarsAndSubSections);

			MemoryStream stream = new MemoryStream();
			file.Save(stream);
			byte[] buffer = stream.ToArray();
			char[] written = Encoding.ASCII.GetChars(buffer);
			Assert.Equal(XactSectionWithScalarsAndSubSections.ToCharArray(), written);
		}
	}
}
