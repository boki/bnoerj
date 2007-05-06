// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

using GdiRect = System.Drawing.Rectangle;
using GdiColor = System.Drawing.Color;
using GdiFont = System.Drawing.Font;
using XnaColor = Microsoft.Xna.Framework.Graphics.Color;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

using BnoerjGlyph = Bnoerj.Locales.Text.Glyph;

namespace Bnoerj.Locales.Importer.Fonts
{
	[ContentProcessor(DisplayName = "Font - Bnoerj")]
	class FontProcessor : ContentProcessor<FontSource, FontContent>
	{
		[Flags]
		enum FontEmbeddingFlags
		{
			Installable		= 0x0000,
			NotAllowed		= 0x0001,
			ReadOnly		= 0x0002,
			PreviewAndPrint	= 0x0004,
			Editable		= 0x0008,
			NoSupsetting	= 0x0100,
			BitmapOnly		= 0x0200,
		}

		public override FontContent Process(FontSource input, ContentProcessorContext context)
		{
			List<char> codePoints = new List<char>(input.CodePoints);
			int localeCodePointCount = codePoints.Count;

			// Ensure ASCII characters [33..126] code points are in the processed
			for (byte cp = 33; cp < 127; cp++)
			{
				if (codePoints.FindIndex(delegate(char c) { return c == cp; }) == -1)
				{
					codePoints.Add((char)cp);
				}
			}
			context.Logger.LogMessage("Processing {0} ({1} total) code points for font {1}", localeCodePointCount, codePoints.Count, input.Name);
#if DEBUG
			StringBuilder sb = new StringBuilder(input.CodePoints.Length);
			foreach (char c in codePoints)
			{
				sb.AppendFormat("{0}", c);
			}
			Console.WriteLine(sb.ToString());
#endif
			GdiFont font = new GdiFont(input.Name, 20, FontStyle.Regular, GraphicsUnit.Pixel);

			// Use the fonts (pseudo) EM square
			Size emSquare = TextRenderer.MeasureText("M", font);
			context.Logger.LogMessage("EM Square: {0}x{1}; {2}", emSquare.Width, emSquare.Height, font.Height);
			context.Logger.LogMessage("=> {0}x{1}", 256.0f / (float)emSquare.Width, 256.0f / (float)emSquare.Height);

			Bitmap bmp = new Bitmap(256, 256, PixelFormat.Format32bppArgb);
			Dictionary<char, BnoerjGlyph> glyphs = new Dictionary<char, BnoerjGlyph>(codePoints.Count);

			Graphics g = Graphics.FromImage(bmp);
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

			if (CheckFontForEmbedding(g, font, context) == false)
			{
				context.Logger.LogWarning("No help link",
					new ContentIdentity("definition.locale", "LocaleContent", "Font"),
					"Font {1} may not be embedded.", font.FontFamily);
			}

			TextFormatFlags textFlags = TextFormatFlags.Left | TextFormatFlags.Bottom | TextFormatFlags.NoPadding | TextFormatFlags.GlyphOverhangPadding;
			Size proposedSize = new Size(int.MaxValue, int.MaxValue);
			int x = 0;
			int y = 0;
			int usedArea = 0;
			foreach (char c in codePoints)
			{
				Size size = TextRenderer.MeasureText(g, c.ToString(), font, proposedSize, textFlags);
				if (x + size.Width > bmp.Width)
				{
					x = 0;
					y += font.Height;
				}
				GdiRect rc = new GdiRect(x, y, size.Width, size.Height);
				TextRenderer.DrawText(g, c.ToString(), font, rc, GdiColor.White, GdiColor.Black, textFlags);

				BnoerjGlyph glyph = new BnoerjGlyph();
				glyph.Bounds = new XnaRect(x, y, size.Width, size.Height);
				glyphs.Add(c, glyph);

				x += size.Width;

				usedArea += size.Width * size.Height;
			}

			// Draw a font height square in the lower right corner
			x = bmp.Width - font.Height;
			y = bmp.Height - font.Height;
			g.FillRectangle(Brushes.White, x, y, font.Height, font.Height);

			context.Logger.LogMessage("Font efficiency: Using {0:p} of texture area", (float)usedArea / (float)(bmp.Width * bmp.Height));

			SortedDictionary<uint, int> kerningPairs = GetKerningPairs(g, font, glyphs, context);

			PixelBitmapContent<XnaColor> texture = new PixelBitmapContent<XnaColor>(256, 256);
			GdiRect srcRect = new GdiRect(0, 0, bmp.Width, bmp.Height);
			BitmapData bmpData = bmp.LockBits(srcRect, ImageLockMode.WriteOnly, bmp.PixelFormat);
			int bytes = bmp.Width * bmp.Height * 4;
			byte[] bgraValues = new byte[bytes];
			System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, bgraValues, 0, bytes);
			bmp.UnlockBits(bmpData);

			// Set bitmap data to white and the alpha to its luminance value
			for (int i = 0; i < bytes; i += 4)
			{
				float l = (float)bgraValues[i + 2] * 0.312713f +
					(float)bgraValues[i + 1] * 0.329016f +
					(float)bgraValues[i + 0] * 0.358271f;
				bgraValues[i + 3] = (byte)l;
				bgraValues[i + 2] = 255; // red
				bgraValues[i + 1] = 255; // green
				bgraValues[i + 0] = 255; // blue
			}

			texture.SetPixelData(bgraValues);
			return new FontContent(texture, font.Height, glyphs, kerningPairs);
		}

		bool CheckFontForEmbedding(Graphics g, GdiFont font, ContentProcessorContext context)
		{
			bool embeddingAllowed = false;

			IntPtr hDC = g.GetHdc();
			IntPtr hFont = NativeMethods.Gdi32.SelectObject(hDC, font.ToHfont());

			uint cbSize = NativeMethods.Gdi32.GetOutlineTextMetrics(hDC, 0, IntPtr.Zero);
			if (cbSize == 0)
			{
				return embeddingAllowed;
			}

			IntPtr buffer = Marshal.AllocHGlobal((int)cbSize);
			try
			{
				if (NativeMethods.Gdi32.GetOutlineTextMetrics(hDC, cbSize, buffer) != 0)
				{
					NativeMethods.Gdi32.OUTLINETEXTMETRIC otm;
					otm = (NativeMethods.Gdi32.OUTLINETEXTMETRIC)Marshal.PtrToStructure(buffer, typeof(NativeMethods.Gdi32.OUTLINETEXTMETRIC));

					FontEmbeddingFlags fontEmbedding = (FontEmbeddingFlags)otm.otmfsType;
					embeddingAllowed =
						(fontEmbedding == FontEmbeddingFlags.Installable) ||
						((fontEmbedding & FontEmbeddingFlags.NotAllowed) == 0 &&
						(fontEmbedding & FontEmbeddingFlags.Editable) != 0 ||
						(fontEmbedding & FontEmbeddingFlags.BitmapOnly) != 0);
					context.Logger.LogMessage("Embedding font {0} is {1} as {2}", font.Name, embeddingAllowed ? "allowed" : "not allowed", fontEmbedding.ToString());
					context.Logger.LogMessage("Avg. width: {0}, Max. width: {1}", otm.otmTextMetrics.tmAveCharWidth, otm.otmTextMetrics.tmMaxCharWidth);
				}
			}
			finally
			{
				Marshal.FreeHGlobal(buffer);
			}

			NativeMethods.Gdi32.SelectObject(hDC, hFont);
			g.ReleaseHdc(hDC);

			return embeddingAllowed;
		}

		SortedDictionary<uint, int> GetKerningPairs(Graphics g, GdiFont font, Dictionary<char, BnoerjGlyph> glyphs, ContentProcessorContext context)
		{
			// The kerning value is therefore proportional to the em-height.
			// To convert it to a real value you must divide the font's absolute
			// height in points or pixels by the em-height and then multiply by
			// the kerning value.
			// GetKerningPairs() returns the converted values.

			SortedDictionary<uint, int> kernings = new SortedDictionary<uint, int>();
			IntPtr hDC = g.GetHdc();
			IntPtr hFont = NativeMethods.Gdi32.SelectObject(hDC, font.ToHfont());

			uint pairCount = NativeMethods.Gdi32.GetKerningPairs(hDC, 0, null);
			NativeMethods.Gdi32.KERNINGPAIR[] kerningPairs = new NativeMethods.Gdi32.KERNINGPAIR[pairCount];
			NativeMethods.Gdi32.GetKerningPairs(hDC, pairCount, kerningPairs);

			foreach (NativeMethods.Gdi32.KERNINGPAIR pair in kerningPairs)
			{
				BnoerjGlyph glyph;
				if (pair.iKernAmount != 0 &&
					glyphs.TryGetValue((char)pair.wFirst, out glyph) == true &&
					glyphs.TryGetValue((char)pair.wSecond, out glyph) == true)
				{
					uint kerningKey = ((uint)pair.wFirst) << 16 | (uint)pair.wSecond;
					kernings.Add(kerningKey, pair.iKernAmount);
				}
			}

			context.Logger.LogMessage("Kerning pairs: using {0} of {1}", kernings.Count, pairCount);
			NativeMethods.Gdi32.SelectObject(hDC, hFont);
			g.ReleaseHdc(hDC);

			return kernings;
		}
	}
}
