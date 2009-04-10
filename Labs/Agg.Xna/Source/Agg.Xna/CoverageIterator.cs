// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna
{
	/// <summary>
	/// The CoverageIterator class.
	/// </summary>
	public class CoverageIterator
	{
		byte[] covers;
		int index;

		/// <summary>
		/// Initializes a new instance of the CoverageIterator class with the
		/// specified coverage values.
		/// </summary>
		/// <param name="covers">The coverage values.</param>
		internal CoverageIterator(byte[] covers)
		{
			this.covers = covers;
			this.index = 0;
		}

		/// <summary>
		/// Gets the current coverage value.
		/// </summary>
		public byte Current
		{
			get { return covers[index]; }
		}

		/// <summary>
		/// Sets the iterator to the specified position.
		/// </summary>
		/// <param name="index">The new position of the iterator.</param>
		internal void Reset(int index)
		{
			this.index = index;
		}

		/// <summary>
		/// Adds the specified offset to the iterators position.
		/// </summary>
		/// <param name="self">The CoverageIterator instace.</param>
		/// <param name="offset">The offset to add.</param>
		/// <returns>The CoverageIterator instance.</returns>
		public static CoverageIterator operator +(CoverageIterator self, int offset)
		{
			self.index += offset;
			return self;
		}

		/// <summary>
		/// Subtracts the specified offset from the iterators position.
		/// </summary>
		/// <param name="self">The CoverageIterator instace.</param>
		/// <param name="offset">The offset to add.</param>
		/// <returns>The CoverageIterator instance.</returns>
		public static CoverageIterator operator -(CoverageIterator self, int offset)
		{
			self.index -= offset;
			return self;
		}

		/// <summary>
		/// Advances the iterator to the next element.
		/// </summary>
		/// <returns>true if the iterator was successfully advanced to the next element; false if the iterator has passed the end of the collection.</returns>
		public bool MoveNext()
		{
			index++;
			return index < covers.Length;
		}
	}
}
