using CKGL.OpenGLBindings;
using System;

namespace CKGL
{
	public static class Graphics
	{
		private static GraphicsBase graphics;

		internal static void Init()
		{
			switch (Platform.GraphicsBackend)
			{
				case GraphicsBackend.Vulkan:
					throw new NotImplementedException("Vulkan Backend not implemented yet.");
				case GraphicsBackend.OpenGL:
				case GraphicsBackend.OpenGLES:
					graphics = new OpenGL.OpenGLGraphics();
					break;
				default:
					throw new NotSupportedException($"GraphicsBackend {Platform.GraphicsBackend} not supported.");
			}

			graphics.Init();

			State.Init();

			Platform.Events.OnWinResized += () =>
			{
				SetViewport();
				SetScissorTest();
			};

			// Debug
			Output.WriteLine($"Graphics Initialized");
		}

		public static int DrawCalls { get; private set; }

		public static void PreDraw()
		{
			DrawCalls = 0;
			State.PreDraw();
		}

		// TODO - Move SetViewport() to own state
		#region Viewport
		public static void SetViewport() => graphics.SetViewport();
		public static void SetViewport(RenderTarget renderTarget) => graphics.SetViewport(renderTarget);
		public static void SetViewport(int x, int y, int width, int height) => graphics.SetViewport(x, y, width, height);
		#endregion

		// TODO - Move SetScissorTest() to own state
		#region ScissorTest
		public static void SetScissorTest() => graphics.SetScissorTest();
		public static void SetScissorTest(RenderTarget renderTarget) => graphics.SetScissorTest(renderTarget);
		public static void SetScissorTest(int x, int y, int width, int height) => graphics.SetScissorTest(x, y, width, height);
		public static void SetScissorTest(bool enabled) => graphics.SetScissorTest(enabled);
		#endregion

		// TODO - Move SetDepthRange() to own state
		#region DepthRange
		public static void SetDepthRange(float near, float far) => graphics.SetDepthRange(near, far);
		#endregion

		#region Clear
		public static void SetClearColour(Colour colour)
		{
			GL.ClearColour(colour);
		}
		public static void SetClearDepth(float depth)
		{
			GL.ClearDepth(depth);
		}

		public static void ClearColour()
		{
			GL.Clear(BufferBit.Colour);
		}
		public static void ClearColour(Colour colour)
		{
			GL.ClearColour(colour);
			GL.Clear(BufferBit.Colour);
		}

		public static void ClearDepth()
		{
			GL.Clear(BufferBit.Depth);
		}
		public static void ClearDepth(float depth)
		{
			GL.ClearDepth(depth);
			GL.Clear(BufferBit.Depth);
		}

		public static void Clear()
		{
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}

		public static void Clear(Colour colour)
		{
			GL.ClearColour(colour);
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}
		public static void Clear(float depth)
		{
			GL.ClearDepth(depth);
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}
		public static void Clear(Colour colour, float depth)
		{
			GL.ClearColour(colour);
			GL.ClearDepth(depth);
			GL.Clear(BufferBit.Colour | BufferBit.Depth);
		}
		#endregion

		#region State Setters
		internal static void SetFrontFace(FrontFaceState frontFaceState) => graphics.SetFrontFace(frontFaceState);
		internal static void SetCullMode(CullModeState cullModeState) => graphics.SetCullMode(cullModeState);
		internal static void SetPolygonMode(PolygonModeState polygonModeState) => graphics.SetPolygonMode(polygonModeState);
		internal static void SetColourMask(ColourMaskState colourMaskState) => graphics.SetColourMask(colourMaskState);
		internal static void SetDepthMask(DepthMaskState depthMaskState) => graphics.SetDepthMask(depthMaskState);
		internal static void SetDepth(DepthState depthState) => graphics.SetDepth(depthState);
		internal static void SetBlend(BlendState blendState) => graphics.SetBlend(blendState);
		#endregion

		#region State
		public static class State
		{
			internal static Action OnStateChanging;
			internal static Action OnStateChanged;

			public static int Changes { get; private set; }

			internal static void Init()
			{
				Reset();

				OnStateChanged += () => { Changes++; };
			}

			public static void Reset()
			{
				FrontFaceState.Reset();
				CullModeState.Reset();
				PolygonModeState.Reset();
				BlendState.Reset();
				DepthState.Reset();
				ColourMaskState.Reset();
				DepthMaskState.Reset();
			}

			internal static void PreDraw()
			{
				Changes = 0;
			}
		}
		#endregion

		#region Draw
		public static void DrawVertexArrays(DrawMode drawMode, int offset, int count)
		{
			GL.DrawArrays(drawMode, offset, count);
			DrawCalls++;
		}

		public static void DrawIndexedVertexArrays(DrawMode drawMode, int offset, int count)
		{
			GL.DrawElements(drawMode, count, IndexType.UnsignedInt, offset);
			DrawCalls++;
		}
		#endregion
	}
}