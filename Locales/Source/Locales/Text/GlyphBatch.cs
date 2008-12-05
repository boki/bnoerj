// Copyright (C) 2007, Bjoern Graf <bjoern.graf@gmx.net>
// All rights reserved.
//
// This software is licensed as described in the file license.txt, which
// you should have received as part of this distribution. The terms
// are also available at http://www.codeplex.com/Bnoerj/Project/License.aspx.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bnoerj.Locales.Text
{
	public class GlyphBatch : IDisposable
	{
		[Serializable, StructLayout(LayoutKind.Sequential)]
		struct GlyphVertex
		{
			public Vector4 Position;
			public Vector2 TextureCoordinate;
			public Color Color;

			public static readonly VertexElement[] VertexElements = new VertexElement[] {
					new VertexElement(0, 0, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.Position, 0),
					new VertexElement(0, 16, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
					new VertexElement(0, 24, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0)
				};

			public GlyphVertex(Vector4 source, Vector2 textureCoordinate, Color color)
			{
				Position = source;
				TextureCoordinate = textureCoordinate;
				Color = color;
			}

			public unsafe static int SizeInBytes
			{
				get { return sizeof(GlyphVertex); }
			}

			public override bool Equals(Object obj)
			{
				return obj != null && obj.GetType() == typeof(GlyphVertex) && this == (GlyphVertex)obj;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}

			public override String ToString()
			{
				Object[] args = new object[] { Position, Color, TextureCoordinate };
				return String.Format(CultureInfo.CurrentCulture, "{{Position:{0} Color:{1} TextureCoordinate:{3}}}", args);
			}

			public static bool operator == (GlyphVertex vertex1, GlyphVertex vertex2)
			{
				return vertex1.Position == vertex2.Position &&
					vertex1.TextureCoordinate == vertex2.TextureCoordinate &&
					vertex1.Color == vertex2.Color;
			}

			public static bool operator !=(GlyphVertex vertex1, GlyphVertex vertex2)
			{
				return vertex1.Position != vertex2.Position ||
					vertex1.TextureCoordinate != vertex2.TextureCoordinate ||
					vertex1.Color != vertex2.Color;
			}
		}

		GraphicsDevice graphicsDevice;
		bool inBeginEnd;

		GlyphVertex[] glyphQueue;
		int glyphQueueCount;

		DynamicVertexBuffer vertexBuffer;
		VertexDeclaration vertexDeclaration;
		DynamicIndexBuffer indexBuffer;
		VertexShader vertexShader;
		PixelShader pixelShader;
		int vertexBufferPosition;
		Texture2D glyphTexture;

		const int vertexBufferSize = 2048;
		const int indexBufferSize = 6 * vertexBufferSize;

		public event EventHandler Disposing;
		bool isDisposed;

		public GlyphBatch(GraphicsDevice graphicsDevice)
		{
			if (graphicsDevice == null)
			{
				throw new ArgumentNullException("graphicsDevice", "Graphics device cannot be null");
			}

			this.graphicsDevice = graphicsDevice;

			this.glyphQueue = new GlyphVertex[vertexBufferSize];

			this.vertexShader = new VertexShader(this.graphicsDevice, GlyphShaderCode.VertexShader);
			this.pixelShader = new PixelShader(this.graphicsDevice, GlyphShaderCode.PixelShader);
			this.vertexDeclaration = new VertexDeclaration(this.graphicsDevice, GlyphVertex.VertexElements);
			this.AllocateBuffers();
		}

		public bool IsDisposed
		{
			get { return isDisposed; }
		}

		public void Begin(Font font)
		{
			if (inBeginEnd == true)
			{
				throw new InvalidOperationException("End must be called before Begin");
			}
			if (font == null)
			{
				throw new ArgumentNullException("font", "Font cannot be null");
			}

			inBeginEnd = true;
			glyphTexture = font.Texture;
		}

		public void End()
		{
			if (inBeginEnd == false)
			{
				throw new InvalidOperationException("Begin must be called before End");
			}

			if (glyphQueueCount > 0)
			{
				RenderBatch();
				glyphTexture = null;
			}

			inBeginEnd = false;
		}

		unsafe void RenderBatch()
		{
			RenderState renderState = graphicsDevice.RenderState;
			renderState.CullMode = CullMode.CullCounterClockwiseFace;
			renderState.DepthBufferEnable = false;
#if !IGNORED
			renderState.AlphaBlendEnable = true;
			renderState.AlphaBlendOperation = BlendFunction.Add;
			renderState.SourceBlend = Blend.SourceAlpha;
			renderState.DestinationBlend = Blend.InverseSourceAlpha;
			renderState.SeparateAlphaBlendEnabled = false;
			renderState.AlphaTestEnable = true;
			renderState.AlphaFunction = CompareFunction.Greater;
			renderState.ReferenceAlpha = 0;
#endif
			SamplerState state = graphicsDevice.SamplerStates[0];
			state.AddressU = TextureAddressMode.Clamp;
			state.AddressV = TextureAddressMode.Clamp;
			state.MagFilter = TextureFilter.None;
			state.MinFilter = TextureFilter.None;
			state.MipFilter = TextureFilter.None;
			state.MipMapLevelOfDetailBias = 0.0f;
			state.MaxMipLevel = 0;

			graphicsDevice.VertexShader = vertexShader;
			graphicsDevice.PixelShader = pixelShader;
			graphicsDevice.VertexDeclaration = vertexDeclaration;

			Viewport viewport = graphicsDevice.Viewport;
			Vector4 vpSize = new Vector4((float)viewport.Width, (float)viewport.Height, 0f, 0f);
			graphicsDevice.SetVertexShaderConstant(0, vpSize);

			graphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, sizeof(GlyphVertex));
			graphicsDevice.Indices = indexBuffer;

			graphicsDevice.Textures[0] = glyphTexture;
			graphicsDevice.SetVertexShaderConstant(1, new Vector4((float)glyphTexture.Width, (float)glyphTexture.Height, 0f, 0f));

			int count = glyphQueueCount;
			int offset = 0;
			while (count > 0)
			{
				SetDataOptions noOverwrite = SetDataOptions.NoOverwrite;
				int drawCount = count;
				if (drawCount > (vertexBufferSize - vertexBufferPosition))
				{
					drawCount = vertexBufferSize - vertexBufferPosition;
					if (drawCount < vertexBufferSize / 8)
					{
						vertexBufferPosition = 0;
						noOverwrite = SetDataOptions.Discard;
						drawCount = count;
						if (drawCount > vertexBufferSize)
						{
							drawCount = vertexBufferSize;
						}
					}
				}

				int vertexStride = sizeof(GlyphVertex);
				int offsetInBytes = (vertexBufferPosition * vertexStride);
				vertexBuffer.SetData<GlyphVertex>(offsetInBytes, glyphQueue, offset, drawCount, vertexStride, noOverwrite);

				// 2 triangles = 4 vertices, 6 indeces
				int minVertexIndex = vertexBufferPosition;
				int numVertices = drawCount;
				int startIndex = vertexBufferPosition / 2 * 3;
				int primitiveCount = drawCount / 2;
				graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, minVertexIndex, numVertices, startIndex, primitiveCount);

				vertexBufferPosition += drawCount;
				offset += drawCount;
				count -= drawCount;
			}

			glyphQueueCount = 0;
		}

		public void DrawBackground(Vector2 position, Vector2 size, Color color)
		{
			if (inBeginEnd == false)
			{
				throw new InvalidOperationException("Begin must be called before Draw");
			}

			Rectangle source = new Rectangle(glyphTexture.Width - 1, glyphTexture.Height - 1, 1, 1);
			Vector2 pos = position;
			AppendGlyphVertex(pos, new Vector2(source.Left, source.Top), color);
			pos.X += size.X;
			AppendGlyphVertex(pos, new Vector2(source.Right, source.Top), color);
			pos.Y += size.Y;
			AppendGlyphVertex(pos, new Vector2(source.Right, source.Bottom), color);
			pos.X -= size.X;
			AppendGlyphVertex(pos, new Vector2(source.Left, source.Bottom), color);
		}

		public void Draw(Vector2 position, Rectangle source, Color color)
		{
			if (inBeginEnd == false)
			{
				throw new InvalidOperationException("Begin must be called before Draw");
			}

			Vector2 pos = position;
			AppendGlyphVertex(pos, new Vector2(source.Left, source.Top), color);
			pos.X += source.Width;
			AppendGlyphVertex(pos, new Vector2(source.Right, source.Top), color);
			pos.Y += source.Height;
			AppendGlyphVertex(pos, new Vector2(source.Right, source.Bottom), color);
			pos.X -= source.Width;
			AppendGlyphVertex(pos, new Vector2(source.Left, source.Bottom), color);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing == true && isDisposed == false)
			{
				if (Disposing != null)
				{
					Disposing(this, EventArgs.Empty);
				}

				if (vertexDeclaration != null)
				{
					vertexDeclaration.Dispose();
				}
				if (vertexShader != null)
				{
					vertexShader.Dispose();
				}
				if (pixelShader != null)
				{
					pixelShader.Dispose();
				}
				if (vertexBuffer != null)
				{
					vertexBuffer.Dispose();
				}
				if (indexBuffer != null)
				{
					indexBuffer.Dispose();
				}

				isDisposed = true;
			}
		}

		void AllocateBuffers()
		{
			if (vertexBuffer == null || vertexBuffer.IsDisposed == true)
			{
				vertexBuffer = new DynamicVertexBuffer(graphicsDevice, typeof(GlyphVertex), vertexBufferSize, BufferUsage.WriteOnly);
				vertexBufferPosition = 0;
			}

			if (indexBuffer == null || indexBuffer.IsDisposed == true)
			{
				indexBuffer = new DynamicIndexBuffer(graphicsDevice, typeof(short), indexBufferSize, BufferUsage.WriteOnly);
				indexBuffer.SetData<short>(CreateIndexData());
			}
		}

		static short[] CreateIndexData()
		{
			short[] indeces = new short[indexBufferSize];
			for (int i = 0; i < vertexBufferSize; i++)
			{
				indeces[(i * 6) + 0] = (short)((i * 4) + 0);
				indeces[(i * 6) + 1] = (short)((i * 4) + 1);
				indeces[(i * 6) + 2] = (short)((i * 4) + 2);
				indeces[(i * 6) + 3] = (short)((i * 4) + 0);
				indeces[(i * 6) + 4] = (short)((i * 4) + 2);
				indeces[(i * 6) + 5] = (short)((i * 4) + 3);
			}
			return indeces;
		}

		unsafe void AppendGlyphVertex(Vector2 position, Vector2 textureCoordinate, Color color)
		{
			if (glyphQueueCount >= glyphQueue.Length)
			{
				Array.Resize<GlyphVertex>(ref glyphQueue, glyphQueue.Length * 2);
			}

			fixed (GlyphVertex* pVertex = &glyphQueue[glyphQueueCount++])
			{
				pVertex->Position.X = position.X;
				pVertex->Position.Y = position.Y;
				pVertex->Position.Z = 0;
				pVertex->Position.W = 1;
				pVertex->TextureCoordinate = textureCoordinate;
				pVertex->Color = color;
			}
		}
	}
}
