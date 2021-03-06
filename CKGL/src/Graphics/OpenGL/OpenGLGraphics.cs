using System.IO;
using CKGL.OpenGLBindings;

namespace CKGL.OpenGL
{
	internal class OpenGLGraphics : GraphicsBase
	{
		internal override void Init()
		{
			GL.Init();
		}

		#region Resources
		internal override VertexBuffer CreateVertexBuffer(BufferUsage bufferUsage)
		{
			return new OpenGLVertexBuffer(bufferUsage);
		}

		internal override IndexBuffer CreateIndexBuffer(BufferUsage bufferUsage)
		{
			return new OpenGLIndexBuffer(bufferUsage);
		}

		internal override GeometryInput CreateGeometryInput(IndexBuffer indexBuffer, VertexStream[] vertexStreams)
		{
			return new OpenGLGeometryInput(indexBuffer, vertexStreams);
		}

		internal override Texture CreateTexture2D(byte[] data, int width, int height, TextureFormat textureFormat, TextureFilter minFilter, TextureFilter magFilter, TextureWrap wrapX, TextureWrap wrapY)
		{
			return new OpenGLTexture(data, TextureType.Texture2D, width, height, 1, textureFormat, minFilter, magFilter, wrapX, wrapY, null);
		}

		internal override Texture CreateTexture2DFromFile(string file, TextureFormat textureFormat, TextureFilter minFilter, TextureFilter magFilter, TextureWrap wrapX, TextureWrap wrapY)
		{
			Platform.LoadImage(file, out int width, out int height, out byte[] data);
			return new OpenGLTexture(data, TextureType.Texture2D, width, height, 1, textureFormat, minFilter, magFilter, wrapX, wrapY, null);
		}

		internal override Framebuffer CreateDefaultFramebuffer()
		{
			return OpenGLFramebuffer.CreateDefault();
		}

		internal override Framebuffer CreateFramebuffer(int width, int height, int colourTextures, TextureFormat textureColourFormat, TextureFormat? textureDepthFormat = null)
		{
			return new OpenGLFramebuffer(width, height, colourTextures, textureColourFormat, textureDepthFormat);
		}

		internal override Shader CreateShader(string source)
		{
			return new OpenGLShader(source);
		}

		internal override Shader CreateShaderFromFile(string file)
		{
			if (!File.Exists(file))
				throw new FileNotFoundException("Shader file not found.", file);
			return new OpenGLShader(File.ReadAllText(file));
		}
		#endregion

		#region Viewport
		internal override void SetViewport() => SetViewport(Framebuffer.Current);
		internal override void SetViewport(Framebuffer framebuffer)
			=> SetViewport(0, 0, (framebuffer ?? Framebuffer.Default).Width, (framebuffer ?? Framebuffer.Default).Height);
		internal override void SetViewport(int x, int y, int width, int height)
			=> GL.Viewport(x, y, width, height);
		#endregion

		#region ScissorTest
		internal override void SetScissorTest() => SetScissorTest(Framebuffer.Current);
		internal override void SetScissorTest(Framebuffer framebuffer)
			=> SetScissorTest(0, 0, (framebuffer ?? Framebuffer.Default).Width, (framebuffer ?? Framebuffer.Default).Height);
		internal override void SetScissorTest(int x, int y, int width, int height)
		{
			GL.Enable(EnableCap.ScissorTest);
			GL.Scissor(x, y, width, height);
		}

		internal override void SetScissorTest(bool enabled)
		{
			if (enabled)
				GL.Enable(EnableCap.ScissorTest);
			else
				GL.Disable(EnableCap.ScissorTest);
		}
		#endregion

		#region DepthRange
		internal override void SetDepthRange(double near, double far)
		{
			GL.DepthRange(near.Clamp(0d, 1d), far.Clamp(0d, 1d));
		}
		#endregion

		#region Clear
		internal override void Clear(Colour colour)
		{
			SetClearColour(colour);
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}

		internal override void Clear(double depth)
		{
			SetClearDepth(depth);
			GL.Clear(BufferBit.Depth);
		}

		internal override void Clear(Colour colour, double depth)
		{
			SetClearColour(colour);
			SetClearDepth(depth);
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}

		private Colour clearColour = new Colour(0, 0, 0, 0);
		private void SetClearColour(Colour colour)
		{
			if (clearColour != colour)
			{
				GL.ClearColour(colour);
				clearColour = colour;
			}
		}

		private double clearDepth = 1d;
		private void SetClearDepth(double depth)
		{
			if (clearDepth != depth)
			{
				GL.ClearDepth(depth);
				clearDepth = depth;
			}
		}
		#endregion

		#region State Setters
		internal override void SetFrontFace(FrontFaceState frontFaceState)
		{
			GL.FrontFace(frontFaceState.FrontFace.ToOpenGL());
		}

		internal override void SetCullMode(CullModeState cullModeState)
		{
			if (cullModeState.Enabled)
				GL.Enable(EnableCap.CullFace);
			else
				GL.Disable(EnableCap.CullFace);

			GL.CullFace(cullModeState.Face.ToOpenGL());
		}

		internal override void SetPolygonMode(PolygonModeState polygonModeState)
		{
			GL.PolygonMode(polygonModeState.PolygonMode.ToOpenGL());
		}

		internal override void SetColourMask(ColourMaskState colourMaskState)
		{
			GL.ColourMask(colourMaskState.R, colourMaskState.G, colourMaskState.B, colourMaskState.A);
		}

		internal override void SetDepthMask(DepthMaskState depthMaskState)
		{
			GL.DepthMask(depthMaskState.Depth);
		}

		internal override void SetDepth(DepthState depthState)
		{
			if (depthState.Enabled)
				GL.Enable(EnableCap.DepthTest);
			else
				GL.Disable(EnableCap.DepthTest);

			GL.DepthFunc(depthState.DepthFunction.ToOpenGL());
		}

		internal override void SetBlend(BlendState blendState)
		{
			if (blendState.Enabled)
				GL.Enable(EnableCap.Blend);
			else
				GL.Disable(EnableCap.Blend);

			if (blendState.ColourSource == blendState.AlphaSource && blendState.ColourDestination == blendState.AlphaDestination)
				GL.BlendFunc(blendState.ColourSource.ToOpenGL(), blendState.ColourDestination.ToOpenGL());
			else
				GL.BlendFuncSeparate(blendState.ColourSource.ToOpenGL(), blendState.ColourDestination.ToOpenGL(), blendState.AlphaSource.ToOpenGL(), blendState.AlphaDestination.ToOpenGL());

			if (blendState.ColourEquation == blendState.AlphaEquation)
				GL.BlendEquation(blendState.ColourEquation.ToOpenGL());
			else
				GL.BlendEquationSeparate(blendState.ColourEquation.ToOpenGL(), blendState.AlphaEquation.ToOpenGL());
		}
		#endregion

		#region Draw
		internal override void DrawVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count)
		{
			GL.DrawArrays(primitiveTopology.ToOpenGL(), offset, count);
		}

		internal override void DrawIndexedVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count, IndexType indexType)
		{
			GL.DrawElements(primitiveTopology.ToOpenGL(), count, indexType.ToOpenGL(), offset);
		}

		internal override void DrawVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount)
		{
			GL.DrawArraysInstanced(primitiveTopology.ToOpenGL(), offset, count, primitiveCount);
		}

		internal override void DrawIndexedVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount, IndexType indexType)
		{
			GL.DrawElementsInstanced(primitiveTopology.ToOpenGL(), count, indexType.ToOpenGL(), offset, primitiveCount);
		}
		#endregion
	}
}