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

	/// <summary>
	/// The Outline class.
	/// </summary>
	public unsafe class Outline
	{
		/// <summary>
		/// Outline flags.
		/// </summary>
		[Flags]
		enum Flags
		{
			/// <summary>
			/// None.
			/// </summary>
			None = 0x00,

			/// <summary>
			/// The outline is open.
			/// </summary>
			Open = 0x01,

			/// <summary>
			/// The outline is unsorted.
			/// </summary>
			SortRequired = 0x02
		}

		const int CellBlockShift = 12;
		const int CellBlockSize = 1 << CellBlockShift;
		const int CellBlockMask = CellBlockSize - 1;
		const int CellBlockPool = 256;
		const int CellBlockLimit = 1024;

		int m_num_blocks;
		int m_max_blocks;
		int m_cur_block;
		int m_num_cells;
		Cell[][] m_cells;
		//cell* m_cur_cell_ptr;
		int m_cur_cell_ptr;
		Cell[] m_sorted_cells;
		int m_sorted_size;
		Cell m_cur_cell;
		int m_cur_x;
		int m_cur_y;
		int m_close_x;
		int m_close_y;
		int m_min_x;
		int m_min_y;
		int m_max_x;
		int m_max_y;
		Flags m_flags;

		/// <summary>
		/// Initializes a new instance of the Outline class.
		/// </summary>
		public Outline()
		{
			m_min_x = PolyBase.MaxValue;
			m_min_y = PolyBase.MaxValue;
			m_max_x = PolyBase.MinValue;
			m_max_y = PolyBase.MinValue;

			m_flags = Flags.SortRequired;

			m_cur_cell.Set(0x7FFF, 0x7FFF, 0, 0);
		}

		/// <summary>
		/// Gets the minimum x-coordinate of the outline.
		/// </summary>
		public int MinX
		{
			get { return m_min_x; }
		}

		/// <summary>
		/// Gets the minimum y-coordinate of the outline.
		/// </summary>
		public int MinY
		{
			get { return m_min_y; }
		}

		/// <summary>
		/// Gets the maximum x-coordinate of the outline.
		/// </summary>
		public int MaxX
		{
			get { return m_max_x; }
		}

		/// <summary>
		/// Gets the maximum y-coordinate of the outline.
		/// </summary>
		public int MaxY
		{
			get { return m_max_y; }
		}

		/// <summary>
		/// Gets the number of cells in the outline.
		/// </summary>
		public int CellCount
		{
			get { return m_num_cells; }
		}

		/// <summary>
		/// Resets this instance.
		/// </summary>
		public void Reset()
		{
			m_num_cells = 0;
			m_cur_block = 0;
			m_cur_cell.Set(0x7FFF, 0x7FFF, 0, 0);

			m_min_x = PolyBase.MaxValue;
			m_min_y = PolyBase.MaxValue;
			m_max_x = PolyBase.MinValue;
			m_max_y = PolyBase.MinValue;

			m_flags |= Flags.SortRequired;
			m_flags &= ~Flags.Open;
		}

		/// <summary>
		/// Gets the sorted cells of this outline instance.
		/// </summary>
		/// <returns>An array of the outlines cells.</returns>
		public Cell[] GetCells()
		{
			if ((m_flags & Flags.Open) != Flags.None)
			{
				LineTo(m_close_x, m_close_y);
				m_flags &= ~Flags.Open;
			}

			// Perform sort only the first time.
			if ((m_flags & Flags.SortRequired) != Flags.None)
			{
				AddCurrentCell();
				if (m_num_cells == 0)
				{
					return null;
				}

				SortCells();
				m_flags &= ~Flags.SortRequired;
			}

			return m_sorted_cells;
		}

		/// <summary>
		/// Updates the current position to the specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the new position, in pixel units.</param>
		/// <param name="y">The y-coordinate of the new position, in pixel units.</param>
		public void MoveTo(int x, int y)
		{
			if ((m_flags & Flags.SortRequired) == Flags.None)
			{
				Reset();
			}

			if ((m_flags & Flags.Open) != Flags.None)
			{
				LineTo(m_close_x, m_close_y);
			}

			SetCurrentCell(x >> PolyBase.Shift, y >> PolyBase.Shift);

			m_close_x = x;
			m_close_y = y;

			m_cur_x = x;
			m_cur_y = y;
		}

		/// <summary>
		/// Draws a line from the current position up to, but not including, the
		/// specified point.
		/// </summary>
		/// <param name="x">The x-coordinate of the end point, in pixel units.</param>
		/// <param name="y">The y-coordinate of the end point, in pixel units.</param>
		public void LineTo(int x, int y)
		{
			if ((m_flags & Flags.SortRequired) != Flags.None && ((m_cur_x ^ x) | (m_cur_y ^ y)) != 0)
			{
				int c = m_cur_x >> PolyBase.Shift;
				if (c < m_min_x)
				{
					m_min_x = c;
				}

				++c;
				if (c > m_max_x)
				{
					m_max_x = c;
				}

				c = x >> PolyBase.Shift;
				if (c < m_min_x)
				{
					m_min_x = c;
				}

				++c;
				if (c > m_max_x)
				{
					m_max_x = c;
				}

				RenderLine(m_cur_x, m_cur_y, x, y);
				m_cur_x = x;
				m_cur_y = y;
				m_flags |= Flags.Open;
			}
		}

		/// <summary>
		/// Allocates a new cell block in the cell block buffer.
		/// </summary>
		void AllocateBlock()
		{
			if (m_cur_block >= m_num_blocks)
			{
				if (m_num_blocks >= m_max_blocks)
				{
					Array.Resize<Cell[]>(ref m_cells, m_max_blocks + CellBlockPool);
					m_max_blocks += CellBlockPool;
				}

				m_cells[m_num_blocks++] = new Cell[CellBlockSize];
			}

			// REVIEW: m_cur_cell_ptr = m_cells[m_cur_block++];
			m_cur_cell_ptr = 0;
			m_cur_block++;
		}

		/// <summary>
		/// Adds the current cell to the cell block buffer.
		/// </summary>
		void AddCurrentCell()
		{
			if ((m_cur_cell.Area | m_cur_cell.Cover) != 0)
			{
				if ((m_num_cells & CellBlockMask) == 0)
				{
					if (m_num_blocks >= CellBlockLimit)
					{
						return;
					}

					AllocateBlock();
				}

				// REVIEW: *m_cur_cell_ptr++ = m_cur_cell;
				m_cells[m_cur_block - 1][m_cur_cell_ptr++] = m_cur_cell;
				m_num_cells++;
			}
		}

		/// <summary>
		/// Sets the current cell to the specified coordinate.
		/// </summary>
		/// <param name="x">The x-coordinate of the new position, in pixel units.</param>
		/// <param name="y">The y-coordinate of the new position, in pixel units.</param>
		void SetCurrentCell(int x, int y)
		{
			if (m_cur_cell.PackedCoord != (y << 16) + x)
			{
				AddCurrentCell();
				m_cur_cell.Set(x, y, 0, 0);
			}
		}

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="ey"></param>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		void RenderScanline(int ey, int x1, int y1, int x2, int y2)
		{
			int ex1 = x1 >> PolyBase.Shift;
			int ex2 = x2 >> PolyBase.Shift;
			int fx1 = x1 & PolyBase.Mask;
			int fx2 = x2 & PolyBase.Mask;

			int delta, p, first, dx;
			int incr, lift, mod, rem;

			// trivial case. Happens often
			if (y1 == y2)
			{
				SetCurrentCell(ex2, ey);
				return;
			}

			// everything is located in a single cell. That is easy!
			if (ex1 == ex2)
			{
				delta = y2 - y1;
				m_cur_cell.AddCover(delta, (fx1 + fx2) * delta);
				return;
			}

			// ok, we'll have to render a run of adjacent cells on the same
			// scanline...
			p = (PolyBase.Size - fx1) * (y2 - y1);
			first = PolyBase.Size;
			incr = 1;

			dx = x2 - x1;

			if (dx < 0)
			{
				p = fx1 * (y2 - y1);
				first = 0;
				incr = -1;
				dx = -dx;
			}

			delta = p / dx;
			mod = p % dx;

			if (mod < 0)
			{
				delta--;
				mod += dx;
			}

			m_cur_cell.AddCover(delta, (fx1 + first) * delta);

			ex1 += incr;
			SetCurrentCell(ex1, ey);
			y1 += delta;

			if (ex1 != ex2)
			{
				p = PolyBase.Size * (y2 - y1 + delta);
				lift = p / dx;
				rem = p % dx;

				if (rem < 0)
				{
					lift--;
					rem += dx;
				}

				mod -= dx;

				while (ex1 != ex2)
				{
					delta = lift;
					mod += rem;
					if (mod >= 0)
					{
						mod -= dx;
						delta++;
					}

					m_cur_cell.AddCover(delta, PolyBase.Size * delta);
					y1 += delta;
					ex1 += incr;
					SetCurrentCell(ex1, ey);
				}
			}

			delta = y2 - y1;
			m_cur_cell.AddCover(delta, (fx2 + PolyBase.Size - first) * delta);
		}

		/// <summary>
		/// Renders a line.
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="y1"></param>
		/// <param name="x2"></param>
		/// <param name="y2"></param>
		void RenderLine(int x1, int y1, int x2, int y2)
		{
			int ey1 = y1 >> PolyBase.Shift;
			int ey2 = y2 >> PolyBase.Shift;
			int fy1 = y1 & PolyBase.Mask;
			int fy2 = y2 & PolyBase.Mask;

			int dx, dy, x_from, x_to;
			int p, rem, mod, lift, delta, first, incr;

			if (ey1 < m_min_y) m_min_y = ey1;
			if (ey1 + 1 > m_max_y) m_max_y = ey1 + 1;
			if (ey2 < m_min_y) m_min_y = ey2;
			if (ey2 + 1 > m_max_y) m_max_y = ey2 + 1;

			dx = x2 - x1;
			dy = y2 - y1;

			// everything is on a single scanline
			if (ey1 == ey2)
			{
				RenderScanline(ey1, x1, fy1, x2, fy2);
				return;
			}

			// Vertical line - we have to calculate start and end cells,
			// and then - the common values of the area and coverage for
			// all cells of the line. We know exactly there's only one 
			// cell, so, we don't have to call render_scanline().
			incr = 1;
			if (dx == 0)
			{
				int ex = x1 >> PolyBase.Shift;
				int two_fx = (x1 - (ex << PolyBase.Shift)) << 1;
				int area;

				first = PolyBase.Size;
				if (dy < 0)
				{
					first = 0;
					incr = -1;
				}

				x_from = x1;

				// render_scanline(ey1, x_from, fy1, x_from, first);
				delta = first - fy1;
				m_cur_cell.AddCover(delta, two_fx * delta);

				ey1 += incr;
				SetCurrentCell(ex, ey1);

				delta = first + first - PolyBase.Size;
				area = two_fx * delta;
				while (ey1 != ey2)
				{
					// render_scanline(ey1, x_from, poly_base_size - first, x_from, first);
					m_cur_cell.SetCover(delta, area);
					ey1 += incr;
					SetCurrentCell(ex, ey1);
				}

				// render_scanline(ey1, x_from, poly_base_size - first, x_from, fy2);
				delta = fy2 - PolyBase.Size + first;
				m_cur_cell.AddCover(delta, two_fx * delta);
				return;
			}

			// ok, we have to render several scanlines
			p = (PolyBase.Size - fy1) * dx;
			first = PolyBase.Size;

			if (dy < 0)
			{
				p = fy1 * dx;
				first = 0;
				incr = -1;
				dy = -dy;
			}

			delta = p / dy;
			mod = p % dy;

			if (mod < 0)
			{
				delta--;
				mod += dy;
			}

			x_from = x1 + delta;
			RenderScanline(ey1, x1, fy1, x_from, first);

			ey1 += incr;
			SetCurrentCell(x_from >> PolyBase.Shift, ey1);

			if (ey1 != ey2)
			{
				p = PolyBase.Size * dx;
				lift = p / dy;
				rem = p % dy;

				if (rem < 0)
				{
					lift--;
					rem += dy;
				}
				mod -= dy;

				while (ey1 != ey2)
				{
					delta = lift;
					mod += rem;
					if (mod >= 0)
					{
						mod -= dy;
						delta++;
					}

					x_to = x_from + delta;
					RenderScanline(ey1, x_from, PolyBase.Size - first, x_to, first);
					x_from = x_to;

					ey1 += incr;
					SetCurrentCell(x_from >> PolyBase.Shift, ey1);
				}
			}

			RenderScanline(ey1, x_from, PolyBase.Size - first, x2, fy2);
		}

		/// <summary>
		/// Sorts the cells based on their coordinates in clockwise order.
		/// </summary>
		void SortCells()
		{
			if (m_num_cells == 0)
			{
				return;
			}

			if (m_num_cells > m_sorted_size)
			{
				m_sorted_size = m_num_cells;
				m_sorted_cells = new Cell[m_num_cells];
			}

			int b;
			int dstIndex = 0;
			for (b = 0; b < m_num_cells >> CellBlockShift; b++)
			{
				Array.Copy(m_cells[b], 0, m_sorted_cells, dstIndex, CellBlockSize);
				dstIndex += CellBlockSize;
			}

			int len = (m_num_cells & CellBlockMask);
			Array.Copy(m_cells[b], 0, m_sorted_cells, dstIndex, len);

			// REVIEW: qsort_cells(m_sorted_cells, m_num_cells);
			Array.Sort(m_sorted_cells, 0, m_num_cells);
		}
	}
}
