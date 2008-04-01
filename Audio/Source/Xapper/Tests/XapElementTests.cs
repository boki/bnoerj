// Copyright (C) 2008, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using Xunit;
using Bnoerj.Audio.XapElements;

namespace Bnoerj.Audio.Xapper.Tests
{
	public class XapElementTests
	{
		[Fact]
		public void NullInputShouldBeEmpty()
		{
			Section section = new Section();
			section.AddElement(null);
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void EmptyInputShouldBeEmpty()
		{
			Section section = new Section();
			section.AddElement("");
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void InvalidInputShouldBeEmpty()
		{
			Section section = new Section();
			section.AddElement("Foo");
			Assert.Equal(0, section.ElementCount);
		}

		[Fact]
		public void SingleAssignment()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar");
			Assert.Equal(1, section.ElementCount);
			ScalarElement element = section["Foo"] as ScalarElement;
			Assert.NotNull(element);
			Assert.Equal("Foo", element.Name);
			Assert.Equal("Bar", element.Value);
		}

		[Fact]
		public void MultipleAssignmentsShouldResultInOneElement()
		{
			Section section = new Section();
			section.AddElement("Foo = Bar = moo");
			Assert.Equal(1, section.ElementCount);
			ScalarElement element = section["Foo"] as ScalarElement;
			Assert.NotNull(element);
			Assert.Equal("Foo", element.Name);
			Assert.Equal("Bar = moo", element.Value);
		}
	}
}
