// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna
{
	using Microsoft.Xna.Framework.Graphics;

	/// <summary>
	/// The Renderer class is used to render scanlines.
	/// </summary>
	/// <example>
	/// // Creation
	/// Agg.Lite.RenderingBuffer rbuf = new Agg.Lite.RenderingBuffer(ptr, w, h, stride);
	/// Agg.Lite.Renderer ren = new Agg.Lite.renderer(rbuf, new Agg.Lite.SpanRgb24());
	/// Agg.Lite.rasterizer ras = new Agg.Lite.Rasterizer();
	///
	/// // Clear the frame buffer
	/// ren.Clear(Microsoft.Xna.Framework.Graphics.Color.Black);
	///
	/// // Making polygon
	/// // ras.MoveTo(. . .);
	/// // ras.LineTo(. . .);
	/// // . . .
	///
	/// // Rendering
	/// ras.Render(ren, new Microsoft.Xna.Framework.Graphics.Color(200, 100, 80));
	/// </example>
	public unsafe class Renderer : IRenderer
	{
		RenderingBuffer m_rbuf;
		ISpan m_span;

		/// <summary>
		/// Initializes a new instance of the Renderer class using the specified
		/// rendering buffer and span.
		/// </summary>
		/// <remarks>
		/// The 'span' argument is one of the span renderers, such as SpanRgb24 
		/// and others.
		/// </remarks>
		/// <param name="renderingBuffer">The rendering buffer to render into.</param>
		/// <param name="span">The span.</param>
		public Renderer(RenderingBuffer renderingBuffer, ISpan span)
		{
			m_rbuf = renderingBuffer;
			m_span = span;
		}

		/// <summary>
		/// Gets the rendering buffer.
		/// </summary>
		public RenderingBuffer Buffer
		{
			get { return m_rbuf; }
		}

		/// <summary>
		/// Clears the rendering buffer to the specified color.
		/// </summary>
		/// <param name="color">Color value to which the render buffer is cleared.</param>
		public void Clear(Color color)
		{
			for (int y = 0; y < m_rbuf.Height; y++)
			{
				m_span.HorizontalLine(m_rbuf.Row(y), 0, m_rbuf.Width, color);
			}
		}

		/// <summary>
		/// Sets the color of the pixel at the specified coordinate.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel.</param>
		/// <param name="y">The y-coordinate of the pixel.</param>
		/// <param name="color">Color value to which the pixel is set.</param>
		public void SetPixel(int x, int y, Color color)
		{
			if (m_rbuf.IsInbox(x, y) == true)
			{
				m_span.HorizontalLine(m_rbuf.Row(y), x, 1, color);
			}
		}

		/// <summary>
		/// Gets the color value of the pixel at the specified coordinate.
		/// </summary>
		/// <param name="x">The x-coordinate of the pixel.</param>
		/// <param name="y">The y-coordinate of the pixel.</param>
		/// <returns>The color value of the pixel.</returns>
		public Color GetPixel(int x, int y)
		{
			if (m_rbuf.IsInbox(x, y) == true)
			{
				return m_span.Get(m_rbuf.Row(y), x);
			}

			return Color.Black;
		}

		/// <summary>
		/// Renders the specified scanline in the specified color into the
		/// rendering buffer.
		/// </summary>
		/// <param name="scanline">The scanline to render.</param>
		/// <param name="color">The color to render the scanline.</param>
		public void Render(Scanline scanline, Color color)
		{
			if (scanline.Y < 0 || scanline.Y >= m_rbuf.Height)
			{
				return;
			}

			int base_x = scanline.BaseX;
			int y = scanline.Y;
			RowIterator row = m_rbuf.Row(y);
			Scanline.Iterator span = new Scanline.Iterator(scanline);
			for (int num_spans = scanline.SpanCount; num_spans > 0; num_spans--)
			{
				int x = span.Next() + base_x;
				CoverageIterator covers = span.Covers();
				int num_pix = span.PixelCount();
				if (x < 0)
				{
					num_pix += x;
					if (num_pix <= 0)
					{
						continue;
					}

					covers -= x;
					x = 0;
				}

				if (x + num_pix >= m_rbuf.Width)
				{
					num_pix = m_rbuf.Width - x;
					if (num_pix <= 0)
					{
						continue;
					}
				}

				m_span.Render(row, x, num_pix, covers, color);
			}
		}
	}
}
