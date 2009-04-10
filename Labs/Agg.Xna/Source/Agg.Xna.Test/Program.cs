// Copyright (C) 2009, Bjoern Graf <bjoern.graf@gmx.net>
// Copyright (C) 2002-2003 Maxim Shemanarev (McSeem)
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://bnoerj.codeplex.com/license.

namespace Agg.Xna.Test
{
	using System;
	using System.Drawing.Imaging;
	using Microsoft.Xna.Framework.Graphics;
	using Agg.Xna.Curves;
	using Agg.Xna.Paths;

	class Program
	{
		struct Point
		{
			public static Point Empty = new Point();

			public double X;
			public double Y;

			public Point(double x, double y)
			{
				X = x;
				Y = y;
			}

			public override string ToString()
			{
				return String.Format("{{{0},{1}}}", X, Y);
			}
		}

		const int width = 768;
		const int height = 576;

		static Random rng;

		static unsafe void Main(String[] args)
		{
			rng = new Random(123);

			// Allocate the framebuffer
			byte[] buf = new byte[width * height * 3];

			// Create the rendering buffer 
			RenderingBuffer rbuf = new RenderingBuffer(buf, width, height, width * 3);

			// Create the renderer and the rasterizer
			Renderer ren = new Renderer(rbuf, new SpanBgr24());
			Rasterizer ras = new Rasterizer();

			// Setup the rasterizer
			ras.SetGamma(1.3);
			ras.FillRule = FillingRule.FillNonZero;

			ren.Clear(new Color(255, 255, 255));
#if !TEST_CURVE
			double pieceWidth = 0.59 * width / 2;
			double pieceHeight = 0.59 * height / 2;
			double ox = rbuf.Width / 4;
			double oy = rbuf.Height / 3;
			double cx = rbuf.Width / 2;
			double cy = rbuf.Height / 2;
			Point[] cpts = new Point[] {
				new Point(-1, 0),
				new Point(1 - 1.0/8, 1.0/3),
				new Point(-1, -1.0/3),
				new Point(0, -1.0/3),

				new Point(0, -1.0/3),
				new Point(1, -1.0/3),
				new Point(-1 + 1.0/8, 1.0/3),
				new Point(1, 0),

				Point.Empty, Point.Empty, Point.Empty, Point.Empty,
				Point.Empty, Point.Empty, Point.Empty, Point.Empty,

				Point.Empty, Point.Empty, Point.Empty, Point.Empty,
				Point.Empty, Point.Empty, Point.Empty, Point.Empty,
			};

			int ci2 = 8;
			int ci3 = 16;
			double x;
			double y;
			AffineTransform trans =
				AffineTransform.CreateScale(1, 1.5) *
				AffineTransform.CreateTranslation(0, -1);
			AffineTransform rot =
				AffineTransform.CreateTranslation(0, -1) *
				AffineTransform.CreateScale(-1, 1) *
				AffineTransform.CreateRotation(-Math.PI / 2);
			AffineTransform rot2 =
				AffineTransform.CreateScale(1, 1.5) *
				AffineTransform.CreateTranslation(0, 1) *
				AffineTransform.CreateScale(1, -1) *
				AffineTransform.CreateRotation(-Math.PI);
			for (int ci = 0; ci < ci2; ci++)
			{
				double sx = cpts[ci].X;
				double sy = cpts[ci].Y;
				x = sx;
				y = sy;
				trans.Transform(ref x, ref y);
				cpts[ci].X = x;
				cpts[ci].Y = y;

				x = sx;
				y = sy;
				rot.Transform(ref x, ref y);
				cpts[ci2 + ci].X = x;
				cpts[ci2 + ci].Y = y;

				x = sx;
				y = sy;
				rot2.Transform(ref x, ref y);
				cpts[ci3 + ci].X = x;
				cpts[ci3 + ci].Y = y;
			}
			
			trans = new AffineTransform(
				pieceWidth, 0,
				0, pieceHeight,
				rbuf.Width / 2, rbuf.Height / 2);
			for (int ci = 0; ci < cpts.Length; ci++)
			{
				x = cpts[ci].X;
				y = cpts[ci].Y;
				trans.Transform(ref x, ref y);
				cpts[ci].X = x;
				cpts[ci].Y = y;
			}
			
			Curve4 curve = new Curve4();
			curve.ApproximationMethod = CurveApproximationMethod.Incremental;

			PathCommand cmd;
			for (int ci = 0; ci < cpts.Length; ci += 4)
			{
				curve.Initialize(
					cpts[ci + 0].X, cpts[ci + 0].Y,
					cpts[ci + 1].X, cpts[ci + 1].Y,
					cpts[ci + 2].X, cpts[ci + 2].Y,
					cpts[ci + 3].X, cpts[ci + 3].Y);

				do
				{
					cmd = curve.GetVertex(out x, out y);
					if (ci == 0 && cmd == PathCommand.MoveTo)
					{
						ras.MoveToD(x, y);
					}
					else if (cmd == PathCommand.LineTo)
					{
						ras.LineToD(x, y);
					}
				} while (cmd != PathCommand.Stop);
			}

			ras.Render(ren, Color.Red);
			
			for (int i = 1; i < 4; i++)
			{
				draw_line(ras, cpts[i - 1].X, cpts[i - 1].Y, cpts[i].X, cpts[i].Y, 1.0);
				ras.Render(ren, Color.Gray);

				int j = ci2 + i;
				draw_line(ras, cpts[j - 1].X, cpts[j - 1].Y, cpts[j].X, cpts[j].Y, 1.0);
				ras.Render(ren, Color.Gray);

				j = ci3 + i;
				draw_line(ras, cpts[j - 1].X, cpts[j - 1].Y, cpts[j].X, cpts[j].Y, 1.0);
				ras.Render(ren, Color.Gray);
			}
#endif
#if HIDE
			int i;

			// Draw random polygons
			for (i = 0; i < 10; i++)
			{
				int n = rng.Next() % 6 + 3;

				// Make the polygon. One can call move_to() more than once. 
				// In this case the rasterizer behaves like Win32 API PolyPolygon().
				ras.MoveToD(random(-30, rbuf.Width + 30), random(-30, rbuf.Height + 30));

				for (int j = 1; j < n; j++)
				{
					ras.LineToD(random(-30, rbuf.Width + 30), random(-30, rbuf.Height + 30));
				}

				// Render
				ras.Render(ren, new Color((byte)(rng.Next() & 0xFF), (byte)(rng.Next() & 0xFF), (byte)(rng.Next() & 0xFF), (byte)((rng.Next() & 0x7F) + 100)));
			}

			// Draw random ellipses
			for (i = 0; i < 50; i++)
			{
				draw_ellipse(ras,
							 random(-30, rbuf.Width + 30),
							 random(-30, rbuf.Height + 30),
							 random(3, 50),
							 random(3, 50));
				ras.Render(ren, new Color((byte)(rng.Next() & 0x7F), (byte)(rng.Next() & 0x7F), (byte)(rng.Next() & 0x7F), (byte)((rng.Next() & 0x7F) + 100)));
			}

			// Draw random straight lines
			for (i = 0; i < 20; i++)
			{
				draw_line(ras,
						  random(-30, rbuf.Width + 30),
						  random(-30, rbuf.Height + 30),
						  random(-30, rbuf.Width + 30),
						  random(-30, rbuf.Height + 30),
						  random(0.1, 10));

				ras.Render(ren, new Color((byte)(rng.Next() & 0x7F), (byte)(rng.Next() & 0x7F), (byte)(rng.Next() & 0x7F), (byte)(rng.Next() & 0x7F)));
			}
#endif
			System.Drawing.Rectangle rc = new System.Drawing.Rectangle(0, 0, (int)rbuf.Width, (int)rbuf.Height);
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(rc.Width, rc.Height, PixelFormat.Format24bppRgb);
			BitmapData bmpData = bmp.LockBits(
				rc,
				System.Drawing.Imaging.ImageLockMode.WriteOnly,
				bmp.PixelFormat);

			IntPtr p = bmpData.Scan0;
			int bytes = bmpData.Stride * bmp.Height;
			System.Runtime.InteropServices.Marshal.Copy(buf, 0, p, bytes);

			bmp.UnlockBits(bmpData);
			bmp.Save("agg_test.bmp");
#if PPM
			// Write a .ppm file
			String hdr = String.Format("P6\n{0} {1}\n255\n", rbuf.width(), rbuf.height());
			byte[] hdrBuf = ASCIIEncoding.ASCII.GetBytes(hdr);
			using (FileStream strm = new FileStream("agg_test.ppm", FileMode.Create))
			{
				strm.Write(hdrBuf, 0, hdrBuf.Length);
				strm.Write(buf, 0, (int)(rbuf.width() * rbuf.height() * 3));
			}
#endif
		}

		static double random(double min, double max)
		{
			int r = (rng.Next() << 15) | rng.Next();
			return ((r & 0xFFFFFFF) / (double)(0xFFFFFFF + 1)) * (max - min) + min;
		}

		static void draw_ellipse(Rasterizer ras, double x, double y, double rx, double ry)
		{
			int i;
			ras.MoveToD(x + rx, y);

			// Here we have a fixed number of approximation steps, namely 360
			// while in reality it's supposed to be smarter.
			for (i = 1; i < 360; i++)
			{
				double a = (double)i * 3.1415926 / 180.0;
				ras.LineToD(x + Math.Cos(a) * rx, y + Math.Sin(a) * ry);
			}
		}

		static void draw_line(Rasterizer ras, double x1, double y1, double x2, double y2, double width)
		{
			double dx = x2 - x1;
			double dy = y2 - y1;
			double d = Math.Sqrt(dx * dx + dy * dy);

			dx = width * (y2 - y1) / d;
			dy = width * (x2 - x1) / d;

			ras.MoveToD(x1 - dx, y1 + dy);
			ras.LineToD(x2 - dx, y2 + dy);
			ras.LineToD(x2 + dx, y2 - dy);
			ras.LineToD(x1 + dx, y1 - dy);
		}
	}
}
