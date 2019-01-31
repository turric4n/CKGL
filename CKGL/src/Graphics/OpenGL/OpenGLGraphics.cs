using CKGL.OpenGLBindings;

namespace CKGL.OpenGL
{
	internal class OpenGLGraphics : GraphicsBase
	{
		internal override void Init()
		{
			GL.Init();
		}

		#region Viewport
		public override void SetViewport() => SetViewport(RenderTarget.Current);
		public override void SetViewport(RenderTarget renderTarget)
			=> SetViewport(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
		public override void SetViewport(int x, int y, int width, int height)
			=> GL.Viewport(x, y, width, height);
		#endregion

		#region ScissorTest
		public override void SetScissorTest() => SetScissorTest(RenderTarget.Current);
		public override void SetScissorTest(RenderTarget renderTarget)
			=> SetScissorTest(0, 0, (renderTarget ?? RenderTarget.Default).Width, (renderTarget ?? RenderTarget.Default).Height);
		public override void SetScissorTest(int x, int y, int width, int height)
		{
			GL.Enable(EnableCap.ScissorTest);
			GL.Scissor(x, y, width, height);
		}

		public override void SetScissorTest(bool enabled)
		{
			if (enabled)
				GL.Enable(EnableCap.ScissorTest);
			else
				GL.Disable(EnableCap.ScissorTest);
		}
		#endregion

		#region DepthRange
		public override void SetDepthRange(float near, float far)
		{
			GL.DepthRange(near.Clamp(0f, 1f), far.Clamp(0f, 1f));
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
	}
}