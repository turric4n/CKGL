using System;
using static Retyped.dom;
using WebGLEXT = Retyped.dom.Literals; // WebGL Extensions

namespace CKGL.WebGL
{
	#region Extensions
	internal static class Extensions
	{
		// WebGL 2 BlendFunc Min/Max
		internal static double MIN = 0x8007;
		internal static double MAX = 0x8008;
	}
	#endregion

	internal class WebGLGraphics : GraphicsBase
	{
		internal static WebGLRenderingContext GL = null;

		internal override void Init()
		{
			string[] contextIDs = new string[] {
					"webgl",
					"experimental-webgl",
					//"webkit-3d",
					//"moz-webgl"
				};

			foreach (string contextID in contextIDs)
			{
				try
				{
					GL = Platform.Canvas.getContext(contextID).As<WebGLRenderingContext>();
				}
				catch { }

				if (GL != null)
					break;
			}

			if (GL == null)
				throw new CKGLException("Couldn't create WebGL 1.0 context");

			// Debug
			Output.WriteLine($"GraphicsBackend - WebGL 1.0 Initialized");
			Output.WriteLine($"WebGL Context - GLSL Version: {GL.getParameter(GL.SHADING_LANGUAGE_VERSION)}");
			Output.WriteLine($"WebGL Context - VERSION: {GL.getParameter(GL.VERSION)}");
			Output.WriteLine($"WebGL Context - VENDOR: {GL.getParameter(GL.VENDOR)}");
			Output.WriteLine($"WebGL Context - RENDERER: {GL.getParameter(GL.RENDERER)}");
			var debugRendererInfo = GL.getExtension(WebGLEXT.WEBGL_debug_renderer_info);
			if (debugRendererInfo != null)
			{
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_VENDOR_WEBGL: {GL.getParameter(debugRendererInfo.UNMASKED_VENDOR_WEBGL)}");
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_RENDERER_WEBGL: {GL.getParameter(debugRendererInfo.UNMASKED_RENDERER_WEBGL)}");
			}
			//Output.WriteLine($"WebGL - Extensions: \n{string.Join("\n", gl.GetSupportedExtensions())}");
		}

		#region Resources
		internal override VertexBuffer CreateVertexBuffer(BufferUsage bufferUsage)
		{
			return new WebGLVertexBuffer(bufferUsage);
		}

		internal override IndexBuffer CreateIndexBuffer(BufferUsage bufferUsage)
		{
			return new WebGLIndexBuffer(bufferUsage);
		}

		internal override GeometryInput CreateGeometryInput(IndexBuffer indexBuffer, VertexStream[] vertexStreams)
		{
			return new WebGLGeometryInput(indexBuffer, vertexStreams);
		}

		internal override Shader CreateShader(string source)
		{
			return new WebGLShader(source);
		}

		internal override Shader CreateShaderFromFile(string file)
		{
			throw new CKGLException("WebGL doesn't support creating shaders from file.");
		}
		#endregion

		#region Viewport
		internal override void SetViewport() => SetViewport(RenderTarget.Current);
		internal override void SetViewport(RenderTarget renderTarget)
			=> SetViewport(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
		internal override void SetViewport(int x, int y, int width, int height)
			=> GL.viewport(x, y, width, height);
		#endregion

		#region ScissorTest
		internal override void SetScissorTest() => SetScissorTest(RenderTarget.Current);
		internal override void SetScissorTest(RenderTarget renderTarget)
			=> SetScissorTest(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
		internal override void SetScissorTest(int x, int y, int width, int height)
		{
			GL.enable(GL.SCISSOR_TEST);
			GL.scissor(x, y, width, height);
		}

		internal override void SetScissorTest(bool enabled)
		{
			if (enabled)
				GL.enable(GL.SCISSOR_TEST);
			else
				GL.disable(GL.SCISSOR_TEST);
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
			GL.clear((int)GL.COLOR_BUFFER_BIT | (int)GL.DEPTH_BUFFER_BIT);
		}

		internal override void Clear(double depth)
		{
			SetClearDepth(depth);
			GL.clear(GL.DEPTH_BUFFER_BIT);
		}

		internal override void Clear(Colour colour, double depth)
		{
			SetClearColour(colour);
			SetClearDepth(depth);
			GL.clear((int)GL.COLOR_BUFFER_BIT | (int)GL.DEPTH_BUFFER_BIT);
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
			GL.frontFace(frontFaceState.FrontFace.ToWebGL());
		}

		internal override void SetCullMode(CullModeState cullModeState)
		{
			if (cullModeState.Enabled)
				GL.enable(GL.CULL_FACE);
			else
				GL.disable(GL.CULL_FACE);

			GL.cullFace(cullModeState.Face.ToWebGL());
		}

		internal override void SetPolygonMode(PolygonModeState polygonModeState)
		{
			//GL.polygonMode(polygonModeState.PolygonMode.ToWebGL());
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
				GL.enable(GL.DEPTH_TEST);
			else
				GL.disable(GL.DEPTH_TEST);

			GL.depthFunc(depthState.DepthFunction.ToWebGL());
		}

		internal override void SetBlend(BlendState blendState)
		{
			if (blendState.Enabled)
				GL.enable(GL.BLEND);
			else
				GL.disable(GL.BLEND);

			if (blendState.ColourSource == blendState.AlphaSource && blendState.ColourDestination == blendState.AlphaDestination)
				GL.blendFunc(blendState.ColourSource.ToWebGL(), blendState.ColourDestination.ToWebGL());
			else
				GL.blendFuncSeparate(blendState.ColourSource.ToWebGL(), blendState.ColourDestination.ToWebGL(), blendState.AlphaSource.ToWebGL(), blendState.AlphaDestination.ToWebGL());

			if (blendState.ColourEquation == blendState.AlphaEquation)
				GL.blendEquation(blendState.ColourEquation.ToWebGL());
			else
				GL.blendEquationSeparate(blendState.ColourEquation.ToWebGL(), blendState.AlphaEquation.ToWebGL());
		}
		#endregion

		#region Draw
		internal override void DrawVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count)
		{
			GL.drawArrays(primitiveTopology.ToWebGL(), offset, count);
		}

		internal override void DrawIndexedVertexArrays(PrimitiveTopology primitiveTopology, int offset, int count, IndexType indexType)
		{
			GL.drawElements(primitiveTopology.ToWebGL(), count, indexType.ToWebGL(), offset);
		}

		internal override void DrawVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount)
		{
			//GL.drawArraysInstanced(primitiveTopology.ToWebGL(), offset, count, primitiveCount);
			throw new CKGLException("TODO - drawArraysInstanced");
		}

		internal override void DrawIndexedVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount, IndexType indexType)
		{
			//GL.drawElementsInstanced(primitiveTopology.ToWebGL(), count, indexType.ToWebGL(), offset, primitiveCount);
			throw new CKGLException("TODO - DrawElementsInstanced");
		}
		#endregion
	}
}