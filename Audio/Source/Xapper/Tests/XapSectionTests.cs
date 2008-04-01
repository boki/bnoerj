// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using Bnoerj.Audio.XapElements;
using Xunit;

namespace Bnoerj.Audio.Tests
{
	public class XapSectionTests
	{
		[Fact]
		public void NullElementLineShouldNotAddElement()
		{
			Section section = new Section();
			section.AddElement(null);
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void EmptyElementLineShouldNotAddElement()
		{
			Section section = new Section();
			section.AddElement("");
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void ElementLineShouldAddOneElement()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar");
			Assert.Equal(1, section.ElementCount);
			Assert.NotNull(section["Foo"]);
			Assert.Equal("Bar", (section["Foo"] as ScalarElement).Value);
		}

		[Fact]
		public void MultiAssignmentLineShouldAddOneElement()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar = moo");
			Assert.Equal(1, section.ElementCount);
			Assert.Equal("Bar = moo", (section["Foo"] as ScalarElement).Value);
		}

		[Fact]
		public void ElementLineShouldReplaceExisting()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar");
			section.AddElement("Foo = Baz");
			Assert.Equal(1, section.ElementCount);
			Assert.Equal("Baz", (section["Foo"] as ScalarElement).Value);
		}

		[Fact]
		public void NullAssignmentShouldRemoveElement()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar");
			section["Foo"] = null;
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void NullAssignmentShouldRemoveFirstSectionElement()
		{
			Section root = new Section();
			Section child1 = new Section();
			Section child2 = new Section();
			root.AddSection("Dummy", child1);
			root.AddSection("Dummy", child2);
			root["Dummy"] = null;
			Assert.Equal(1, root.ElementCount);
			Assert.Equal(child2, (root["Dummy"] as SectionElement).Value);
		}

		[Fact]
		public void ElementAssignmentShouldreplaceExisting()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar");
			section["Foo"] = new ScalarElement("Foo", "Baz");
			Assert.Equal(1, section.ElementCount);
			Assert.Equal("Baz", (section["Foo"] as ScalarElement).Value);
		}

		[Fact]
		public void SubSectionShouldIncreaseElementCount()
		{
			Section root = new Section();
			root.AddSection("Dummy", new Section());

			Assert.Equal(1, root.ElementCount);
			Assert.NotNull(root["Dummy"]);
			Assert.IsType(typeof(SectionElement), root["Dummy"]);
		}

		[Fact]
		public void SubSectionShouldAddSectionElement()
		{
			Section root = new Section();
			root.AddSection("Dummy", new Section());

			Assert.NotNull(root["Dummy"]);
			Assert.IsType(typeof(SectionElement), root["Dummy"]);
		}

		[Fact]
		public void GetElementsShouldReturnSubSections()
		{
			Section root = new Section();
			Section child1 = new Section();
			Section child2 = new Section();
			root.AddSection("Dummy", child1);
			root.AddSection("Dummy", child2);

			ElementCollection children = root.GetElements("Dummy");
			Assert.NotNull(children);
			Assert.Equal(2, children.Count);
			Assert.Equal(child1, (children[0] as SectionElement).Value);
			Assert.Equal(child2, (children[1] as SectionElement).Value);
		}
	}
}
