using static Retyped.dom; // DOM / WebGL Types
using static Retyped.webgl2; // WebGL 2.0 Types - WebGL2RenderingContext, WebGLVertexArrayObject
using static Retyped.webgl2.WebGL2RenderingContext; // WebGL 2.0 Enums
using WebGL2_EXT = Retyped.dom.Literals; // WebGL Extensions

namespace CKGL.WebGL2
{
	internal class WebGL2Graphics : GraphicsBase
	{
		#region Missing Enums
		// WebGL 2 BlendFunc Min/Max
		internal static double MIN = 0x8007;
		internal static double MAX = 0x8008;
		#endregion

		#region Extensions
		internal static class Extensions
		{
			private static WEBGL_debug_renderer_info _WEBGL_debug_renderer_info;
			internal static WEBGL_debug_renderer_info WEBGL_debug_renderer_info
			{
				get
				{
					if (_WEBGL_debug_renderer_info == null)
					{
						_WEBGL_debug_renderer_info = GL.getExtension(WebGL2_EXT.WEBGL_debug_renderer_info);
						Output.WriteLine("WebGL 2.0 Extension \"WEBGL_debug_renderer_info\" acquired.");

						if (_WEBGL_debug_renderer_info == null)
							throw new CKGLException("WebGL 2.0 \"WEBGL_debug_renderer_info\" extension was requested, but is not supported in this browser.");
					}

					return _WEBGL_debug_renderer_info;
				}
			}
		}
		#endregion

		internal static WebGL2RenderingContext GL = null;

		internal static bool IsLittleEndian = new Retyped.es5.Uint8Array(new Retyped.es5.Uint32Array(new uint[] { 0x12345678 }).buffer)[0] == 0x78;

		internal override void Init()
		{
			GL = Platform.WebGL2Context;

			if (GL == null)
				throw new CKGLException("Couldn't create WebGL 2.0 context");

			// Debug
			Output.WriteLine("GraphicsBackend - WebGL 2.0 Initialized");
			var attributes = GL.getContextAttributes();
			Output.WriteLine($"WebGL Context - Attributes: alpha[{attributes.alpha}] premultipliedAlpha[{attributes.premultipliedAlpha}] depth[{attributes.depth}] stencil[{attributes.stencil}] antialias[{attributes.antialias}] preserveDrawingBuffer[{attributes.preserveDrawingBuffer}] failIfMajorPerformanceCaveat[{attributes.failIfMajorPerformanceCaveat}]");
			Output.WriteLine($"WebGL Context - GLSL Version: {GL.getParameter(SHADING_LANGUAGE_VERSION)}");
			Output.WriteLine($"WebGL Context - VERSION: {GL.getParameter(VERSION)}");
			Output.WriteLine($"WebGL Context - VENDOR: {GL.getParameter(VENDOR)}");
			Output.WriteLine($"WebGL Context - RENDERER: {GL.getParameter(RENDERER)}");
			try
			{
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_VENDOR_WEBGL: {GL.getParameter(Extensions.WEBGL_debug_renderer_info.UNMASKED_VENDOR_WEBGL)}");
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_RENDERER_WEBGL: {GL.getParameter(Extensions.WEBGL_debug_renderer_info.UNMASKED_RENDERER_WEBGL)}");
			}
			catch { }
			Output.WriteLine($"WebGL Context - MaxColourAttachments: {GL.getParameter(MAX_COLOR_ATTACHMENTS_Static)}");
			Output.WriteLine($"WebGL Context - MaxCubeMapTextureSize: {GL.getParameter(MAX_CUBE_MAP_TEXTURE_SIZE)}");
			Output.WriteLine($"WebGL Context - MaxDrawBuffers: {GL.getParameter(MAX_DRAW_BUFFERS_Static)}");
			Output.WriteLine($"WebGL Context - MaxElementIndices: {GL.getParameter(MAX_ELEMENTS_INDICES_Static)}");
			Output.WriteLine($"WebGL Context - MaxElementVertices: {GL.getParameter(MAX_ELEMENTS_VERTICES_Static)}");
			Output.WriteLine($"WebGL Context - MaxRenderbufferSize: {GL.getParameter(MAX_RENDERBUFFER_SIZE)}");
			Output.WriteLine($"WebGL Context - MaxSamples: {GL.getParameter(MAX_SAMPLES_Static)}");
			Output.WriteLine($"WebGL Context - MaxTextureImageUnits: {GL.getParameter(MAX_TEXTURE_IMAGE_UNITS)}");
			Output.WriteLine($"WebGL Context - MaxTextureSize: {GL.getParameter(MAX_TEXTURE_SIZE)}");
			//Output.WriteLine($"WebGL - Extensions: \n{string.Join("\n", GL.GetSupportedExtensions())}");
		}

		#region Resources
		internal override VertexBuffer CreateVertexBuffer(BufferUsage bufferUsage)
		{
			return new WebGL2VertexBuffer(bufferUsage);
		}

		internal override IndexBuffer CreateIndexBuffer(BufferUsage bufferUsage)
		{
			return new WebGL2IndexBuffer(bufferUsage);
		}

		internal override GeometryInput CreateGeometryInput(IndexBuffer indexBuffer, VertexStream[] vertexStreams)
		{
			return new WebGL2GeometryInput(indexBuffer, vertexStreams);
		}

		internal override Texture CreateTexture2D(byte[] data, int width, int height, TextureFormat textureFormat, TextureFilter minFilter, TextureFilter magFilter, TextureWrap wrapX, TextureWrap wrapY)
		{
			return new WebGL2Texture(data, TextureType.Texture2D, width, height, 1, textureFormat, minFilter, magFilter, wrapX, wrapY, null);
		}

		internal override Texture CreateTexture2DFromFile(string file, TextureFormat textureFormat, TextureFilter minFilter, TextureFilter magFilter, TextureWrap wrapX, TextureWrap wrapY)
		{
			return new WebGL2Texture(file, TextureType.Texture2D, textureFormat, minFilter, magFilter, wrapX, wrapY, null);
		}

		internal override Framebuffer CreateDefaultFramebuffer()
		{
			return WebGL2Framebuffer.CreateDefault();
		}

		internal override Framebuffer CreateFramebuffer(int width, int height, int colourTextures, TextureFormat textureColourFormat, TextureFormat? textureDepthFormat = null)
		{
			return new WebGL2Framebuffer(width, height, colourTextures, textureColourFormat, textureDepthFormat);
		}

		internal override Shader CreateShader(string source)
		{
			return new WebGL2Shader(source);
		}

		internal override Shader CreateShaderFromFile(string file)
		{
			throw new CKGLException("WebGL doesn't support creating shaders from file.");
		}
		#endregion

		#region Viewport
		internal override void SetViewport() => SetViewport(Framebuffer.Current);
		internal override void SetViewport(Framebuffer framebuffer)
			=> SetViewport(0, 0, (framebuffer ?? Framebuffer.Default).Width, (framebuffer ?? Framebuffer.Default).Height);
		internal override void SetViewport(int x, int y, int width, int height)
			=> GL.viewport(x, y, width, height);
		#endregion

		#region ScissorTest
		internal override void SetScissorTest() => SetScissorTest(Framebuffer.Current);
		internal override void SetScissorTest(Framebuffer framebuffer)
			=> SetScissorTest(0, 0, (framebuffer ?? Framebuffer.Default).Width, (framebuffer ?? Framebuffer.Default).Height);
		internal override void SetScissorTest(int x, int y, int width, int height)
		{
			GL.enable(SCISSOR_TEST);
			GL.scissor(x, y, width, height);
		}

		internal override void SetScissorTest(bool enabled)
		{
			if (enabled)
				GL.enable(SCISSOR_TEST);
			else
				GL.disable(SCISSOR_TEST);
		}
		#endregion

		#region DepthRange
		internal override void SetDepthRange(double near, double far)
		{
			GL.depthRange(near.Clamp(0d, 1d), far.Clamp(0d, 1d));
		}
		#endregion

		#region Clear
		internal override void Clear(Colour colour)
		{
			SetClearColour(colour);
			GL.clear((int)COLOR_BUFFER_BIT | (int)DEPTH_BUFFER_BIT);
		}

		internal override void Clear(double depth)
		{
			SetClearDepth(depth);
			GL.clear(DEPTH_BUFFER_BIT);
		}

		internal override void Clear(Colour colour, double depth)
		{
			SetClearColour(colour);
			SetClearDepth(depth);
			GL.clear((int)COLOR_BUFFER_BIT | (int)DEPTH_BUFFER_BIT);
		}

		private Colour clearColour = new Colour(0, 0, 0, 0);
		private void SetClearColour(Colour colour)
		{
			if (clearColour != colour)
			{
				GL.clearColor(colour.R, colour.G, colour.B, colour.A);
				clearColour = colour;
			}
		}

		private double clearDepth = 1d;
		private void SetClearDepth(double depth)
		{
			if (clearDepth != depth)
			{
				GL.clearDepth(depth);
				clearDepth = depth;
			}
		}
		#endregion

		#region State Setters
		internal override void SetFrontFace(FrontFaceState frontFaceState)
		{
			GL.frontFace(frontFaceState.FrontFace.ToWebGL2());
		}

		internal override void SetCullMode(CullModeState cullModeState)
		{
			if (cullModeState.Enabled)
				GL.enable(CULL_FACE);
			else
				GL.disable(CULL_FACE);

			GL.cullFace(cullModeState.Face.ToWebGL2());
		}

		internal override void SetPolygonMode(PolygonModeState polygonModeState)
		{
			//GL.polygonMode(polygonModeState.PolygonMode.ToWebGL2());
			throw new CKGLException("glPolygonMode is not available in WebGL.");
		}

		internal override void SetColourMask(ColourMaskState colourMaskState)
		{
			GL.colorMask(colourMaskState.R, colourMaskState.G, colourMaskState.B, colourMaskState.A);
		}

		internal override void SetDepthMask(DepthMaskState depthMaskState)
		{
			GL.depthMask(depthMaskState.Depth);
		}

		internal override void SetDepth(DepthState depthState)
		{
			if (depthState.Enabled)
				GL.enable(DEPTH_TEST);
			else
				GL.disable(DEPTH_TEST);

			GL.depthFunc(depthState.DepthFunction.ToWebGL2());
		}

		internal override void SetBlend(BlendState blendState)
		{
			if (blendState.Enabled)
				GL.enable(BLEND);
			else
				GL.disable(BLEND);

			if (blendState.ColourSource == blendState.AlphaSource && blendState.ColourDestination == blendState.AlphaDestination)
				GL.blendFunc(blendState.ColourSource.ToWebGL2(), blendState.ColourDestination.ToWebGL2());
			else
				GL.blendFuncSeparate(blendState.ColourSource.ToWebGL2(), blendState.ColourDestination.ToWebGL2(), blendState.AlphaSource.ToWebGL2(), blendState.AlphaDestination.ToWebGL2());

			if (blendState.ColourEquation == blendState.AlphaEquation)
				GL.blendEquation(blendState.ColourEquation.ToWebGL2());
			else
				GL.blendEquationSeparate(blendState.ColourEquation.ToWebGL2(), blendState.AlphaEquation.ToWebGL2());
		}
		#endregion

		#region Draw
		internal override void DrawVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count)
		{
			GL.drawArrays(primitiveTopology.ToWebGL2(), offset, count);
		}

		internal override void DrawIndexedVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count, IndexType indexType)
		{
			GL.drawElements(primitiveTopology.ToWebGL2(), count, indexType.ToWebGL2(), offset);
		}

		internal override void DrawVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount)
		{
			GL.drawArraysInstanced(primitiveTopology.ToWebGL2(), offset, count, primitiveCount);
		}

		internal override void DrawIndexedVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount, IndexType indexType)
		{
			GL.drawElementsInstanced(primitiveTopology.ToWebGL2(), count, indexType.ToWebGL2(), offset, primitiveCount);
		}
		#endregion
	}
}