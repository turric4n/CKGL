using System;
using CKGL.OpenGLBindings;
using GLint = System.Int32;
using GLuint = System.UInt32;

namespace CKGL.OpenGL
{
	internal struct VertexAttribute
	{
		internal readonly VertexType VertexType;
		internal readonly GLint Count;
		internal readonly GLint Size;
		internal readonly bool Normalized;

		internal VertexAttribute(VertexType vertexType, GLint count, bool normalized)
		{
			VertexType = vertexType;
			Count = count;
			Normalized = normalized;

			switch (VertexType)
			{
				case VertexType.UnsignedByte:
					Size = count * sizeof(byte);
					break;
				case VertexType.Byte:
					Size = count * sizeof(sbyte);
					break;
				case VertexType.UnsignedShort:
					Size = count * sizeof(ushort);
					break;
				case VertexType.Short:
					Size = count * sizeof(short);
					break;
				case VertexType.UnsignedInt:
					Size = count * sizeof(uint);
					break;
				case VertexType.Int:
					Size = count * sizeof(int);
					break;
				case VertexType.Float:
					Size = count * sizeof(float);
					break;
				default:
					throw new NotImplementedException("Unknown VertexType");
			}
		}
	};

	internal interface IVertex
	{
		VertexAttributeLayout AttributeLayout { get; }
	}

	internal class VertexAttributeLayout
	{
		private VertexAttribute[] attributes;

		internal GLint Stride { get; private set; }
		internal VertexAttribute[] Attributes { get { return (VertexAttribute[])attributes.Clone(); } }

		internal VertexAttributeLayout(params VertexAttribute[] attributes) : this(0, attributes) { }
		internal VertexAttributeLayout(int stride, params VertexAttribute[] attributes)
		{
			if (attributes == null || attributes.Length == 0)
				throw new ArgumentNullException("attributes", "At least 1 attribute is required");

			this.attributes = (VertexAttribute[])attributes.Clone();

			if (stride > 0)
				Stride = stride;
			else
				for (int i = 0; i < attributes.Length; i++)
					Stride += attributes[i].Size;
		}

		internal void SetVertexAttributes()
		{
			GLint offset = 0;
			for (GLuint i = 0; i < attributes.Length; i++)
			{
				GL.EnableVertexAttribArray(i);
				GL.VertexAttribPointer(i, attributes[(int)i].Count, attributes[(int)i].VertexType, attributes[(int)i].Normalized, Stride, offset);
				//Output.WriteLine($"id: {i}, Count: {attributes[(int)i].Count}, Type: {attributes[(int)i].VertexType}, Normalized: {attributes[(int)i].Normalized}, Size/Stride: {attributes[(int)i].Size}/{Stride}, offset: {offset}"); // Debug
				offset += attributes[(int)i].Size;
			}
		}
	}
}