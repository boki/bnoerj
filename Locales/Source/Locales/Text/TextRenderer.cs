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
using Microsoft.Xna.Framework.Graphics;

namespace Bnoerj.Locales.Text
{
	/// <summary>
	/// Provides methods used to measure and render text. This class cannot be inherited.
	/// </summary>
	public sealed class TextRenderer
	{
		/// <summary>
		/// Represents a positioned character in a TextRun.
		/// </summary>
		struct PositionedGlyph
		{
			/// <summary>Gets or sets the position of the Glyph relative to the TextRun.</summary>
			public Vector2 Position;
			/// <summary>Gets or sets the Glyph.</summary>
			public Glyph Glyph;

			public PositionedGlyph(Vector2 position, Glyph glyph)
			{
				Position = position;
				Glyph = glyph;
			}
		}

		/// <summary>
		/// Represents a sequence of characters that share a single property set.
		/// </summary>
		struct TextRun
		{
			/// <summary>Gets ar sets the position Glyphs in the text run.</summary>
			public PositionedGlyph[] Glyphs;
			/// <summary>Gets or sets the bounding rectangle of the text run.</summary>
			public TextRectangle Bounds;

			public TextRun(PositionedGlyph[] glyphs, TextRectangle bounds)
			{
				Glyphs = glyphs;
				Bounds = bounds;
			}
		}

		// Used for undefined proposed sizes
		static readonly Vector2 BigSize = new Vector2(int.MaxValue, int.MaxValue);

		/// <summary>
		/// Provides the size, in pixels, of the specified text when drawn with the specified font.
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="font">The Font to apply to the measured text.</param>
		/// <returns>The Size, in pixels, of text drawn on a single line with the specified font.</returns>
		public static Vector2 MeasureText(String text, Font font)
		{
			return MeasureText(text, font, BigSize, TextFormatFlags.Default);
		}

		/// <summary>
		/// Provides the size, in pixels, of the specified text when drawn with the
		/// specified font, using the specified size to create an initial bounding
		/// rectangle. 
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="font">The Font to apply to the measured text.</param>
		/// <param name="proposedSize">The Size of the initial bounding rectangle.</param>
		/// <returns>The Size, in pixels, of text drawn with the specified font.</returns>
		public static Vector2 MeasureText(String text, Font font, Vector2 proposedSize)
		{
			return MeasureText(text, font, proposedSize, TextFormatFlags.Default);
		}

		/// <summary>
		/// Provides the size, in pixels, of the specified text when drawn with
		/// the specified font and formatting instructions, using the specified
		/// size to create the initial bounding rectangle for the text. 
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="font">The Font to apply to the measured text.</param>
		/// <param name="flags">The formatting instructions to apply to the measured text.</param>
		/// <returns>The Size, in pixels, of text drawn with the specified font and format.</returns>
		public static Vector2 MeasureText(String text, Font font, TextFormatFlags flags)
		{
			return MeasureText(text, font, BigSize, flags);
		}

		/// <summary>
		/// Provides the size, in pixels, of the specified text when drawn with
		/// the specified font and formatting instructions, using the specified
		/// size to create the initial bounding rectangle for the text. 
		/// </summary>
		/// <param name="text">The text to measure.</param>
		/// <param name="font">The Font to apply to the measured text.</param>
		/// <param name="proposedSize">The size of the initial bounding rectangle.</param>
		/// <param name="flags">The formatting instructions to apply to the measured text.</param>
		/// <returns>The Size, in pixels, of text drawn with the specified font and format.</returns>
		public static Vector2 MeasureText(String text, Font font, Vector2 proposedSize, TextFormatFlags flags)
		{
			Vector2 size;
			BreakText(text, font, proposedSize, flags, out size);
			return size;
		}

		/// <summary>
		/// Draws the specified text at the specified location using the specified
		/// glyph batch, font, and color.
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="position">The Vector2 that represents the upper-left corner of the drawn text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, Vector2 position, Color color)
		{
			DrawText(glyphBatch, text, font, new TextRectangle(position), color, Color.TransparentWhite, TextFormatFlags.Default);
		}

		/// <summary>
		/// Draws the specified text within the specified bounds using the specified
		/// glyph batch, font and color.
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="bounds">The Rectangle that represents the bounds of the text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, TextRectangle bounds, Color color)
		{
			DrawText(glyphBatch, text, font, bounds, color, Color.TransparentWhite, TextFormatFlags.Default);
		}

		/// <summary>
		/// Draws the specified text at the specified location, using the specified
		/// glyph batch, font, color, and back color.
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="position"></param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="background">The Color to apply to the background area of the drawn text.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, Vector2 position, Color color, Color background)
		{
			DrawText(glyphBatch, text, font, new TextRectangle(position), color, background, TextFormatFlags.Default);
		}

		/// <summary>
		/// Draws the specified text within the specified bounds using the specified
		/// glyph batch, font, color, and back color.
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="bounds">The Rectangle that represents the bounds of the text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="background">The Color to apply to the background area of the drawn text.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, TextRectangle bounds, Color color, Color background)
		{
			DrawText(glyphBatch, text, font, bounds, color, background, TextFormatFlags.Default);
		}

		/// <summary>
		/// Draws the specified text at the specified location using the specified
		/// glyph batch, font, color, and formatting instructions. 
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="position">The Vector2 that represents the upper-left corner of the drawn text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="flags">A bitwise combination of the TextFormatFlags values.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, Vector2 position, Color color, TextFormatFlags flags)
		{
			DrawText(glyphBatch, text, font, new TextRectangle(position), color, Color.TransparentWhite, flags);
		}

		/// <summary>
		/// Draws the specified text within the specified bounds using the specified
		/// glyph batch, font, color, and formatting instructions.
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="bounds">The Rectangle that represents the bounds of the text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="flags">A bitwise combination of the TextFormatFlags values.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, TextRectangle bounds, Color color, TextFormatFlags flags)
		{
			DrawText(glyphBatch, text, font, bounds, color, Color.TransparentWhite, flags);
		}

		/// <summary>
		/// Draws the specified text at the specified location using the specified
		/// glyph batch, font, color, back color, and formatting instructions 
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="position">The Vector2 that represents the upper-left corner of the drawn text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="background">The Color to apply to the background area of the drawn text.</param>
		/// <param name="flags">A bitwise combination of the TextFormatFlags values.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, Vector2 position, Color color, Color background, TextFormatFlags flags)
		{
			DrawText(glyphBatch, text, font, new TextRectangle(position), color, background, flags);
		}

		/// <summary>
		/// Draws the specified text within the specified bounds using the specified
		/// glyph batch, font, color, back color, and formatting instructions. 
		/// </summary>
		/// <param name="glyphBatch">The glyph batch in which to draw the text.</param>
		/// <param name="text">The text to draw.</param>
		/// <param name="font">The Font to apply to the drawn text.</param>
		/// <param name="bounds">The Rectangle that represents the bounds of the text.</param>
		/// <param name="color">The Color to apply to the drawn text.</param>
		/// <param name="background">The Color to apply to the background area of the drawn text.</param>
		/// <param name="flags">A bitwise combination of the TextFormatFlags values.</param>
		public static void DrawText(GlyphBatch glyphBatch, String text, Font font, TextRectangle bounds, Color color, Color background, TextFormatFlags flags)
		{
			Vector2 size;
			TextRun[] textRuns = BreakText(text, font, bounds.Size, flags, out size);
			Vector2 offset = bounds.Position;

			// Apply right/bottom/center alignments only when a bounding
			// rectangle is specified
			if (bounds.HasSize == true)
			{
				if ((flags & TextFormatFlags.Right) != TextFormatFlags.Default)
				{
					offset.X = bounds.Right;
				}
				else if ((flags & TextFormatFlags.HorizontalCenter) != TextFormatFlags.Default)
				{
					offset.X += (float)Math.Round(0.5f * bounds.Width);
				}

				// Bottom align only when the text is a single line (only to
				// be consistent with System.Windows.Forms.TextRenderer)
				if ((flags & TextFormatFlags.SingleLine) != TextFormatFlags.Default &&
					(flags & TextFormatFlags.Bottom) != TextFormatFlags.Default)
				{
					offset.Y = bounds.Bottom - font.Height;
				}
				else if ((flags & TextFormatFlags.VerticalCenter) != TextFormatFlags.Default)
				{
					offset.Y += (float)Math.Round(0.5f * (bounds.Height - size.Y));
				}
			}

			foreach (TextRun textRun in textRuns)
			{
				Vector2 textRunOffset = offset + textRun.Bounds.Position;
				if ((flags & TextFormatFlags.Right) != TextFormatFlags.Default)
				{
					textRunOffset.X -= textRun.Bounds.Width;
				}
				else if ((flags & TextFormatFlags.HorizontalCenter) != TextFormatFlags.Default)
				{
					textRunOffset.X -= (float)Math.Round(0.5f * textRun.Bounds.Width);
				}
				glyphBatch.DrawBackground(textRunOffset, textRun.Bounds.Size, background);

				foreach (PositionedGlyph glyph in textRun.Glyphs)
				{
					Vector2 pos = textRunOffset + glyph.Position;
					glyphBatch.Draw(pos, glyph.Glyph.Bounds, color);
				}
			}
		}

		/// <summary>
		/// Breaks the specified text within the specified bounds using the specified
		/// font and formatting instructions. 
		/// </summary>
		/// <param name="text">The text to break.</param>
		/// <param name="font">The Font to apply to the text.</param>
		/// <param name="proposedSize">The Size of the initial bounding rectangle.</param>
		/// <param name="flags">The formatting instructions to apply to the text.</param>
		/// <param name="size">The Size, in pixels, of text broken with the specified font and format.</param>
		/// <returns></returns>
		static TextRun[] BreakText(String text, Font font, Vector2 proposedSize, TextFormatFlags flags, out Vector2 size)
		{
			Glyph spaceGlyph = font.Glyphs[' '];

			Vector2 pos = Vector2.Zero;
			size = new Vector2(0, font.Height);

			uint kerningPairKey = 0;
			int kerningAmount = 0;

			Glyph glyph;
			List<TextRun> textRuns = new List<TextRun>();
			List<PositionedGlyph> glyphs = new List<PositionedGlyph>(text.Length);

			foreach (char c in text)
			{
				bool hasGlyph = font.Glyphs.TryGetValue(c, out glyph);
				if ((flags & TextFormatFlags.SingleLine) == TextFormatFlags.Default &&
					(c == '\n' || (hasGlyph == true && pos.X + glyph.Size.X > proposedSize.X)))
				{
					//FIXME: handle line break on end of words

					textRuns.Add(new TextRun(glyphs.ToArray(), new TextRectangle(0, size.Y - font.Height, pos.X, font.Height)));
					glyphs.Clear();

					size.X = (float)Math.Max(size.X, pos.X);
					size.Y += font.Height;

					if (hasGlyph == true)
					{
						glyphs.Add(new PositionedGlyph(pos, spaceGlyph));
						pos.X += glyph.Size.X;
						kerningPairKey = (uint)c;
					}
					else
					{
						pos.X = 0;
						kerningPairKey = 0;
					}
				}
				else if (hasGlyph == true)
				{
					// Lookup kerning
					kerningPairKey = kerningPairKey << 16 | (uint)c;
					if (font.KerningPairs.TryGetValue(kerningPairKey, out kerningAmount) == true)
					{
						pos.X += kerningAmount;
					}

					glyphs.Add(new PositionedGlyph(pos, glyph));
					pos.X += glyph.Size.X;
				}
				else
				{
					kerningPairKey = kerningPairKey << 16 | (uint)c;
					glyphs.Add(new PositionedGlyph(pos, spaceGlyph));
					pos.X += spaceGlyph.Size.X;
				}
			}

			if (glyphs.Count > 0)
			{
				textRuns.Add(new TextRun(glyphs.ToArray(), new TextRectangle(0, size.Y - font.Height, pos.X, font.Height)));
				size.X = (float)Math.Max(size.X, pos.X);
			}

			return textRuns.ToArray();
		}
	}
}
