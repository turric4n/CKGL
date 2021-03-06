// Right-handed, Row-Major, 4x4 Matrix for 3D Transformations

/*
 * Partially Derived from:
 * Matrix.cs in the MonoGame Library: https://github.com/MonoGame/MonoGame
 * Matrix2D.cs in the Nez Library: https://github.com/prime31/Nez/
 * Matrix2D.cs in the MonoGame.Extended Library: https://github.com/craftworkgames/MonoGame.Extended
 * Matrix4x4.cs and Matrix3x2.cs in the Rise Library: https://github.com/ChevyRay/Rise
 */

using System;
using System.Runtime.CompilerServices;

namespace CKGL
{
	public struct Matrix
	{
		// Column-Major Storage Order
		public float M11; // X Scale		// Y/Z Rotation
		public float M12;                   // Z   Rotation		// X Axis - Y Shear
		public float M13;                   // Y   Rotation		// X Axis - Z Shear
		public float M14;                                                               // Always 0
		public float M21;                   // Z   Rotation		// Y Axis - X Shear
		public float M22; // Y Scale		// X/Z Rotation
		public float M23;                   // X   Rotation		// Y Axis - Z Shear
		public float M24;                                                               // Always 0
		public float M31;                   // Y   Rotation		// Z Axis - X Shear
		public float M32;                   // X   Rotation		// Z Axis - Y Shear
		public float M33; // Z Scale		// X/Y Rotation
		public float M34;                                                               // Always 0
		public float M41; // X Translation
		public float M42; // Y Translation
		public float M43; // Z Translation
		public float M44;                                                               // Always 1

		#region Constructors
		public Matrix(float m11, float m12, float m13, float m14,
					  float m21, float m22, float m23, float m24,
					  float m31, float m32, float m33, float m34,
					  float m41, float m42, float m43, float m44)
		{
			M11 = m11; M12 = m12; M13 = m13; M14 = m14;
			M21 = m21; M22 = m22; M23 = m23; M24 = m24;
			M31 = m31; M32 = m32; M33 = m33; M34 = m34;
			M41 = m41; M42 = m42; M43 = m43; M44 = m44;
		}
		#endregion

		#region Static Constructors
		public static readonly Matrix Identity = new Matrix(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
		#endregion

		#region Properties
		public Vector3 Backward
		{
			get
			{
				return new Vector3(M31, M32, M33);
			}
			set
			{
				M31 = value.X;
				M32 = value.Y;
				M33 = value.Z;
			}
		}

		public Vector3 Forward
		{
			get
			{
				return new Vector3(-M31, -M32, -M33);
			}
			set
			{
				M31 = -value.X;
				M32 = -value.Y;
				M33 = -value.Z;
			}
		}

		public Vector3 Down
		{
			get
			{
				return new Vector3(-M21, -M22, -M23);
			}
			set
			{
				M21 = -value.X;
				M22 = -value.Y;
				M23 = -value.Z;
			}
		}

		public Vector3 Up
		{
			get
			{
				return new Vector3(M21, M22, M23);
			}
			set
			{
				M21 = value.X;
				M22 = value.Y;
				M23 = value.Z;
			}
		}

		public Vector3 Left
		{
			get
			{
				return new Vector3(-M11, -M12, -M13);
			}
			set
			{
				M11 = -value.X;
				M12 = -value.Y;
				M13 = -value.Z;
			}
		}

		public Vector3 Right
		{
			get
			{
				return new Vector3(M11, M12, M13);
			}
			set
			{
				M11 = value.X;
				M12 = value.Y;
				M13 = value.Z;
			}
		}

		public Vector3 Translation
		{
			get
			{
				return new Vector3(M41, M42, M43);
			}
			set
			{
				M41 = value.X;
				M42 = value.Y;
				M43 = value.Z;
			}
		}
		#endregion

		#region Methods
#if !WEBGL
		public float[] ToArrayRowMajor()
		{
			float[] array = {
				M11, M12, M13, M14,
				M21, M22, M23, M24,
				M31, M32, M33, M34,
				M41, M42, M43, M44
			};
			return array;
		}
		public float[] ToArrayColumnMajor()
		{
			float[] array = {
				M11, M21, M31, M41,
				M12, M22, M32, M42,
				M13, M23, M33, M43,
				M14, M24, M34, M44
			};
			return array;
		}
#elif WEBGL
		public double[] ToArrayRowMajor()
		{
			double[] array = {
				M11, M12, M13, M14,
				M21, M22, M23, M24,
				M31, M32, M33, M34,
				M41, M42, M43, M44
			};
			return array;
		}
		public double[] ToArrayColumnMajor()
		{
			double[] array = {
				M11, M21, M31, M41,
				M12, M22, M32, M42,
				M13, M23, M33, M43,
				M14, M24, M34, M44
			};
			return array;
		}
#endif

		public Matrix2D ToMatrix2D()
		{
			return new Matrix2D(M11, M12,
								M21, M22,
								M41, M42);
		}

		public Matrix3x3 ToMatrix3x3()
		{
			return new Matrix3x3(M11, M12, M13,
								 M21, M22, M23,
								 M31, M32, M33);
		}

		public float Determinant()
		{
			float num22 = M11;
			float num21 = M12;
			float num20 = M13;
			float num19 = M14;
			float num12 = M21;
			float num11 = M22;
			float num10 = M23;
			float num9 = M24;
			float num8 = M31;
			float num7 = M32;
			float num6 = M33;
			float num5 = M34;
			float num4 = M41;
			float num3 = M42;
			float num2 = M43;
			float num = M44;
			float num18 = (num6 * num) - (num5 * num2);
			float num17 = (num7 * num) - (num5 * num3);
			float num16 = (num7 * num2) - (num6 * num3);
			float num15 = (num8 * num) - (num5 * num4);
			float num14 = (num8 * num2) - (num6 * num4);
			float num13 = (num8 * num3) - (num7 * num4);
			return ((((num22 * (((num11 * num18) - (num10 * num17)) + (num9 * num16))) - (num21 * (((num12 * num18) - (num10 * num15)) + (num9 * num14)))) + (num20 * (((num12 * num17) - (num11 * num15)) + (num9 * num13)))) - (num19 * (((num12 * num16) - (num11 * num14)) + (num10 * num13))));
		}

		public Matrix Transpose()
		{
			return new Matrix(M11, M21, M31, M41,
							  M12, M22, M32, M42,
							  M13, M23, M33, M43,
							  M14, M24, M34, M44);
		}

		public Matrix Inverse()
		{
			Matrix result = Identity;

			float num1 = M11;
			float num2 = M12;
			float num3 = M13;
			float num4 = M14;
			float num5 = M21;
			float num6 = M22;
			float num7 = M23;
			float num8 = M24;
			float num9 = M31;
			float num10 = M32;
			float num11 = M33;
			float num12 = M34;
			float num13 = M41;
			float num14 = M42;
			float num15 = M43;
			float num16 = M44;
			float num17 = (float)((double)num11 * (double)num16 - (double)num12 * (double)num15);
			float num18 = (float)((double)num10 * (double)num16 - (double)num12 * (double)num14);
			float num19 = (float)((double)num10 * (double)num15 - (double)num11 * (double)num14);
			float num20 = (float)((double)num9 * (double)num16 - (double)num12 * (double)num13);
			float num21 = (float)((double)num9 * (double)num15 - (double)num11 * (double)num13);
			float num22 = (float)((double)num9 * (double)num14 - (double)num10 * (double)num13);
			float num23 = (float)((double)num6 * (double)num17 - (double)num7 * (double)num18 + (double)num8 * (double)num19);
			float num24 = (float)-((double)num5 * (double)num17 - (double)num7 * (double)num20 + (double)num8 * (double)num21);
			float num25 = (float)((double)num5 * (double)num18 - (double)num6 * (double)num20 + (double)num8 * (double)num22);
			float num26 = (float)-((double)num5 * (double)num19 - (double)num6 * (double)num21 + (double)num7 * (double)num22);
			float num27 = (float)(1.0 / ((double)num1 * (double)num23 + (double)num2 * (double)num24 + (double)num3 * (double)num25 + (double)num4 * (double)num26));

			result.M11 = num23 * num27;
			result.M21 = num24 * num27;
			result.M31 = num25 * num27;
			result.M41 = num26 * num27;
			result.M12 = (float)-((double)num2 * (double)num17 - (double)num3 * (double)num18 + (double)num4 * (double)num19) * num27;
			result.M22 = (float)((double)num1 * (double)num17 - (double)num3 * (double)num20 + (double)num4 * (double)num21) * num27;
			result.M32 = (float)-((double)num1 * (double)num18 - (double)num2 * (double)num20 + (double)num4 * (double)num22) * num27;
			result.M42 = (float)((double)num1 * (double)num19 - (double)num2 * (double)num21 + (double)num3 * (double)num22) * num27;
			float num28 = (float)((double)num7 * (double)num16 - (double)num8 * (double)num15);
			float num29 = (float)((double)num6 * (double)num16 - (double)num8 * (double)num14);
			float num30 = (float)((double)num6 * (double)num15 - (double)num7 * (double)num14);
			float num31 = (float)((double)num5 * (double)num16 - (double)num8 * (double)num13);
			float num32 = (float)((double)num5 * (double)num15 - (double)num7 * (double)num13);
			float num33 = (float)((double)num5 * (double)num14 - (double)num6 * (double)num13);
			result.M13 = (float)((double)num2 * (double)num28 - (double)num3 * (double)num29 + (double)num4 * (double)num30) * num27;
			result.M23 = (float)-((double)num1 * (double)num28 - (double)num3 * (double)num31 + (double)num4 * (double)num32) * num27;
			result.M33 = (float)((double)num1 * (double)num29 - (double)num2 * (double)num31 + (double)num4 * (double)num33) * num27;
			result.M43 = (float)-((double)num1 * (double)num30 - (double)num2 * (double)num32 + (double)num3 * (double)num33) * num27;
			float num34 = (float)((double)num7 * (double)num12 - (double)num8 * (double)num11);
			float num35 = (float)((double)num6 * (double)num12 - (double)num8 * (double)num10);
			float num36 = (float)((double)num6 * (double)num11 - (double)num7 * (double)num10);
			float num37 = (float)((double)num5 * (double)num12 - (double)num8 * (double)num9);
			float num38 = (float)((double)num5 * (double)num11 - (double)num7 * (double)num9);
			float num39 = (float)((double)num5 * (double)num10 - (double)num6 * (double)num9);
			result.M14 = (float)-((double)num2 * (double)num34 - (double)num3 * (double)num35 + (double)num4 * (double)num36) * num27;
			result.M24 = (float)((double)num1 * (double)num34 - (double)num3 * (double)num37 + (double)num4 * (double)num38) * num27;
			result.M34 = (float)-((double)num1 * (double)num35 - (double)num2 * (double)num37 + (double)num4 * (double)num39) * num27;
			result.M44 = (float)((double)num1 * (double)num36 - (double)num2 * (double)num38 + (double)num3 * (double)num39) * num27;

			return result;
		}
		#endregion

		#region Static Methods
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Matrix CreateTransform(Vector3 origin, Vector3 position, Quaternion rotation, Vector3 scale, Shear3D shear)
		{
			Matrix transformMatrix = Identity;

			if (origin != Vector3.Zero)
				transformMatrix *= CreateTranslation(-origin);

			if (shear != Shear3D.Identity)
				transformMatrix *= CreateShear(shear);

			if (scale != Vector3.One)
				transformMatrix *= CreateScale(scale);

			if (rotation != Quaternion.Identity)
				transformMatrix *= rotation.Matrix;

			if (position != Vector3.Zero)
			{
				if (origin != Vector3.Zero)
					transformMatrix *= CreateTranslation(position - origin);
				else
					transformMatrix *= CreateTranslation(position);
			}

			if (origin != Vector3.Zero)
				transformMatrix *= CreateTranslation(origin);

			return transformMatrix;
		}

		public static Matrix CreateFromAxisAngle(Vector3 axis, float rotations)
		{
			Matrix result = Identity;

			float x = axis.X;
			float y = axis.Y;
			float z = axis.Z;
			float num2 = Math.Sin(rotations.RotationsToRadians());
			float num = Math.Cos(rotations.RotationsToRadians());
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;
			result.M11 = num11 + (num * (1f - num11));
			result.M12 = (num8 - (num * num8)) + (num2 * z); // Handedness Switch
			result.M13 = (num7 - (num * num7)) - (num2 * y); // Handedness Switch
			result.M14 = 0;
			result.M21 = (num8 - (num * num8)) - (num2 * z); // Handedness Switch
			result.M22 = num10 + (num * (1f - num10));
			result.M23 = (num6 - (num * num6)) + (num2 * x); // Handedness Switch
			result.M24 = 0;
			result.M31 = (num7 - (num * num7)) + (num2 * y); // Handedness Switch
			result.M32 = (num6 - (num * num6)) - (num2 * x); // Handedness Switch
			result.M33 = num9 + (num * (1f - num9));
			result.M34 = 0;
			result.M41 = 0;
			result.M42 = 0;
			result.M43 = 0;
			result.M44 = 1;

			return result;
		}

		public static Matrix CreateFromEuler(Vector3 euler) => CreateFromEuler(euler.X, euler.Y, euler.Z);
		public static Matrix CreateFromEuler(float x, float y, float z)
		{
			return Quaternion.CreateFromEuler(x, y, z).Matrix;
		}

		public static Matrix CreateLookAt(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 up)
		{
			Matrix result = Identity;

			var lookForward = Vector3.Normalize(cameraTarget - cameraPosition);
			var lookRight = Vector3.Normalize(Vector3.Cross(up, lookForward));
			var lookUp = Vector3.Cross(lookForward, lookRight);
			result.M11 = lookRight.X;
			result.M12 = lookUp.X;
			result.M13 = lookForward.X;
			result.M14 = 0f;
			result.M21 = lookRight.Y;
			result.M22 = lookUp.Y;
			result.M23 = lookForward.Y;
			result.M24 = 0f;
			result.M31 = lookRight.Z;
			result.M32 = lookUp.Z;
			result.M33 = lookForward.Z;
			result.M34 = 0f;
			result.M41 = -Vector3.Dot(lookRight, cameraPosition);
			result.M42 = -Vector3.Dot(lookUp, cameraPosition);
			result.M43 = -Vector3.Dot(lookForward, cameraPosition);
			result.M44 = 1f;

			return result;
		}

		public static Matrix CreateOrthographic(Point2 size, float zNearPlane, float zFarPlane)
		{
			return CreateOrthographic(size.X, size.Y, zNearPlane, zFarPlane);
		}
		public static Matrix CreateOrthographic(float width, float height, float zNearPlane, float zFarPlane)
		{
			Matrix result = Identity;

			result.M11 = 2f / width;
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 2f / height;
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M34 = 0f;
			result.M41 = 0f;
			result.M42 = 0f;
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;

			return result;
		}

		public static Matrix CreateOrthographicOffCenter(RectangleI viewingVolume, float zNearPlane, float zFarPlane)
		{
			return CreateOrthographicOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, zNearPlane, zFarPlane);
		}
		public static Matrix CreateOrthographicOffCenter(float left, float right, float bottom, float top, float zNearPlane, float zFarPlane)
		{
			Matrix result = Identity;

			result.M11 = 2f / (right - left);
			result.M12 = 0f;
			result.M13 = 0f;
			result.M14 = 0f;
			result.M21 = 0f;
			result.M22 = 2f / (top - bottom);
			result.M23 = 0f;
			result.M24 = 0f;
			result.M31 = 0f;
			result.M32 = 0f;
			result.M33 = 1f / (zNearPlane - zFarPlane);
			result.M34 = 0f;
			result.M41 = (left + right) / (left - right);
			result.M42 = (bottom + top) / (bottom - top);
			result.M43 = zNearPlane / (zNearPlane - zFarPlane);
			result.M44 = 1f;

			return result;
		}

		public static Matrix CreatePerspective(float width, float height, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result = Identity;

			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}
			result.M11 = (2f * nearPlaneDistance) / width;
			result.M12 = result.M13 = result.M14 = 0f;
			result.M22 = (2f * nearPlaneDistance) / height;
			result.M21 = result.M23 = result.M24 = 0f;
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance); // Handedness Switch
			result.M31 = result.M32 = 0f;
			result.M34 = -1; // Handedness Switch
			result.M41 = result.M42 = result.M44 = 0f;
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance); // Handedness Switch

			return result;
		}

		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result = Identity;

			if ((fieldOfView <= 0f) || (fieldOfView >= Math.PI))
			{
				throw new ArgumentException("fieldOfView <= 0 or >= PI");
			}
			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}
			float num = 1f / (Math.Tan(fieldOfView * 0.5f));
			float num9 = num / aspectRatio;
			result.M11 = num9;
			result.M12 = result.M13 = result.M14 = 0;
			result.M22 = num;
			result.M21 = result.M23 = result.M24 = 0;
			result.M31 = result.M32 = 0f;
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance); // Handedness Switch
			result.M34 = -1; // Handedness Switch
			result.M41 = result.M42 = result.M44 = 0;
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance); // Handedness Switch

			return result;
		}

		public static Matrix CreatePerspectiveOffCenter(RectangleI viewingVolume, float nearPlaneDistance, float farPlaneDistance)
		{
			return CreatePerspectiveOffCenter(viewingVolume.Left, viewingVolume.Right, viewingVolume.Bottom, viewingVolume.Top, nearPlaneDistance, farPlaneDistance);
		}
		public static Matrix CreatePerspectiveOffCenter(float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance)
		{
			Matrix result = Identity;

			if (nearPlaneDistance <= 0f)
			{
				throw new ArgumentException("nearPlaneDistance <= 0");
			}
			if (farPlaneDistance <= 0f)
			{
				throw new ArgumentException("farPlaneDistance <= 0");
			}
			if (nearPlaneDistance >= farPlaneDistance)
			{
				throw new ArgumentException("nearPlaneDistance >= farPlaneDistance");
			}
			result.M11 = (2f * nearPlaneDistance) / (right - left);
			result.M12 = result.M13 = result.M14 = 0;
			result.M22 = (2f * nearPlaneDistance) / (top - bottom);
			result.M21 = result.M23 = result.M24 = 0;
			result.M31 = (left + right) / (right - left);
			result.M32 = (top + bottom) / (top - bottom);
			result.M33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance); // Handedness Switch
			result.M34 = -1; // Handedness Switch
			result.M43 = (nearPlaneDistance * farPlaneDistance) / (nearPlaneDistance - farPlaneDistance); // Handedness Switch
			result.M41 = result.M42 = result.M44 = 0;

			return result;
		}

		public static Matrix CreateRotationX(float rotations)
		{
			Matrix result = Identity;

			var cos = Math.Cos(rotations.RotationsToRadians());
			var sin = Math.Sin(rotations.RotationsToRadians());

			result.M22 = cos;
			result.M23 = sin; // Handedness Switch
			result.M32 = -sin; // Handedness Switch
			result.M33 = cos;

			return result;
		}

		public static Matrix CreateRotationY(float rotations)
		{
			Matrix result = Identity;

			var cos = Math.Cos(rotations.RotationsToRadians());
			var sin = Math.Sin(rotations.RotationsToRadians());

			result.M11 = cos;
			result.M13 = -sin; // Handedness Switch
			result.M31 = sin; // Handedness Switch
			result.M33 = cos;

			return result;
		}

		public static Matrix CreateRotationZ(float rotations)
		{
			Matrix result = Identity;

			var cos = Math.Cos(rotations.RotationsToRadians());
			var sin = Math.Sin(rotations.RotationsToRadians());

			result.M11 = cos;
			result.M12 = sin; // Handedness Switch
			result.M21 = -sin; // Handedness Switch
			result.M22 = cos;

			return result;
		}

		public static Matrix CreateScale(Vector3 scale)
		{
			return CreateScale(scale.X, scale.Y, scale.Z);
		}
		public static Matrix CreateScale(float scale)
		{
			return CreateScale(scale, scale, scale);
		}
		public static Matrix CreateScale(float scaleX, float scaleY, float scaleZ)
		{
			Matrix result = Identity;

			result.M11 = scaleX;
			result.M12 = 0;
			result.M13 = 0;
			result.M14 = 0;
			result.M21 = 0;
			result.M22 = scaleY;
			result.M23 = 0;
			result.M24 = 0;
			result.M31 = 0;
			result.M32 = 0;
			result.M33 = scaleZ;
			result.M34 = 0;
			result.M41 = 0;
			result.M42 = 0;
			result.M43 = 0;
			result.M44 = 1;

			return result;
		}

		public static Matrix CreateShear(Shear3D shear)
		{
			Matrix result = Identity;

			result.M11 = 1;
			result.M12 = shear.XY;
			result.M13 = shear.XZ;
			result.M14 = 0;
			result.M21 = shear.YX;
			result.M22 = 1;
			result.M23 = shear.YZ;
			result.M24 = 0;
			result.M31 = shear.ZX;
			result.M32 = shear.ZY;
			result.M33 = 1;
			result.M34 = 0;
			result.M41 = 0;
			result.M42 = 0;
			result.M43 = 0;
			result.M44 = 1;

			return result;
		}

		public static Matrix CreateTranslation(Vector3 position)
		{
			return CreateTranslation(position.X, position.Y, position.Z);
		}
		public static Matrix CreateTranslation(float positionX, float positionY, float positionZ)
		{
			Matrix result = Identity;

			result.M11 = 1;
			result.M12 = 0;
			result.M13 = 0;
			result.M14 = 0;
			result.M21 = 0;
			result.M22 = 1;
			result.M23 = 0;
			result.M24 = 0;
			result.M31 = 0;
			result.M32 = 0;
			result.M33 = 1;
			result.M34 = 0;
			result.M41 = positionX;
			result.M42 = positionY;
			result.M43 = positionZ;
			result.M44 = 1;

			return result;
		}
		#endregion

		#region Overrides
		public override string ToString()
		{
			return $"{{M11:{M11} M12:{M12} M13:{M13} M14:{M14}}} {{M21:{M21} M22:{M22} M23:{M23} M24:{M24}}} {{M31:{M31} M32:{M32} M33:{M33} M34:{M34}}} {{M41:{M41} M42:{M42} M43:{M43} M44:{M44}}}";
		}

		public override bool Equals(object obj)
		{
			return obj is Matrix && Equals((Matrix)obj);
		}
		public bool Equals(Matrix matrix)
		{
			return this == matrix;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hash = 17;
				hash = hash * 23 + M11.GetHashCode();
				hash = hash * 23 + M12.GetHashCode();
				hash = hash * 23 + M13.GetHashCode();
				hash = hash * 23 + M14.GetHashCode();
				hash = hash * 23 + M21.GetHashCode();
				hash = hash * 23 + M22.GetHashCode();
				hash = hash * 23 + M23.GetHashCode();
				hash = hash * 23 + M24.GetHashCode();
				hash = hash * 23 + M31.GetHashCode();
				hash = hash * 23 + M32.GetHashCode();
				hash = hash * 23 + M33.GetHashCode();
				hash = hash * 23 + M34.GetHashCode();
				hash = hash * 23 + M41.GetHashCode();
				hash = hash * 23 + M42.GetHashCode();
				hash = hash * 23 + M43.GetHashCode();
				hash = hash * 23 + M44.GetHashCode();
				return hash;
			}
		}
		#endregion

		#region Operators
		public static bool operator ==(Matrix a, Matrix b)
		{
			return
				a.M11 == b.M11 &&
				a.M12 == b.M12 &&
				a.M13 == b.M13 &&
				a.M14 == b.M14 &&
				a.M21 == b.M21 &&
				a.M22 == b.M22 &&
				a.M23 == b.M23 &&
				a.M24 == b.M24 &&
				a.M31 == b.M31 &&
				a.M32 == b.M32 &&
				a.M33 == b.M33 &&
				a.M34 == b.M34 &&
				a.M41 == b.M41 &&
				a.M42 == b.M42 &&
				a.M43 == b.M43 &&
				a.M44 == b.M44
				;
		}

		public static bool operator !=(Matrix a, Matrix b)
		{
			return
				a.M11 != b.M11 ||
				a.M12 != b.M12 ||
				a.M13 != b.M13 ||
				a.M14 != b.M14 ||
				a.M21 != b.M21 ||
				a.M22 != b.M22 ||
				a.M23 != b.M23 ||
				a.M24 != b.M24 ||
				a.M31 != b.M31 ||
				a.M32 != b.M32 ||
				a.M33 != b.M33 ||
				a.M34 != b.M34 ||
				a.M41 != b.M41 ||
				a.M42 != b.M42 ||
				a.M43 != b.M43 ||
				a.M44 != b.M44
				;
		}

		public static Matrix operator +(Matrix a, Matrix b)
		{
			a.M11 = a.M11 + b.M11;
			a.M12 = a.M12 + b.M12;
			a.M13 = a.M13 + b.M13;
			a.M14 = a.M14 + b.M14;
			a.M21 = a.M21 + b.M21;
			a.M22 = a.M22 + b.M22;
			a.M23 = a.M23 + b.M23;
			a.M24 = a.M24 + b.M24;
			a.M31 = a.M31 + b.M31;
			a.M32 = a.M32 + b.M32;
			a.M33 = a.M33 + b.M33;
			a.M34 = a.M34 + b.M34;
			a.M41 = a.M41 + b.M41;
			a.M42 = a.M42 + b.M42;
			a.M43 = a.M43 + b.M43;
			a.M44 = a.M44 + b.M44;
			return a;
		}

		public static Matrix operator -(Matrix a, Matrix b)
		{
			a.M11 = a.M11 - b.M11;
			a.M12 = a.M12 - b.M12;
			a.M13 = a.M13 - b.M13;
			a.M14 = a.M14 - b.M14;
			a.M21 = a.M21 - b.M21;
			a.M22 = a.M22 - b.M22;
			a.M23 = a.M23 - b.M23;
			a.M24 = a.M24 - b.M24;
			a.M31 = a.M31 - b.M31;
			a.M32 = a.M32 - b.M32;
			a.M33 = a.M33 - b.M33;
			a.M34 = a.M34 - b.M34;
			a.M41 = a.M41 - b.M41;
			a.M42 = a.M42 - b.M42;
			a.M43 = a.M43 - b.M43;
			a.M44 = a.M44 - b.M44;
			return a;
		}

		// Using matrix multiplication algorithm - see http://en.wikipedia.org/wiki/Matrix_multiplication
		public static Matrix operator *(Matrix a, Matrix b)
		{
			var m11 = (a.M11 * b.M11) + (a.M12 * b.M21) + (a.M13 * b.M31) + (a.M14 * b.M41);
			var m12 = (a.M11 * b.M12) + (a.M12 * b.M22) + (a.M13 * b.M32) + (a.M14 * b.M42);
			var m13 = (a.M11 * b.M13) + (a.M12 * b.M23) + (a.M13 * b.M33) + (a.M14 * b.M43);
			var m14 = (a.M11 * b.M14) + (a.M12 * b.M24) + (a.M13 * b.M34) + (a.M14 * b.M44);
			var m21 = (a.M21 * b.M11) + (a.M22 * b.M21) + (a.M23 * b.M31) + (a.M24 * b.M41);
			var m22 = (a.M21 * b.M12) + (a.M22 * b.M22) + (a.M23 * b.M32) + (a.M24 * b.M42);
			var m23 = (a.M21 * b.M13) + (a.M22 * b.M23) + (a.M23 * b.M33) + (a.M24 * b.M43);
			var m24 = (a.M21 * b.M14) + (a.M22 * b.M24) + (a.M23 * b.M34) + (a.M24 * b.M44);
			var m31 = (a.M31 * b.M11) + (a.M32 * b.M21) + (a.M33 * b.M31) + (a.M34 * b.M41);
			var m32 = (a.M31 * b.M12) + (a.M32 * b.M22) + (a.M33 * b.M32) + (a.M34 * b.M42);
			var m33 = (a.M31 * b.M13) + (a.M32 * b.M23) + (a.M33 * b.M33) + (a.M34 * b.M43);
			var m34 = (a.M31 * b.M14) + (a.M32 * b.M24) + (a.M33 * b.M34) + (a.M34 * b.M44);
			var m41 = (a.M41 * b.M11) + (a.M42 * b.M21) + (a.M43 * b.M31) + (a.M44 * b.M41);
			var m42 = (a.M41 * b.M12) + (a.M42 * b.M22) + (a.M43 * b.M32) + (a.M44 * b.M42);
			var m43 = (a.M41 * b.M13) + (a.M42 * b.M23) + (a.M43 * b.M33) + (a.M44 * b.M43);
			var m44 = (a.M41 * b.M14) + (a.M42 * b.M24) + (a.M43 * b.M34) + (a.M44 * b.M44);
			a.M11 = m11;
			a.M12 = m12;
			a.M13 = m13;
			a.M14 = m14;
			a.M21 = m21;
			a.M22 = m22;
			a.M23 = m23;
			a.M24 = m24;
			a.M31 = m31;
			a.M32 = m32;
			a.M33 = m33;
			a.M34 = m34;
			a.M41 = m41;
			a.M42 = m42;
			a.M43 = m43;
			a.M44 = m44;
			return a;
		}

		public static Matrix operator *(Matrix m, float n)
		{
			m.M11 = m.M11 * n;
			m.M12 = m.M12 * n;
			m.M13 = m.M13 * n;
			m.M14 = m.M14 * n;
			m.M21 = m.M21 * n;
			m.M22 = m.M22 * n;
			m.M23 = m.M23 * n;
			m.M24 = m.M24 * n;
			m.M31 = m.M31 * n;
			m.M32 = m.M32 * n;
			m.M33 = m.M33 * n;
			m.M34 = m.M34 * n;
			m.M41 = m.M41 * n;
			m.M42 = m.M42 * n;
			m.M43 = m.M43 * n;
			m.M44 = m.M44 * n;
			return m;
		}

		public static Matrix operator /(Matrix a, Matrix b)
		{
			a.M11 = a.M11 / b.M11;
			a.M12 = a.M12 / b.M12;
			a.M13 = a.M13 / b.M13;
			a.M14 = a.M14 / b.M14;
			a.M21 = a.M21 / b.M21;
			a.M22 = a.M22 / b.M22;
			a.M23 = a.M23 / b.M23;
			a.M24 = a.M24 / b.M24;
			a.M31 = a.M31 / b.M31;
			a.M32 = a.M32 / b.M32;
			a.M33 = a.M33 / b.M33;
			a.M34 = a.M34 / b.M34;
			a.M41 = a.M41 / b.M41;
			a.M42 = a.M42 / b.M42;
			a.M43 = a.M43 / b.M43;
			a.M44 = a.M44 / b.M44;
			return a;
		}

		public static Matrix operator /(Matrix m, float n)
		{
			float inverseScalar = 1f / n;
			m.M11 = m.M11 * inverseScalar;
			m.M12 = m.M12 * inverseScalar;
			m.M13 = m.M13 * inverseScalar;
			m.M14 = m.M14 * inverseScalar;
			m.M21 = m.M21 * inverseScalar;
			m.M22 = m.M22 * inverseScalar;
			m.M23 = m.M23 * inverseScalar;
			m.M24 = m.M24 * inverseScalar;
			m.M31 = m.M31 * inverseScalar;
			m.M32 = m.M32 * inverseScalar;
			m.M33 = m.M33 * inverseScalar;
			m.M34 = m.M34 * inverseScalar;
			m.M41 = m.M41 * inverseScalar;
			m.M42 = m.M42 * inverseScalar;
			m.M43 = m.M43 * inverseScalar;
			m.M44 = m.M44 * inverseScalar;
			return m;
		}

		public static Matrix operator -(Matrix m)
		{
			m.M11 = -m.M11;
			m.M12 = -m.M12;
			m.M13 = -m.M13;
			m.M14 = -m.M14;
			m.M21 = -m.M21;
			m.M22 = -m.M22;
			m.M23 = -m.M23;
			m.M24 = -m.M24;
			m.M31 = -m.M31;
			m.M32 = -m.M32;
			m.M33 = -m.M33;
			m.M34 = -m.M34;
			m.M41 = -m.M41;
			m.M42 = -m.M42;
			m.M43 = -m.M43;
			m.M44 = -m.M44;
			return m;
		}
		#endregion

		#region Implicit Convertion Operators
		public static implicit operator Matrix(Matrix2D matrix2D)
		{
			return matrix2D.ToMatrix();
		}

		public static implicit operator Matrix(Matrix3x3 matrix3x3)
		{
			return matrix3x3.ToMatrix();
		}
		#endregion
	}
}