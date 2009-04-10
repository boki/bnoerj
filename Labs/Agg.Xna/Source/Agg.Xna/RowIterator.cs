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
	/// The RowIterator class.
	/// </summary>
	public class RowIterator
	{
		/// <summary>
		/// The index of the start of the row.
		/// </summary>
		int baseIndex;

		/// <summary>
		/// The current index into the row.
		/// </summary>
		int curIndex;

		/// <summary>
		/// The rendering buffer.
		/// </summary>
		byte[] buffer;

		/// <summary>
		/// The width of the row.
		/// </summary>
		int width;

		/// <summary>
		/// Initialies a new instance of the RowIterator class using the
		/// specified rendering buffer, rows start indices and row.
		/// </summary>
		/// <param name="buffer">The rendering buffer.</param>
		/// <param name="baseIndex">The row start index.</param>
		/// <param name="width">The width of the row.</param>
		internal RowIterator(byte[] buffer, int baseIndex, int width)
		{
			this.buffer = buffer;
			this.baseIndex = baseIndex;
			this.curIndex = baseIndex;
			this.width = width;
		}

		/// <summary>
		/// Gets the value of the row at the specified offset.
		/// </summary>
		/// <param name="offset">The offset from the row start, in bytes.</param>
		/// <returns>The value at the specified offset.</returns>
		public byte this[int offset]
		{
			get { return buffer[curIndex + offset]; }
			set { buffer[curIndex + offset] = value; }
		}

		/// <summary>
		/// Initializes the instance of the RowIterator using the specified
		/// row start indices and row.
		/// </summary>
		/// <param name="baseIndex">The row start index.</param>
		/// <param name="width">The width of the row.</param>
		internal void Initialize(int baseIndex, int width)
		{
			this.baseIndex = baseIndex;
			this.curIndex = baseIndex;
			this.width = width;
		}

		/// <summary>
		/// Updates the current position by the specified offset.
		/// </summary>
		/// <param name="offset">The offset, in bytes, to move the current position.</param>
		/// <returns>true if the iterator was successfully advanced to the next element; false if the iterator has passed the end of the row.</returns>
		public bool Move(int offset)
		{
#if DEBUG
			// FIXME: check lower bound, too
			if (baseIndex + offset >= buffer.Length)
			{
				throw new ArgumentException(String.Format("{0} + {1} ({2}) < {3}", baseIndex, offset, baseIndex + offset, buffer.Length), "offset");
			}
#endif
			curIndex = baseIndex + offset;
			return curIndex < baseIndex + width;
		}

		/// <summary>
		/// Advances the iterator to the next element.
		/// </summary>
		/// <returns>true if the iterator was successfully advanced to the next element; false if the iterator has passed the end of the row.</returns>
		public bool MoveNext()
		{
			curIndex++;
			return curIndex < baseIndex + width;
		}
	}
}
