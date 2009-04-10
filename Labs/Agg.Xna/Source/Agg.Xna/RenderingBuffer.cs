// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

namespace Agg.Xna
{
	/// <summary>
	/// Represents a rendering buffer.
	/// </summary>
	/// <remarks>
	/// This class does not know anything about memory organizations, all it
	/// does it keeps an array of pointers to each pixel row. The general rules
	/// of rendering are as follows.
	/// 
	/// 1. Allocate or create somehow a rendering buffer itself. Since the
	///    library does not depend on any particular platform or architecture
	///    it was decided that it's your responsibility to create and destroy
	///    rendering buffers properly. You can use any available mechanism to
	///    create it - you can use a system API function, simple memory
	///    allocation, or even statically defined array. You also should know
	///    the memory organization (or possible variants) in your system. For
	///    example, there's an R,G,B or B,G,R organizations with one byte per
	///    component (three byter per pixel) is used very often. So, if you
	///    intend to use class render_bgr24, for example, you should allocate
	///    at least width*height*3 bytes of memory.
	///
	/// 2. Create a rendering_buffer object and then call method Attach(). It
	///    requires a pointer to the buffer itself, width and height of the
	///    buffer in pixels, and the length of the row in bytes. All these
	///    values must properly correspond to the memory organization. The
	///    argument stride is used because in reality the row length in bytes
	///    does not obligatory correspond with the width of the image in pixels,
	///    i.e. it cannot be simply calculated as width_in_pixels * bytes_per_pixel.
	///    For example, it must be aligned to 4 bytes in Windows bitmaps.
	///    Besides, the value of stride can be negative - it depends on the
	///    order of displaying the rendering buffer - from top to bottom or
	///    from bottom to top. In other words, if stride > 0 the pointers to
	///    each row will start from the beginning of the buffer and increase.
	///    If it < 0, the pointers start from the end of the buffer and decrease.
	///    It gives you an additional degree of freedom. The Method Attach() can
	///    be called more than once. The execution time of it is very little,
	///    still it allocates memory of heigh * sizeof(char*) bytes and has a
	///    loop while(height--) {...}, so it's unreasonable to call it every
	///    time before drawing any single pixel :-)
	///
	/// 3. Create an object (or a number of objects) of a rendering class, such
	///    as RendererBgr24Solid, RendererBgr24Image and so on. These classes
	///    require a pointer to the renderer_buffer object, but they do not perform
	///    any considerable operations except storing this pointer. So, rendering
	///    objects can be created on demand almost any time. These objects know 
	///    about concrete memory organization (this knowledge is hardcoded), so 
	///    actually, the memory you allocated or created in clause 1 should 
	///    actually be in correspondence to the needs of the rendering class.
	///  
	/// 4. Render your image using rendering classes, for example, rasterizer
	///  
	/// 5. Display the result, or store it, or whatever. It's also your 
	///    responsibility and depends on the platform.
	/// </remarks>
	public unsafe class RenderingBuffer
	{
		/// <summary>
		/// The rendering buffer.
		/// </summary>
		byte[] m_buf;

		/// <summary>
		/// Indices to each row of the buffer.
		/// </summary>
		int[] m_rows;

		/// <summary>
		/// The width in pixels.
		/// </summary>
		int m_width;

		/// <summary>
		/// The height in pixels.
		/// </summary>
		int m_height;

		/// <summary>
		/// Number of bytes per row. Can be less than zero.
		/// </summary>
		int m_stride;

		/// <summary>
		/// The maximal current height.
		/// </summary>
		int m_max_height;

		/// <summary>
		/// The row iterator instance.
		/// </summary>
		RowIterator rowIterator;

		/// <summary>
		/// Initializes a new instance of the RenderingBuffer class using the
		/// specified buffer, width, height and stride.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="width">The width of the buffer, in pixels.</param>
		/// <param name="height">The height of the buffer, in pixels.</param>
		/// <param name="stride">The stride of the buffer, in bytes.</param>
		public RenderingBuffer(byte[] buffer, int width, int height, int stride)
		{
			Attach(buffer, width, height, stride);
		}

		/// <summary>
		/// Gets the buffer.
		/// </summary>
		public byte[] Buffer
		{
			get { return m_buf; }
		}

		/// <summary>
		/// Gets the width of the buffer, in pixels.
		/// </summary>
		public int Width
		{
			get { return m_width; }
		}

		/// <summary>
		/// Gets the height of the buffer, in pixels.
		/// </summary>
		public int Height
		{
			get { return m_height; }
		}

		/// <summary>
		/// Gets the stride of the buffer, in bytes. This value can be less
		/// than zero for bottom-to-top buffers.
		/// </summary>
		public int Stride
		{
			get { return m_stride; }
		}

		/// <summary>
		/// Gets the absolute stride of the buffer, in bytes.
		/// </summary>
		public int AbsoluteStride
		{
			get { return m_stride < 0 ? -m_stride : m_stride; }
		}

		/// <summary>
		/// Gets the row iterator for the specified row.
		/// </summary>
		/// <param name="y">The x-coordinate of the row.</param>
		/// <returns>The row iterator.</returns>
		public RowIterator Row(int y)
		{
			rowIterator.Initialize(m_rows[y], Width);
			return rowIterator;//new RowIterator(m_buf, m_rows[y], Width);
		}

		/// <summary>
		/// Retrieves a value indicating whether the specified coordinate is
		/// inside the buffer.
		/// </summary>
		/// <param name="x">The x-coordiante to test.</param>
		/// <param name="y">The y-coordinate to test.</param>
		/// <returns>ture if the specified coordinate is within the buffer; false otherwise.</returns>
		public bool IsInbox(int x, int y)
		{
			return x >= 0 && y >= 0 && x < m_width && y < m_height;
		}

		/// <summary>
		/// Attaches the instance to the specified buffer.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="width">The width of the buffer, in pixels.</param>
		/// <param name="height">The height of the buffer, in pixels.</param>
		/// <param name="stride">The stride of the buffer, in bytes.</param>
		public void Attach(byte[] buffer, int width, int height, int stride)
		{
			m_buf = buffer;
			m_width = width;
			m_height = height;
			m_stride = stride;
			if (height > m_max_height)
			{
				m_max_height = height;
				m_rows = new int[m_max_height];
			}

			int rowIndex = 0;
			if (stride < 0)
			{
				rowIndex = m_buf.Length - (height - 1) * stride;
			}

			for (int y = 0; y < height; y++)
			{
				m_rows[y] = rowIndex;
				rowIndex += stride;
			}

			rowIterator = new RowIterator(m_buf, m_rows[0], Width);
		}
	}
}
