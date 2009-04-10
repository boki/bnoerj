// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna
{
	using System;
	using Microsoft.Xna.Framework.Graphics;

	/// <summary>
	/// The Rasterizer class.
	/// </summary>
	public unsafe class Rasterizer
	{
		const int AaShift = Scanline.AaShift;
		const int AaNum = 1 << AaShift;
		const int AaMask = AaNum - 1;
		const int Aa2Num = AaNum * 2;
		const int Aa2Mask = Aa2Num - 1;

		/// <summary>
		/// The default gamma value lookup table.
		/// </summary>
		static byte[] defaultGamma = new byte[]
		{
			  0,  0,  1,  1,  2,  2,  3,  4,  4,  5,  5,  6,  7,  7,  8,  8,
			  9, 10, 10, 11, 11, 12, 13, 13, 14, 14, 15, 16, 16, 17, 18, 18,
			 19, 19, 20, 21, 21, 22, 22, 23, 24, 24, 25, 25, 26, 27, 27, 28,
			 29, 29, 30, 30, 31, 32, 32, 33, 34, 34, 35, 36, 36, 37, 37, 38,
			 39, 39, 40, 41, 41, 42, 43, 43, 44, 45, 45, 46, 47, 47, 48, 49,
			 49, 50, 51, 51, 52, 53, 53, 54, 55, 55, 56, 57, 57, 58, 59, 60,
			 60, 61, 62, 62, 63, 64, 65, 65, 66, 67, 68, 68, 69, 70, 71, 71,
			 72, 73, 74, 74, 75, 76, 77, 78, 78, 79, 80, 81, 82, 83, 83, 84,
			 85, 86, 87, 88, 89, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99,
			100,101,101,102,103,104,105,106,107,108,109,110,111,112,114,115,
			116,117,118,119,120,121,122,123,124,126,127,128,129,130,131,132,
			134,135,136,137,139,140,141,142,144,145,146,147,149,150,151,153,
			154,155,157,158,159,161,162,164,165,166,168,169,171,172,174,175,
			177,178,180,181,183,184,186,188,189,191,192,194,195,197,199,200,
			202,204,205,207,209,210,212,214,215,217,219,220,222,224,225,227,
			229,230,232,234,236,237,239,241,242,244,246,248,249,251,253,255
		};

		Outline m_outline;
		Scanline m_scanline;
		FillingRule m_filling_rule;
		byte[] m_gamma;

		/// <summary>
		/// Initializes a new instance of the Rasterizer class.
		/// </summary>
		public Rasterizer()
		{
			m_filling_rule = FillingRule.FillNonZero;

			m_outline = new Outline();
			m_scanline = new Scanline();

			m_gamma = new byte[256];
			Array.Copy(defaultGamma, m_gamma, m_gamma.Length);
		}

		/// <summary>
		/// Gets or sets the filling rule.
		/// </summary>
		public FillingRule FillRule
		{
			get { return m_filling_rule; }
			set { m_filling_rule = value; }
		}

		/// <summary>
		/// Gets the minimum x-coordinate.
		/// </summary>
		public int MinX
		{
			get { return m_outline.MinX; }
		}

		/// <summary>
		/// Gets the minimum y-coordinate.
		/// </summary>
		public int MinY
		{
			get { return m_outline.MinY; }
		}

		/// <summary>
		/// Gets the maximum x-coordinate.
		/// </summary>
		public int MaxX
		{
			get { return m_outline.MaxX; }
		}

		/// <summary>
		/// Gets the maximum y-coordinate.
		/// </summary>
		public int MaxY
		{
			get { return m_outline.MaxY; }
		}

		/// <summary>
		/// Resets the rasterizer.
		/// </summary>
		public void Reset()
		{
			m_outline.Reset();
		}

		/// <summary>
		/// Sets the value used for gamma correction.
		/// </summary>
		/// <param name="gamma">The new gamma value.</param>
		public void SetGamma(double gamma)
		{
			for (int i = 0; i < 256; i++)
			{
				m_gamma[i] = (byte)(Math.Pow((double)i / 255.0, gamma) * 255.0);
			}
		}

		/// <summary>
		/// Sets the value used for gamma correction.
		/// </summary>
		/// <param name="gamma">The new gamma value.</param>
		public void SetGamma(byte[] gamma)
		{
			Array.Copy(gamma, m_gamma, m_gamma.Length);
		}

		/// <summary>
		/// Updates the current position to the specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the new position, in PolyCoordinate units.</param>
		/// <param name="y">The y-coordinate of the new position, in PolyCoordinate pixel units.</param>
		public void MoveTo(int x, int y)
		{
			m_outline.MoveTo(x, y);
		}

		/// <summary>
		/// Draws a line from the current position up to, but not including, the
		/// specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the end point, in PolyCoordinate units.</param>
		/// <param name="y">The y-coordinate of the end point, in PolyCoordinate units.</param>
		public void LineTo(int x, int y)
		{
			m_outline.LineTo(x, y);
		}

		/// <summary>
		/// Updates the current position to the specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the new position, in pixel units.</param>
		/// <param name="y">The y-coordinate of the new position, in pixel units.</param>
		public void MoveToD(double x, double y)
		{
			m_outline.MoveTo(PolyBase.PolyCoordinate(x), PolyBase.PolyCoordinate(y));
		}

		/// <summary>
		/// Draws a line from the current position up to, but not including, the
		/// specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the end point, in pixel units.</param>
		/// <param name="y">The y-coordinate of the end point, in pixel units.</param>
		public void LineToD(double x, double y)
		{
			m_outline.LineTo(PolyBase.PolyCoordinate(x), PolyBase.PolyCoordinate(y));
		}

		/// <summary>
		/// Renders the current outline using the specified renderer and color.
		/// </summary>
		/// <typeparam name="Renderer">The type of the renderer.</typeparam>
		/// <param name="renderer">The renderer.</param>
		/// <param name="color">The color.</param>
		public void Render<Renderer>(Renderer renderer, Color color)
			where Renderer : IRenderer
		{
			Render(renderer, color, 0, 0);
		}

		/// <summary>
		/// Renders the current outline using the specified renderer and color.
		/// </summary>
		/// <typeparam name="Renderer">The type of the renderer.</typeparam>
		/// <param name="renderer">The renderer.</param>
		/// <param name="color">The color.</param>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		public void Render<Renderer>(Renderer renderer, Color color, int dx, int dy)
			where Renderer : IRenderer
		{
			if (m_outline.CellCount == 0)
			{
				return;
			}

			Cell[] cells = m_outline.GetCells();
			int cellCount = m_outline.CellCount;

			m_scanline.Reset(m_outline.MinX, m_outline.MaxX, dx, dy);

			int cover = 0;
			fixed (Cell* cellsRef = &cells[0])
			{
				Cell* cells_ptr = cellsRef;
				Cell* cur_cell = cells_ptr++;
				for (; ; )
				{
					Cell* start_cell = cur_cell;

					int coord = cur_cell->PackedCoord;
					int x = cur_cell->X;
					int y = cur_cell->Y;

					int area = start_cell->Area;
					cover += start_cell->Cover;

					// accumulate all start cells
					while ((cur_cell = cells_ptr++) != null && cellCount-- > 0)
					{
						if (cur_cell->PackedCoord != coord)
							break;

						area += cur_cell->Area;
						cover += cur_cell->Cover;
					}

					int alpha;
					if (area > 0)
					{
						alpha = CalculateAlpha((cover << (PolyBase.Shift + 1)) - area);
						if (alpha > 0)
						{
							if (m_scanline.IsReady(y) == true)
							{
								renderer.Render(m_scanline, color);
								m_scanline.ResetSpans();
							}

							m_scanline.AddCell(x, y, m_gamma[alpha]);
						}

						x++;
					}

					if (cur_cell == null || cellCount <= 0)
					{
						break;
					}

					if (cur_cell->X > x)
					{
						alpha = CalculateAlpha(cover << (PolyBase.Shift + 1));
						if (alpha > 0)
						{
							if (m_scanline.IsReady(y) == true)
							{
								renderer.Render(m_scanline, color);
								m_scanline.ResetSpans();
							}

							m_scanline.AddSpan(x, y, cur_cell->X - x, m_gamma[alpha]);
						}
					}
				}
			}

			if (m_scanline.SpanCount > 0)
			{
				renderer.Render(m_scanline, color);
			}
		}

		/// <summary>
		/// Retrieves a value indicating whether the pixel at the specified
		/// coordinate is covered.
		/// </summary>
		/// <param name="tx">The x-coordinate to test.</param>
		/// <param name="ty">The y-coordinate to test.</param>
		/// <returns>true if the pixel at the specified coordinate is covered: false otherwise.</returns>
		public bool HitTest(int tx, int ty)
		{
			if (m_outline.CellCount == 0)
			{
				return false;
			}

			Cell[] cells = m_outline.GetCells();
			int cellCount = m_outline.CellCount;

			int x, y;
			int cover;
			int alpha;
			int area;

			cover = 0;
			fixed (Cell* cellsRef = &cells[0])
			{
				Cell* cells_ptr = cellsRef;
				Cell* cur_cell = cells_ptr++;
				for (; ; )
				{
					Cell* start_cell = cur_cell;

					int coord = cur_cell->PackedCoord;
					x = cur_cell->X;
					y = cur_cell->Y;

					if (y > ty)
					{
						return false;
					}

					area = start_cell->Area;
					cover += start_cell->Cover;

					while ((cur_cell = cells_ptr++) != null)
					{
						if (cur_cell->PackedCoord != coord)
						{
							break;
						}

						area += cur_cell->Area;
						cover += cur_cell->Cover;
					}

					if (area > 0)
					{
						alpha = CalculateAlpha((cover << (PolyBase.Shift + 1)) - area);
						if (alpha > 0)
						{
							if (tx == x && ty == y)
							{
								return true;
							}
						}

						x++;
					}

					if (cur_cell == null)
					{
						break;
					}

					if (cur_cell->X > x)
					{
						alpha = CalculateAlpha(cover << (PolyBase.Shift + 1));
						if (alpha > 0)
						{
							if (ty == y && tx >= x && tx <= cur_cell->X)
							{
								return true;
							}
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Calculates the alpha value for the area.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <returns>The alpha value.</returns>
		int CalculateAlpha(int area)
		{
			int cover = area >> (PolyBase.Shift * 2 + 1 - AaShift);
			if (cover < 0)
			{
				cover = -cover;
			}

			if (m_filling_rule == FillingRule.FillEvenOdd)
			{
				cover &= Aa2Mask;
				if (cover > AaNum)
				{
					cover = Aa2Num - cover;
				}
			}

			if (cover > AaMask)
			{
				cover = AaMask;
			}

			return cover;
		}
	}
}
