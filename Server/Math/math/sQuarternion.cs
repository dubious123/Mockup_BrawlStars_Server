
using System.Globalization;
using System.Runtime.CompilerServices;

public struct sQuaternion : IEquatable<sQuaternion>, IFormattable
{
	//
	// 요약:
	//     X component of the Quaternion. Don't modify this directly unless you know quaternions
	//     inside out.
	public sfloat x;

	//
	// 요약:
	//     Y component of the Quaternion. Don't modify this directly unless you know quaternions
	//     inside out.
	public sfloat y;

	//
	// 요약:
	//     Z component of the Quaternion. Don't modify this directly unless you know quaternions
	//     inside out.
	public sfloat z;

	//
	// 요약:
	//     W component of the Quaternion. Do not directly modify quaternions.
	public sfloat w;

	private static readonly sQuaternion identityQuaternion = new sQuaternion(0f, 0f, 0f, 1f);

	public readonly sfloat kEpsilon = (sfloat)1E-06f;

	public sfloat this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return index switch
			{
				0 => x,
				1 => y,
				2 => z,
				3 => w,
				_ => throw new IndexOutOfRangeException("Invalid Quaternion index!"),
			};
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			switch (index)
			{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				case 2:
					z = value;
					break;
				case 3:
					w = value;
					break;
				default:
					throw new IndexOutOfRangeException("Invalid Quaternion index!");
			}
		}
	}

	//
	// 요약:
	//     The identity rotation (Read Only).
	public static sQuaternion identity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return identityQuaternion;
		}
	}

	////
	//// 요약:
	////     Returns or sets the euler angle representation of the rotation.
	//public sVector3 eulerAngles
	//{
	//	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//	get
	//	{
	//		return Internal_MakePositive(Internal_ToEulerRad(this) * (sfloat)57.29578f);
	//	}
	//	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//	set
	//	{
	//		this = Internal_FromEulerRad(value * ((sfloat)Math.PI / (sfloat)180f));
	//	}
	//}

	//
	// 요약:
	//     Returns this quaternion with a magnitude of 1 (Read Only).
	public sQuaternion normalized
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Normalize(this);
		}
	}

	//
	// 요약:
	//     Creates a rotation which rotates from fromDirection to toDirection.
	//
	// 매개 변수:
	//   fromDirection:
	//
	//   toDirection:
	//public static sQuaternion FromToRotation(sVector3 fromDirection, sVector3 toDirection)
	//{
	//	FromToRotation_Injected(ref fromDirection, ref toDirection, out var ret);
	//	return ret;
	//}

	//
	// 요약:
	//     Returns the Inverse of rotation.
	//
	// 매개 변수:
	//   rotation:

	public static sQuaternion Inverse(sQuaternion rotation)
	{
		//  -1   (       a              -v       )
		// q   = ( -------------   ------------- )
		//       (  a^2 + |v|^2  ,  a^2 + |v|^2  )

		sQuaternion ans;

		sfloat ls = rotation.x * rotation.x + rotation.y * rotation.y + rotation.z * rotation.z + rotation.w * rotation.w;
		sfloat invNorm = (sfloat)1.0f / ls;

		return new(
		ans.x = -rotation.x * invNorm,
		ans.y = -rotation.y * invNorm,
		ans.z = -rotation.z * invNorm,
		ans.w = rotation.w * invNorm);
	}

	//
	// 요약:
	//     Spherically interpolates between quaternions a and b by ratio t. The parameter
	//     t is clamped to the range [0, 1].
	//
	// 매개 변수:
	//   a:
	//     Start value, returned when t = 0.
	//
	//   b:
	//     End value, returned when t = 1.
	//
	//   t:
	//     Interpolation ratio.
	//
	// 반환 값:
	//     A quaternion spherically interpolated between quaternions a and b.
	public static sQuaternion Slerp(sQuaternion quaternion1, sQuaternion quaternion2, sfloat amount)
	{
		sfloat t = amount;

		sfloat cosOmega = quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y +
						 quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w;

		bool flip = false;

		if (cosOmega < (sfloat)0.0f)
		{
			flip = true;
			cosOmega = -cosOmega;
		}

		sfloat s1, s2;

		if (cosOmega > ((sfloat)1.0f - (sfloat)1e-6f))
		{
			// Too close, do straight linear interpolation.
			s1 = (sfloat)1.0f - t;
			s2 = (flip) ? -t : t;
		}
		else
		{
			sfloat omega = sMathf.Acos(cosOmega);
			sfloat invSinOmega = (sfloat)1 / sMathf.Sin(omega);

			s1 = sMathf.Sin(((sfloat)1.0f - t) * omega) * invSinOmega;
			s2 = (flip)
				? -sMathf.Sin(t * omega) * invSinOmega
				: sMathf.Sin(t * omega) * invSinOmega;
		}

		return new(
		s1 * quaternion1.x + s2 * quaternion2.x,
		s1 * quaternion1.y + s2 * quaternion2.y,
		s1 * quaternion1.z + s2 * quaternion2.z,
		s1 * quaternion1.w + s2 * quaternion2.w);
	}
	//
	// 요약:
	//     Spherically interpolates between a and b by t. The parameter t is not clamped.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	public readonly sfloat LengthSquared()
	{
		return x * x + y * y + z * z + w * w;
	}

	private static sQuaternion SlerpUnclamped(ref sQuaternion a, ref sQuaternion b, sfloat t)
	{
		// if either input is zero, return the other.
		if (a.LengthSquared() == (sfloat)0.0f)
		{
			if (b.LengthSquared() == (sfloat)0.0f)
			{
				return new sQuaternion(0f, 0f, 0f, 1f);
			}

			return b;
		}
		else if (b.LengthSquared() == (sfloat)0.0f)
		{
			return a;
		}

		sfloat cosHalfAngle = (a.w * b.w) + sVector3.Dot(new sVector3(a.x, a.y, a.z), new sVector3(b.x, b.y, b.z));
		if (cosHalfAngle >= (sfloat)1.0f || cosHalfAngle <= (sfloat)(-1.0f))
		{
			// angle = 0.0f, so just return one input.
			return a;
		}
		else if (cosHalfAngle < (sfloat)0.0f)
		{
			b.x = -b.x;
			b.y = -b.y;
			b.z = -b.z;
			b.w = -b.w;
			cosHalfAngle = -cosHalfAngle;
		}

		sfloat blendA;
		sfloat blendB;
		if (cosHalfAngle < (sfloat)0.99f)
		{
			// do proper slerp for big angles
			sfloat halfAngle = sMathf.Acos(cosHalfAngle);
			sfloat sinHalfAngle = sMathf.Sin(halfAngle);
			sfloat oneOverSinHalfAngle = (sfloat)1.0f / sinHalfAngle;
			blendA = sMathf.Sin(halfAngle * ((sfloat)1.0f - t)) * oneOverSinHalfAngle;
			blendB = sMathf.Sin(halfAngle * t) * oneOverSinHalfAngle;
		}
		else
		{
			// do lerp if angle is really small.
			blendA = (sfloat)1.0f - t;
			blendB = t;
		}

		sQuaternion result = new sQuaternion(blendA * new sVector3(a.x, a.y, a.z) + blendB * new sVector3(b.x, b.y, b.z), blendA * a.w + blendB * b.w);
		if (result.LengthSquared() > (sfloat)0.0f)
			return Normalize(result);
		else
			return sQuaternion.identity;
	}

	//
	// 요약:
	//     Interpolates between a and b by t and normalizes the result afterwards. The parameter
	//     t is clamped to the range [0, 1].
	//
	// 매개 변수:
	//   a:
	//     Start value, returned when t = 0.
	//
	//   b:
	//     End value, returned when t = 1.
	//
	//   t:
	//     Interpolation ratio.
	//
	// 반환 값:
	//     A quaternion interpolated between quaternions a and b.
	public static sQuaternion Lerp(sQuaternion quaternion1, sQuaternion quaternion2, sfloat amount)
	{
		sfloat t = amount;
		sfloat t1 = (sfloat)1.0f - t;

		sQuaternion r = default;

		sfloat dot = quaternion1.x * quaternion2.x + quaternion1.y * quaternion2.y +
					quaternion1.z * quaternion2.z + quaternion1.w * quaternion2.w;

		if (dot >= (sfloat)0.0f)
		{
			r.x = t1 * quaternion1.x + t * quaternion2.x;
			r.y = t1 * quaternion1.y + t * quaternion2.y;
			r.z = t1 * quaternion1.z + t * quaternion2.z;
			r.w = t1 * quaternion1.w + t * quaternion2.w;
		}
		else
		{
			r.x = t1 * quaternion1.x - t * quaternion2.x;
			r.y = t1 * quaternion1.y - t * quaternion2.y;
			r.z = t1 * quaternion1.z - t * quaternion2.z;
			r.w = t1 * quaternion1.w - t * quaternion2.w;
		}

		// Normalize it.
		sfloat ls = r.x * r.x + r.y * r.y + r.z * r.z + r.w * r.w;
		sfloat invNorm = (sfloat)1.0f / sMathf.Sqrt(ls);

		r.x *= invNorm;
		r.y *= invNorm;
		r.z *= invNorm;
		r.w *= invNorm;

		return r;
	}

	//
	// 요약:
	//     Interpolates between a and b by t and normalizes the result afterwards. The parameter
	//     t is not clamped.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	//public static sQuaternion LerpUnclamped(sQuaternion a, sQuaternion b, sfloat t)
	//{
	//	LerpUnclamped_Injected(ref a, ref b, t, out var ret);
	//	return ret;
	//}

	//private static sQuaternion Internal_FromEulerRad(sVector3 euler)
	//{
	//	Internal_FromEulerRad_Injected(ref euler, out var ret);
	//	return ret;
	//}

	//private static sVector3 Internal_ToEulerRad(sQuaternion rotation)
	//{
	//	Internal_ToEulerRad_Injected(ref rotation, out var ret);
	//	return ret;
	//}

	//private static void Internal_ToAxisAngleRad(sQuaternion q, out sVector3 axis, out sfloat angle)
	//{
	//	Internal_ToAxisAngleRad_Injected(ref q, out axis, out angle);
	//}

	////
	//// 요약:
	////     Creates a rotation which rotates angle degrees around axis.
	////
	//// 매개 변수:
	////   angle:
	////
	////   axis:
	//public static sQuaternion AngleAxis(sfloat angle, sVector3 axis)
	//{
	//	AngleAxis_Injected(angle, ref axis, out var ret);
	//	return ret;
	//}

	//
	// 요약:
	//     Creates a rotation with the specified forward and upwards directions.
	//
	// 매개 변수:
	//   forward:
	//     The direction to look in.
	//
	//   upwards:
	//     The vector that defines in which direction up is.
	public static sQuaternion LookRotation(sVector3 forward, sVector3 up)
	{
		forward = sVector3.Normalize(forward);
		sVector3 right = sVector3.Normalize(sVector3.Cross(up, forward));
		up = sVector3.Cross(forward, right);
		var m00 = right.x;
		var m01 = right.y;
		var m02 = right.z;
		var m10 = up.x;
		var m11 = up.y;
		var m12 = up.z;
		var m20 = forward.x;
		var m21 = forward.y;
		var m22 = forward.z;
		sfloat num8 = (m00 + m11) + m22;
		var quaternion = default(sQuaternion);
		if (num8 > (sfloat)0f)
		{
			var num = sMathf.Sqrt(num8 + (sfloat)1f);
			quaternion.w = num * (sfloat)0.5f;
			num = (sfloat)0.5f / num;
			quaternion.x = (m12 - m21) * num;
			quaternion.y = (m20 - m02) * num;
			quaternion.z = (m01 - m10) * num;
			return quaternion;
		}

		if ((m00 >= m11) && (m00 >= m22))
		{
			var num7 = sMathf.Sqrt((((sfloat)1f + m00) - m11) - m22);
			var num4 = (sfloat)0.5f / num7;
			quaternion.x = (sfloat)0.5f * num7;
			quaternion.y = (m01 + m10) * num4;
			quaternion.z = (m02 + m20) * num4;
			quaternion.w = (m12 - m21) * num4;
			return quaternion;
		}

		if (m11 > m22)
		{
			var num6 = sMathf.Sqrt((((sfloat)1f + m11) - m00) - m22);
			var num3 = (sfloat)0.5f / num6;
			quaternion.x = (m10 + m01) * num3;
			quaternion.y = (sfloat)0.5f * num6;
			quaternion.z = (m21 + m12) * num3;
			quaternion.w = (m20 - m02) * num3;
			return quaternion;
		}

		var num5 = sMathf.Sqrt((((sfloat)1f + m22) - m00) - m11);
		var num2 = (sfloat)0.5f / num5;
		quaternion.x = (m20 + m02) * num2;
		quaternion.y = (m21 + m12) * num2;
		quaternion.z = (sfloat)0.5f * num5;
		quaternion.w = (m01 - m10) * num2;
		return quaternion;
	}
	//
	// 요약:
	//     Creates a rotation with the specified forward and upwards directions.
	//
	// 매개 변수:
	//   forward:
	//     The direction to look in.
	//
	//   upwards:
	//     The vector that defines in which direction up is.
	public static sQuaternion LookRotation(sVector3 forward)
	{
		return LookRotation(forward, sVector3.up);
	}

	//
	// 요약:
	//     Constructs new Quaternion with given x,y,z,w components.
	//
	// 매개 변수:
	//   x:
	//
	//   y:
	//
	//   z:
	//
	//   w:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sQuaternion(sfloat x, sfloat y, sfloat z, sfloat w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sQuaternion(sVector3 v, sfloat w)
	{
		this.x = v.x;
		this.y = v.y;
		this.z = v.z;
		this.w = w;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sQuaternion(float x, float y, float z, float w)
	{
		this.x = (sfloat)x;
		this.y = (sfloat)y;
		this.z = (sfloat)z;
		this.w = (sfloat)w;
	}
	//
	// 요약:
	//     Set x, y, z and w components of an existing Quaternion.
	//
	// 매개 변수:
	//   newX:
	//
	//   newY:
	//
	//   newZ:
	//
	//   newW:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(sfloat newX, sfloat newY, sfloat newZ, sfloat newW)
	{
		x = newX;
		y = newY;
		z = newZ;
		w = newW;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sQuaternion operator *(sQuaternion lhs, sQuaternion rhs)
	{
		return new sQuaternion(lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y, lhs.w * rhs.y + lhs.y * rhs.w + lhs.z * rhs.x - lhs.x * rhs.z, lhs.w * rhs.z + lhs.z * rhs.w + lhs.x * rhs.y - lhs.y * rhs.x, lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
	}

	public static sVector3 operator *(sQuaternion rotation, sVector3 point)
	{
		sfloat num = rotation.x * (sfloat)2f;
		sfloat num2 = rotation.y * (sfloat)2f;
		sfloat num3 = rotation.z * (sfloat)2f;
		sfloat num4 = rotation.x * num;
		sfloat num5 = rotation.y * num2;
		sfloat num6 = rotation.z * num3;
		sfloat num7 = rotation.x * num2;
		sfloat num8 = rotation.x * num3;
		sfloat num9 = rotation.y * num3;
		sfloat num10 = rotation.w * num;
		sfloat num11 = rotation.w * num2;
		sfloat num12 = rotation.w * num3;
		sVector3 result = default(sVector3);
		result.x = ((sfloat)1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
		result.y = (num7 + num12) * point.x + ((sfloat)1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
		result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + ((sfloat)1f - (num4 + num5)) * point.z;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsEqualUsingDot(sfloat dot)
	{
		return dot > (sfloat)0.999999f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(sQuaternion lhs, sQuaternion rhs)
	{
		return IsEqualUsingDot(Dot(lhs, rhs));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(sQuaternion lhs, sQuaternion rhs)
	{
		return !(lhs == rhs);
	}

	//
	// 요약:
	//     The dot product between two rotations.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Dot(sQuaternion a, sQuaternion b)
	{
		return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
	}

	//
	// 요약:
	//     Creates a rotation with the specified forward and upwards directions.
	//
	// 매개 변수:
	//   view:
	//     The direction to look in.
	//
	//   up:
	//     The vector that defines in which direction up is.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLookRotation(sVector3 view)
	{
		sVector3 up = sVector3.up;
		SetLookRotation(view, up);
	}

	//
	// 요약:
	//     Creates a rotation with the specified forward and upwards directions.
	//
	// 매개 변수:
	//   view:
	//     The direction to look in.
	//
	//   up:
	//     The vector that defines in which direction up is.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetLookRotation(sVector3 view, sVector3 up)
	{
		this = LookRotation(view, up);
	}

	//
	// 요약:
	//     Returns the angle in degrees between two rotations a and b.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Angle(sQuaternion a, sQuaternion b)
	{
		sfloat num = sMathf.Min(sMathf.Abs(Dot(a, b)), (sfloat)1f);
		return IsEqualUsingDot(num) ? (sfloat)0f : (sMathf.Acos(num) * (sfloat)2f * (sfloat)57.29578f);
	}

	private static sVector3 Internal_MakePositive(sVector3 euler)
	{
		sfloat num = (sfloat)(-0.005729578f);
		sfloat num2 = (sfloat)360f + num;
		if (euler.x < num)
		{
			euler.x += (sfloat)360f;
		}
		else if (euler.x > num2)
		{
			euler.x -= (sfloat)360f;
		}

		if (euler.y < num)
		{
			euler.y += (sfloat)360f;
		}
		else if (euler.y > num2)
		{
			euler.y -= (sfloat)360f;
		}

		if (euler.z < num)
		{
			euler.z += (sfloat)360f;
		}
		else if (euler.z > num2)
		{
			euler.z -= (sfloat)360f;
		}

		return euler;
	}

	//
	// 요약:
	//     Returns a rotation that rotates z degrees around the z axis, x degrees around
	//     the x axis, and y degrees around the y axis; applied in that order.
	//
	// 매개 변수:
	//   x:
	//
	//   y:
	//
	//   z:
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static sQuaternion Euler(sfloat x, sfloat y, sfloat z)
	//{
	//	return Internal_FromEulerRad(new sVector3(x, y, z) * ((sfloat)Math.PI / (sfloat)180f));
	//}

	//
	// 요약:
	//     Returns a rotation that rotates z degrees around the z axis, x degrees around
	//     the x axis, and y degrees around the y axis.
	//
	// 매개 변수:
	//   euler:
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static sQuaternion Euler(sVector3 euler)
	//{
	//	return Internal_FromEulerRad(euler * ((sfloat)Math.PI / (sfloat)180f));
	//}

	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public void ToAngleAxis(out sfloat angle, out sVector3 axis)
	//{
	//	Internal_ToAxisAngleRad(this, out axis, out angle);
	//	angle *= (sfloat)57.29578f;
	//}

	//
	// 요약:
	//     Creates a rotation which rotates from fromDirection to toDirection.
	//
	// 매개 변수:
	//   fromDirection:
	//
	//   toDirection:
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public void SetFromToRotation(sVector3 fromDirection, sVector3 toDirection)
	//{
	//	this = FromToRotation(fromDirection, toDirection);
	//}

	//
	// 요약:
	//     Rotates a rotation from towards to.
	//
	// 매개 변수:
	//   from:
	//
	//   to:
	//
	//   maxDegreesDelta:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sQuaternion RotateTowards(sQuaternion from, sQuaternion to, sfloat maxDegreesDelta)
	{
		sfloat num = Angle(from, to);
		if (num == sfloat.Zero)
		{
			return to;
		}

		return SlerpUnclamped(ref from, ref to, sMathf.Min(sfloat.One, maxDegreesDelta / num));
	}

	//
	// 요약:
	//     Converts this quaternion to one with the same orientation but with a magnitude
	//     of 1.
	//
	// 매개 변수:
	//   q:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sQuaternion Normalize(sQuaternion q)
	{
		sfloat num = sMathf.Sqrt(Dot(q, q));
		if (num < sfloat.Epsilon)
		{
			return identity;
		}

		return new sQuaternion(q.x / num, q.y / num, q.z / num, q.w / num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		this = Normalize(this);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object other)
	{
		if (!(other is sQuaternion))
		{
			return false;
		}

		return Equals((sQuaternion)other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(sQuaternion other)
	{
		return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z) && w.Equals(other.w);
	}

	//
	// 요약:
	//     Returns a formatted string for this quaternion.
	//
	// 매개 변수:
	//   format:
	//     A numeric format string.
	//
	//   formatProvider:
	//     An object that specifies culture-specific formatting.
	public override string ToString()
	{
		return ToString(null, null);
	}

	//
	// 요약:
	//     Returns a formatted string for this quaternion.
	//
	// 매개 변수:
	//   format:
	//     A numeric format string.
	//
	//   formatProvider:
	//     An object that specifies culture-specific formatting.
	public string ToString(string format)
	{
		return ToString(format, null);
	}

	//
	// 요약:
	//     Returns a formatted string for this quaternion.
	//
	// 매개 변수:
	//   format:
	//     A numeric format string.
	//
	//   formatProvider:
	//     An object that specifies culture-specific formatting.
	public string ToString(string format, IFormatProvider formatProvider)
	{
		if (string.IsNullOrEmpty(format))
		{
			format = "F5";
		}

		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}

		return String.Format("({0}, {1}, {2}, {3})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider), w.ToString(format, formatProvider));
	}

	public static sVector3 Rotate(in sQuaternion rotation, in sVector3 point)
	{
		sfloat x = rotation.x * (sfloat)2f;
		sfloat y = rotation.y * (sfloat)2f;
		sfloat z = rotation.z * (sfloat)2f;
		sfloat xx = rotation.x * x;
		sfloat yy = rotation.y * y;
		sfloat zz = rotation.z * z;
		sfloat xy = rotation.x * y;
		sfloat xz = rotation.x * z;
		sfloat yz = rotation.y * z;
		sfloat wx = rotation.w * x;
		sfloat wy = rotation.w * y;
		sfloat wz = rotation.w * z;

		return new sVector3(
			((sfloat)1F - (yy + zz)) * point.x + (xy - wz) * point.y + (xz + wy) * point.z,
			(xy + wz) * point.x + ((sfloat)1f - (xx + zz)) * point.y + (yz - wx) * point.z,
			(xz - wy) * point.x + (yz + wx) * point.y + ((sfloat)1f - (xx + yy)) * point.z);
	}
}