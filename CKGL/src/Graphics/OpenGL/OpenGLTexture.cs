using CKGL.OpenGLBindings;
using GLint = System.Int32;
using GLuint = System.UInt32;

namespace CKGL.OpenGL
{
	public class OpenGLTexture : Texture
	{
		internal GLuint ID => id;
		internal TextureTarget TextureTarget;

		private GLuint id;

		private struct Binding
		{
			public GLuint ID;
			public TextureTarget Target;
		}
		private static readonly Binding[] bindings = new Binding[GL.MaxTextureImageUnits];

		internal OpenGLTexture(byte[] data, TextureType type,
							   int width, int height, int depth,
							   TextureFormat format,
							   TextureFilter minFilter, TextureFilter magFilter,
							   TextureWrap wrapX, TextureWrap wrapY)
		{
			Type = type;
			Width = width;
			Height = height;
			Depth = depth;
			Format = format;
			id = GL.GenTexture();
			switch (Type)
			{
				case TextureType.Texture1D when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
					TextureTarget = TextureTarget.Texture1D;
					break;
				case TextureType.Texture1DArray when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
					TextureTarget = TextureTarget.Texture1DArray;
					break;
				case TextureType.Texture2D:
					TextureTarget = TextureTarget.Texture2D;
					break;
				case TextureType.Texture2DArray:
					TextureTarget = TextureTarget.Texture2DArray;
					break;
				case TextureType.Texture2DMultisample:
					TextureTarget = TextureTarget.Texture2DMultisample;
					break;
				case TextureType.Texture3D:
					TextureTarget = TextureTarget.Texture3D;
					break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
			MinFilter = minFilter;
			MagFilter = magFilter;
			WrapX = wrapX;
			WrapY = wrapY;

			SetData(data);
		}

		private unsafe void SetData(byte[] data)
		{
			switch (Type)
			{
				//case TextureType.Texture1D when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				//case TextureType.Texture1DArray when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				case TextureType.Texture2D:
					if (data != null && data.Length < Width * Height * Format.ToOpenGL().PixelFormat().Components())
						throw new CKGLException("Data array is not large enough to fill texture.");
					Bind();
					fixed (byte* ptr = data)
						GL.TexImage2D(TextureTarget, 0, Format.ToOpenGL(), Width, Height, 0, Format.ToOpenGL().PixelFormat(), Format.ToOpenGL().PixelType(), data != null ? ptr : null);
					break;
				//case TextureType.Texture2DArray:
				//	break;
				//case TextureType.Texture2DMultisample:
				//	break;
				//case TextureType.Texture3D:
				//	break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
		}

		public override void Destroy()
		{
			if (id != default)
			{
				GL.DeleteTexture(id);
				id = default;
			}
		}

		public override unsafe byte[] GetData()
		{
			switch (Type)
			{
				//case TextureType.Texture1D when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				//case TextureType.Texture1DArray when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				case TextureType.Texture2D:
					byte[] data = new byte[Width * Height * Format.ToOpenGL().PixelFormat().Components()];
					Bind();
					fixed (byte* ptr = data)
						GL.GetTexImage(TextureTarget, 0, Format.ToOpenGL().PixelFormat(), Format.ToOpenGL().PixelType(), ptr);
					return data;
				//case TextureType.Texture2DArray:
				//	break;
				//case TextureType.Texture2DMultisample:
				//	break;
				//case TextureType.Texture3D:
				//	break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
		}

		public override unsafe Bitmap GetBitmap()
		{
			switch (Type)
			{
				//case TextureType.Texture1D when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				//case TextureType.Texture1DArray when Platform.GraphicsBackend == GraphicsBackend.OpenGL: // Not available in OpenGL ES
				//	break;
				case TextureType.Texture2D:
					Colour[] data = new Colour[Width * Height];
					Bind();
					fixed (Colour* ptr = data)
						GL.GetTexImage(TextureTarget, 0, Format.ToOpenGL().PixelFormat(), Format.ToOpenGL().PixelType(), ptr);
					return new Bitmap(data, Width, Height);
				//case TextureType.Texture2DArray:
				//	break;
				//case TextureType.Texture2DMultisample:
				//	break;
				//case TextureType.Texture3D:
				//	break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
		}

		#region Parameters
		protected override TextureWrap Wrap
		{
			set
			{
				WrapX = value;
				WrapY = value;
			}
		}

		protected override TextureWrap WrapX
		{
			get { return (TextureWrap)GetParam(TextureParam.WrapS); }
			set { SetParam(TextureParam.WrapS, (GLint)value.ToOpenGL()); }
		}

		protected override TextureWrap WrapY
		{
			get { return (TextureWrap)GetParam(TextureParam.WrapT); }
			set { SetParam(TextureParam.WrapT, (GLint)value.ToOpenGL()); }
		}

		protected override TextureFilter Filter
		{
			set
			{
				MinFilter = value;
				MagFilter = value;
			}
		}

		protected override TextureFilter MinFilter
		{
			get { return (TextureFilter)GetParam(TextureParam.MinFilter); }
			set { SetParam(TextureParam.MinFilter, (GLint)value.ToOpenGL()); }
		}

		protected override TextureFilter MagFilter
		{
			get { return (TextureFilter)GetParam(TextureParam.MagFilter); }
			set
			{
				switch (value)
				{
					case TextureFilter.Linear:
					case TextureFilter.LinearMipmapLinear:
					case TextureFilter.LinearMipmapNearest:
						SetParam(TextureParam.MagFilter, (GLint)TextureFilter.Linear.ToOpenGL());
						break;
					case TextureFilter.Nearest:
					case TextureFilter.NearestMipmapLinear:
					case TextureFilter.NearestMipmapNearest:
						SetParam(TextureParam.MagFilter, (GLint)TextureFilter.Nearest.ToOpenGL());
						break;
				}
			}
		}

		private int GetParam(TextureParam param)
		{
			Bind();
			GL.GetTexParameterI(TextureTarget, param, out GLint val);
			return val;
		}

		private void SetParam(TextureParam param, GLint val)
		{
			Bind();
			GL.TexParameterI(TextureTarget, param, val);
		}
		#endregion

		#region Bind
		public override void Bind() => Bind(0);
		public override void Bind(GLuint textureSlot)
		{
			if (id != bindings[textureSlot].ID || TextureTarget != bindings[textureSlot].Target)
			{
				Graphics.State.OnStateChanging?.Invoke();
				GL.ActiveTexture(textureSlot);
				GL.BindTexture(TextureTarget, id);
				Swaps++;

				bindings[textureSlot].ID = id;
				bindings[textureSlot].Target = TextureTarget;
				Graphics.State.OnStateChanged?.Invoke();
			}
		}
		#endregion

		#region Overrides
		public override string ToString()
		{
			return $"Texture: [id: {id}]";
		}

		public override bool Equals(object obj)
		{
			return obj is OpenGLTexture && Equals((OpenGLTexture)obj);
		}
		public override bool Equals(Texture texture)
		{
			return this == (OpenGLTexture)texture;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + id.GetHashCode();
				return hash;
			}
		}
		#endregion

		#region Operators
		public static bool operator ==(OpenGLTexture a, OpenGLTexture b)
		{
			return (a?.id ?? null) == (b?.id ?? null);
		}

		public static bool operator !=(OpenGLTexture a, OpenGLTexture b)
		{
			return (a?.id ?? null) != (b?.id ?? null);
		}
		#endregion
	}
}