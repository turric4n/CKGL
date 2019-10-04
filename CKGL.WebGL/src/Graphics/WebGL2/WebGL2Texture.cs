using static CKGL.WebGL2.WebGL2Graphics; // WebGL 2.0 Context Methods
using static Retyped.dom; // DOM / WebGL Types
using static Retyped.es5; // JS TypedArrays
using static Retyped.webgl2.WebGL2RenderingContext; // WebGL 2.0 Enums

namespace CKGL.WebGL2
{
	public class WebGL2Texture : Texture
	{
		internal WebGLTexture ID => id;
		internal double TextureTarget;

		private WebGLTexture id;

		private struct Binding
		{
			public WebGLTexture ID;
			public double Target;
		}
		//private static readonly Binding[] bindings = new Binding[GL.MaxTextureImageUnits];
		private static readonly Binding[] bindings = new Binding[32];

		internal WebGL2Texture(byte[] data, TextureType type,
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
			id = GL.createTexture();
			switch (Type)
			{
				case TextureType.Texture2D:
					TextureTarget = TEXTURE_2D;
					break;
				case TextureType.Texture2DArray:
					TextureTarget = TEXTURE_2D_ARRAY_Static;
					break;
				case TextureType.Texture3D:
					TextureTarget = TEXTURE_3D_Static;
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

		private void SetData(byte[] data)
		{
			switch (Type)
			{
				case TextureType.Texture2D:
					if (data != null && data.Length < Width * Height * Format.Components())
						throw new CKGLException("Data array is not large enough to fill texture.");
					Bind();
					//fixed (byte* ptr = data)
					//	GL.TexImage2D(TextureTarget, 0, Format.ToWebGL2(), Width, Height, 0, Format.ToWebGL2().PixelFormat(), Format.ToWebGL2().PixelType(), data != null ? ptr : null);
					GL.texImage2D(TextureTarget, 0, Format.ToWebGL2(), Width, Height, 0, Format.ToWebGL2PixelFormat(), Format.ToWebGL2PixelType(), null as ImageData);
					break;
				//case TextureType.Texture2DArray: // Not available in WebGL 1.0
				//	break;
				//case TextureType.Texture3D: // Not available in WebGL 1.0
				//	break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
		}

		public override void Destroy()
		{
			if (id != null)
			{
				GL.deleteTexture(id);
				id = null;
			}
		}

		public override byte[] GetData()
		{
			switch (Type)
			{
				case TextureType.Texture2D:
					byte[] data = new byte[Width * Height * Format.Components()];
					Bind();
					//fixed (byte* ptr = data)
					//	GL.GetTexImage(TextureTarget, 0, Format.ToWebGLPixelFormat(), Format.ToWebGLPixelType(), ptr);
					return data;
				//case TextureType.Texture2DArray: // Not available in WebGL 1.0
				//	break;
				//case TextureType.Texture3D: // Not available in WebGL 1.0
				//	break;
				default:
					throw new IllegalValueException(typeof(TextureType), Type);
			}
		}

		public override Bitmap GetBitmap()
		{
			switch (Type)
			{
				case TextureType.Texture2D:
					Colour[] data = new Colour[Width * Height];
					Bind();
					//fixed (Colour* ptr = data)
					//	GL.GetTexImage(TextureTarget, 0, Format.ToWebGLPixelFormat(), Format.ToWebGLPixelType(), ptr);
					return new Bitmap(data, Width, Height);
				//case TextureType.Texture2DArray: // Not available in WebGL 1.0
				//	break;
				//case TextureType.Texture3D: // Not available in WebGL 1.0
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
			get { return (TextureWrap)GetParam(TEXTURE_WRAP_S); }
			set { SetParam(TEXTURE_WRAP_S, value.ToWebGL2()); }
		}

		protected override TextureWrap WrapY
		{
			get { return (TextureWrap)GetParam(TEXTURE_WRAP_T); }
			set { SetParam(TEXTURE_WRAP_T, value.ToWebGL2()); }
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
			get { return (TextureFilter)GetParam(TEXTURE_MIN_FILTER); }
			set { SetParam(TEXTURE_MIN_FILTER, value.ToWebGL2()); }
		}

		protected override TextureFilter MagFilter
		{
			get { return (TextureFilter)GetParam(TEXTURE_MIN_FILTER); }
			set
			{
				switch (value)
				{
					case TextureFilter.Linear:
					case TextureFilter.LinearMipmapLinear:
					case TextureFilter.LinearMipmapNearest:
						SetParam(TEXTURE_MIN_FILTER, TextureFilter.Linear.ToWebGL2());
						break;
					case TextureFilter.Nearest:
					case TextureFilter.NearestMipmapLinear:
					case TextureFilter.NearestMipmapNearest:
						SetParam(TEXTURE_MIN_FILTER, TextureFilter.Nearest.ToWebGL2());
						break;
				}
			}
		}

		private double GetParam(double param)
		{
			Bind();
			return (double)GL.getTexParameter(TextureTarget, param);
		}

		private void SetParam(double param, double val)
		{
			Bind();
			GL.texParameteri(TextureTarget, param, val);
		}
		#endregion

		#region Bind
		public override void Bind() => Bind(0);
		public override void Bind(uint textureSlot)
		{
			if (id != bindings[textureSlot].ID || TextureTarget != bindings[textureSlot].Target)
			{
				Graphics.State.OnStateChanging?.Invoke();
				GL.activeTexture(textureSlot);
				GL.bindTexture(TextureTarget, id);
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
			return obj is WebGL2Texture && Equals((WebGL2Texture)obj);
		}
		public override bool Equals(Texture texture)
		{
			return this == (WebGL2Texture)texture;
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
		public static bool operator ==(WebGL2Texture a, WebGL2Texture b)
		{
			return (a?.id ?? null) == (b?.id ?? null);
		}

		public static bool operator !=(WebGL2Texture a, WebGL2Texture b)
		{
			return (a?.id ?? null) != (b?.id ?? null);
		}
		#endregion
	}
}