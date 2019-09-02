using System;
using Bridge.Html5; // HTML5 DOM Manipulation
using static CKGL.WebGL.Extensions; // WebGL Extensions
using static Retyped.webgl2; // WebGL Types - WebGL2RenderingContext, WebGLVertexArrayObject
using static Retyped.webgl2.WebGL2RenderingContext; // WebGL Enums

namespace CKGL.WebGL
{
	#region Extensions
	internal static class Extensions
	{
		// WEBGL_debug_renderer_info
		internal static double UNMASKED_VENDOR_WEBGL = 0x9245;
		internal static double UNMASKED_RENDERER_WEBGL = 0x9246;

		// WebGL 2 BlendFunc Min/Max
		internal static double MIN = 0x8007;
		internal static double MAX = 0x8008;
	}
	#endregion

	internal class WebGLGraphics : GraphicsBase
	{
		internal static WebGL2RenderingContext GL = null;

		internal override void Init()
		{
			// Create 3D Context
			string[] names = new string[] {
				"webgl2",
				"experimental-webgl2",
				//"webgl",
				//"experimental-webgl",
				//"webkit-3d",
				//"moz-webgl"
			};

			foreach (string name in names)
			{
				try
				{
					GL = Platform.Canvas.GetContext(name).As<WebGL2RenderingContext>();
					//gl = (WebGL2RenderingContext)Platform.Canvas.GetContext(name);
				}
				catch { }

				if (GL != null)
					break;
			}

			if (GL == null)
				Platform.Canvas.ParentElement.ReplaceChild(new HTMLParagraphElement { InnerHTML = "<b>Either the browser doesn't support WebGL 2.0 or it is disabled.<br>Please follow <a href=\"https://get.webgl.org/\">Get WebGL</a>.</b>" }, Platform.Canvas);

			// Debug
			Output.WriteLine($"Platform - HTML5 Initialized");
			Output.WriteLine($"Window.Navigator - Platform: {Window.Navigator.Platform}");
			Output.WriteLine($"Window.Navigator - UserAgent: {Window.Navigator.UserAgent}");
			Output.WriteLine($"WebGL Context - GLSL Version: {GL.getParameter(SHADING_LANGUAGE_VERSION)}");
			Output.WriteLine($"WebGL Context - VERSION: {GL.getParameter(VERSION)}");
			Output.WriteLine($"WebGL Context - VENDOR: {GL.getParameter(VENDOR)}");
			Output.WriteLine($"WebGL Context - RENDERER: {GL.getParameter(RENDERER)}");
			var dbgRenderInfo = GL.getExtension("WEBGL_debug_renderer_info");
			if (dbgRenderInfo != null)
			{
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_VENDOR_WEBGL: {GL.getParameter(UNMASKED_VENDOR_WEBGL)}");
				Output.WriteLine($"WebGL Context - WEBGL_debug_renderer_info.UNMASKED_RENDERER_WEBGL: {GL.getParameter(UNMASKED_RENDERER_WEBGL)}");
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

#if false
		#region Viewport
		internal override void SetViewport() => SetViewport(RenderTarget.Current);
		internal override void SetViewport(RenderTarget renderTarget)
			=> SetViewport(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
		internal override void SetViewport(int x, int y, int width, int height)
			=> GL.Viewport(x, y, width, height);
		#endregion

		#region ScissorTest
		internal override void SetScissorTest() => SetScissorTest(RenderTarget.Current);
		internal override void SetScissorTest(RenderTarget renderTarget)
			=> SetScissorTest(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
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
#endif

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
			GL.frontFace(frontFaceState.FrontFace.ToWebGL());
		}

		internal override void SetCullMode(CullModeState cullModeState)
		{
			if (cullModeState.Enabled)
				GL.enable(CULL_FACE);
			else
				GL.disable(CULL_FACE);

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
				GL.enable(DEPTH_TEST);
			else
				GL.disable(DEPTH_TEST);

			GL.depthFunc(depthState.DepthFunction.ToWebGL());
		}

		internal override void SetBlend(BlendState blendState)
		{
			if (blendState.Enabled)
				GL.enable(BLEND);
			else
				GL.disable(BLEND);

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
			GL.drawArraysInstanced(primitiveTopology.ToWebGL(), offset, count, primitiveCount);
		}

		internal override void DrawIndexedVertexArraysInstanced(PrimitiveTopology primitiveTopology, int offset, int count, int primitiveCount, IndexType indexType)
		{
			GL.drawElementsInstanced(primitiveTopology.ToWebGL(), count, indexType.ToWebGL(), offset, primitiveCount);
		}
		#endregion
	}
}