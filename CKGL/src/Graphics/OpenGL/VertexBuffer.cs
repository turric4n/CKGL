using System;
using System.Runtime.InteropServices;
using CKGL.OpenGLBindings;
using GLuint = System.UInt32;

namespace CKGL.OpenGL
{
	internal class VertexBuffer
	{
		private static GLuint currentlyBoundVertexBuffer;

		private GLuint id;

		internal VertexBuffer()
		{
			id = GL.GenBuffer();
		}

		internal void Destroy()
		{
			if (id != default)
			{
				GL.DeleteBuffer(id);
				id = default;
			}
		}

		internal void Bind()
		{
			if (id != currentlyBoundVertexBuffer)
			{
				GL.BindBuffer(BufferTarget.Array, id);
				currentlyBoundVertexBuffer = id;
			}
		}

		// TODO - Do we need this: VertexBuffer.LoadData(byte[] buffer, BufferUsage bufferUsage)
		internal void LoadData(byte[] buffer, BufferUsage bufferUsage)
		{
			Bind();
			GL.BufferData(BufferTarget.Array, sizeof(byte) * buffer.Length, buffer, bufferUsage.ToOpenGL());
		}

		internal void LoadData<T>(VertexAttributeLayout vertexAttributeLayout, ref T[] buffer, int vertexCount, BufferUsage bufferUsage) where T : struct
		{
			if (buffer == null)
				throw new ArgumentNullException("data");

			if (buffer.Length < vertexCount)
				throw new ArgumentOutOfRangeException("vertexCount", "This parameter must be a valid index within the array.");

			int marshalledSize = Marshal.SizeOf(typeof(T));

			//Output.WriteLine($"vertexAttributeLayout.Stride: {vertexAttributeLayout.Stride} - marshalledSize: {marshalledSize}"); // Debug
			//Output.WriteLine($"Marshal.SizeOf(typeof(T)): {marshalledSize}"); // Debug

			if (vertexAttributeLayout.Stride != marshalledSize)
				throw new ArgumentOutOfRangeException("The Stride defined in VertexAttributeLayout does not match the Marshalled size of the Vertex.");

			Bind();
			GL.BufferData(BufferTarget.Array, marshalledSize * vertexCount, buffer, bufferUsage.ToOpenGL());
		}
	}
}