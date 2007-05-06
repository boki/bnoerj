// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Bnoerj.Locales.Text
{
	/// <summary>
	/// The TextRectangle represents a bounding box for text drawing.
	/// </summary>
	public struct TextRectangle
	{
		static readonly TextRectangle empty = new TextRectangle();

		public float X;
		public float Y;
		public float Width;
		public float Height;

		/// <summary>
		/// Returns an empty rectangle.
		/// </summary>
		public static TextRectangle Empty
		{
			get { return empty; }
		}

		/// <summary>
		/// Initializes a new instance of TextRectangle.
		/// </summary>
		/// <param name="x">The x-coordinate of the rectangle.</param>
		/// <param name="y">The y-coordinate of the rectangle.</param>
		/// <param name="width">The width of the rectangle.</param>
		/// <param name="height">The height of the rectangle.</param>
		public TextRectangle(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Initializes a new instance of TextRectangle.
		/// </summary>
		/// <param name="position">The x- and y-coordinate of the upper-left corner of the rectangle.</param>
		/// <param name="size">The width and height of the rectangle</param>
		public TextRectangle(Vector2 position, Vector2 size)
			: this(position.X, position.Y, size.X, size.Y)
		{
		}

		/// <summary>
		/// Initializes a new instance of TextRectangle.
		/// </summary>
		/// <remarks>The size is not defined.</remarks>
		/// <param name="position">The x- and y-coordinate of the upper-left corner of the rectangle.</param>
		internal TextRectangle(Vector2 position)
			: this(position.X, position.Y, float.MaxValue, float.MaxValue)
		{
		}

		/// <summary>
		/// Gets the x-coordinate of the left side of the rectangle.
		/// </summary>
		public float Left
		{
			get { return (float)Math.Round(X); }
		}

		/// <summary>
		/// Gets the y-coordinate of the top of the rectangle.
		/// </summary>
		public float Top
		{
			get { return (float)Math.Round(Y); }
		}

		/// <summary>
		/// Gets the x-coordinate of the right side of the rectangle.
		/// </summary>
		public float Right
		{
			get { return (float)Math.Round(X + Width); }
		}

		/// <summary>
		/// Gets the y-coordinate of the bottom of the rectangle.
		/// </summary>
		public float Bottom
		{
			get { return (float)Math.Round(Y + Height); }
		}

		/// <summary>
		/// Gets whether or not the size of the rectangle is defined.
		/// </summary>
		internal bool HasSize
		{
			get { return Width < float.MaxValue && Height < float.MaxValue; }
		}

		/// <summary>
		/// Gets the position of the rectangle.
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2((float)Math.Round(X), (float)Math.Round(Y)); }
		}

		/// <summary>
		/// Gets the size of the rectangle.
		/// </summary>
		public Vector2 Size
		{
			get { return new Vector2((float)Math.Round(Width), (float)Math.Round(Height)); }
		}
	}
}
