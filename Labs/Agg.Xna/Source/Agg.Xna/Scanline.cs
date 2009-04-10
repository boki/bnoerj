// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna
{
	using System;

	/// <summary>
	/// The Scanline class is used to transfer data from the Outline class (or
	/// a similar one) to the rendering buffer.
	/// </summary>
	/// <remarks>
	/// It's organized very simple. The class stores information of horizontal
	/// spans to render it into a pixel-map buffer. Each span has initial X,
	/// length, and an array of bytes that determine the alpha-values for each
	/// pixel. So, the restriction of using this class is 256 levels of Anti-
	/// Aliasing, which is quite enough for any practical purpose.
	/// Before using this class you should know the minimal and maximal pixel
	/// coordinates of your scanline. The protocol of using is:
	/// 1. reset(min_x, max_x)
	/// 2. add_cell() / add_span() - accumulate scanline. You pass Y-coordinate 
	///    into these functions in order to make scanline know the last Y. Before 
	///    calling add_cell() / add_span() you should check with method is_ready(y)
	///    if the last Y has changed. It also checks if the scanline is not empty. 
	///    When forming one scanline the next X coordinate must be always greater
	///    than the last stored one, i.e. it works only with ordered coordinates.
	/// 3. If the current scanline is_ready() you should render it and then call 
	///    reset_spans() before adding new cells/spans.
	///    
	/// Rendering:
	/// Scanline provides an iterator class that allows you to extract the
	/// spans and the cover values for each pixel. Be aware that clipping has
	/// not been done yet, so you should perform it yourself.
	/// Use Scanline.Iterator to render spans:
	///-------------------------------------------------------------------------
	///
	/// int base_x = sl.base_x();          // base X. Should be added to the span's X
	///                                    // "sl" is a const reference to the 
	///                                    // scanline passed in.
	///
	/// int y = sl.y();                    // Y-coordinate of the scanline
	///
	/// ************************************
	/// ...Perform vertical clipping here...
	/// ************************************
	///
	/// scanline::iterator span(sl);
	/// 
	/// byte* row = m_rbuf->row(y); // The the address of the beginning 
	///                                      // of the current row
	/// 
	/// unsigned num_spans = sl.num_spans(); // Number of spans. It's guaranteed that
	///                                      // num_spans is always greater than 0.
	///
	/// do
	/// {
	///     int x = span.next() + base_x;        // The beginning X of the span
	///
	///     const int8u covers* = span.covers(); // The array of the cover values
	///
	///     int num_pix = span.num_pix();        // Number of pixels of the span.
	///                                          // Always greater than 0, still we
	///                                          // shoud use "int" instead of 
	///                                          // "unsigned" because it's more
	///                                          // convenient for clipping
	///
	///     **************************************
	///     ...Perform horizontal clipping here...
	///     ...you have x, covers, and pix_count..
	///     **************************************
	///
	///     byte* dst = row + x;  // Calculate the start address of the row.
	///                                    // In this case we assume a simple 
	///                                    // grayscale image 1-byte per pixel.
	///     do
	///     {
	///         *dst++ = *covers++;        // Hypotetical rendering. 
	///     }
	///     while(--num_pix);
	/// } 
	/// while(--num_spans);  // num_spans cannot be 0, so this loop is quite safe
	///------------------------------------------------------------------------
	///
	/// The question is: why should we accumulate the whole scanline when we
	/// could render just separate spans when they're ready?
	/// That's because using the scaline is in general faster. When is consists 
	/// of more than one span the conditions for the processor cash system are
	/// better, because switching between two different areas of memory (that
	/// can be large ones) occures less frequently.
	/// </remarks>
	public unsafe class Scanline
	{
		public const int AaShift = 8;

		/// <summary>
		/// The iterator class.
		/// </summary>
		public unsafe class Iterator
		{
			CoverageIterator m_covers;
			ushort[] m_counts;
			int m_curCountIndex;
			int[] m_start_indeces;
			int m_curStartIndex;

			/// <summary>
			/// Initializes a new instance of the Iterator class using the
			/// specified scanline.
			/// </summary>
			/// <param name="scanline">The scanline.</param>
			public Iterator(Scanline scanline)
			{
				m_covers = new CoverageIterator(scanline.m_covers);
				m_counts = scanline.m_counts;
				m_start_indeces = scanline.m_start_indeces;
			}

			public int Next()
			{
				//++m_cur_count;
				m_curCountIndex++;
				//++m_cur_start_ptr;
				m_curStartIndex++;
				// REVIEW: return (int)(*m_cur_start_ptr - m_covers);
				return m_start_indeces[m_curStartIndex];
			}

			public int PixelCount()
			{
				return (int)m_counts[m_curCountIndex];
			}

			public CoverageIterator Covers()
			{
				// REVIEW: return *m_cur_start_ptr;
				m_covers.Reset(m_start_indeces[m_curStartIndex]);
				return m_covers;
			}
		}

		int m_min_x;
		int m_max_len;
		int m_dx;
		int m_dy;
		int m_last_x;
		int m_last_y;

		byte[] m_covers;
		int[] m_start_indeces;
		ushort[] m_counts;

		int m_num_spans;
		int m_cur_start_index;
		int m_cur_count;

		/// <summary>
		/// Initializes a new instance of the Scanline class.
		/// </summary>
		public Scanline()
		{
			m_last_x = 0x7FFF;
			m_last_y = 0x7FFF;
		}

		/// <summary>
		/// Gets the base x-coordinate of the scanline.
		/// </summary>
		public int BaseX
		{
			get { return m_min_x + m_dx; }
		}

		/// <summary>
		/// Gets the y-coordinate of the scanline.
		/// </summary>
		public int Y
		{
			get { return m_last_y + m_dy; }
		}

		/// <summary>
		/// Gets the number fo spans in the scanline.
		/// </summary>
		public int SpanCount
		{
			get { return m_num_spans; }
		}

		/// <summary>
		/// Retrieves a value indicating whether the scanline is ready for the
		/// specified y-coordinate.
		/// </summary>
		/// <param name="y">The x-coordinate.</param>
		/// <returns>true if the scanline is ready; false otherwise.</returns>
		public bool IsReady(int y)
		{
			return m_num_spans > 0 && (y ^ m_last_y) != 0;
		}

		/// <summary>
		/// Resets the instance of the Scanline class to the specified minimal
		/// and maximal x-coordinates and dx/dy values.
		/// </summary>
		/// <param name="min_x">The minimal x-coordinate.</param>
		/// <param name="max_x">The maximal x-coordinate.</param>
		/// <param name="dx">The dx.</param>
		/// <param name="dy">The dy.</param>
		public void Reset(int min_x, int max_x, int dx, int dy)
		{
			int max_len = max_x - min_x + 2;
			if (max_len > m_max_len)
			{
				m_covers = new byte[max_len];
				m_start_indeces = new int[max_len];
				m_counts = new ushort[max_len];
				m_max_len = max_len;
			}

			m_dx = dx;
			m_dy = dy;
			m_last_x = 0x7FFF;
			m_last_y = 0x7FFF;
			m_min_x = min_x;
			// REVIEW: m_cur_count = m_counts;
			m_cur_count = 0;
			// REVIEW: m_cur_start_ptr = m_start_ptrs;
			m_cur_start_index = 0;
			m_num_spans = 0;
		}

		/// <summary>
		/// Resets the scanlines spans.
		/// </summary>
		public void ResetSpans()
		{
			m_last_x = 0x7FFF;
			m_last_y = 0x7FFF;
			// REVIEW: m_cur_count = m_counts;
			m_cur_count = 0;
			// REVIEW: m_cur_start_ptr = m_start_ptrs;
			m_cur_start_index = 0;
			m_num_spans = 0;
		}

		/// <summary>
		/// Adds a span at the specified coordinate using the specified number
		/// and coverage.
		/// </summary>
		/// <param name="x">The x-coordinate of the span.</param>
		/// <param name="y">The y-coordinate of the span.</param>
		/// <param name="length">The length of the span.</param>
		/// <param name="cover">The coverage of the span.</param>
		public void AddSpan(int x, int y, int length, int cover)
		{
#if DEBUG
			if (x < m_min_x)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("x < m_min_x in scanline.add_span(): {0} < {1}", x, m_min_x));
				return;
			}
			else if (x - m_min_x >= m_max_len)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("x > m_max_x in scanline.add_span(): {0} > {1}", x - m_min_x, m_max_len));
				return;
			}
#endif
			x -= m_min_x;

			// REVIEW: memset(m_covers + x, cover, num);
			int len = x + length;
			for (int i = x; i < len; i++)
			{
				m_covers[i] = (byte)cover;
			}

			if (x == m_last_x + 1)
			{
				m_counts[m_cur_count] += (ushort)length;
			}
			else
			{
				m_counts[++m_cur_count] = (ushort)length;
				m_start_indeces[++m_cur_start_index] = x;
				m_num_spans++;
			}

			m_last_x = x + (length - 1);
			m_last_y = y;
		}

		/// <summary>
		/// Adds a cell to the scanline at the specified coordinate using the
		/// specified coverage.
		/// </summary>
		/// <param name="x">The x-coordinate of the cell.</param>
		/// <param name="y">The y-coordinate of the cell.</param>
		/// <param name="cover">The coverage of the cell.</param>
		public void AddCell(int x, int y, int cover)
		{
#if DEBUG
			if (x < m_min_x)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("x < m_min_x in scanline.add_cell(): {0} < {1}", x, m_min_x));
				return;
			}
			else if (x - m_min_x >= m_max_len)
			{
				System.Diagnostics.Debug.WriteLine(String.Format("x > m_max_x in scanline.add_cell(): {0} > {1}", x - m_min_x, m_max_len));
				return;
			}
#endif
			x -= m_min_x;
			m_covers[x] = (byte)cover;
			if (x == m_last_x + 1)
			{
				m_counts[m_cur_count]++;
			}
			else
			{
				m_counts[++m_cur_count] = 1;
				m_start_indeces[++m_cur_start_index] = x;
				m_num_spans++;
			}

			m_last_x = x;
			m_last_y = y;
		}
	}
}
