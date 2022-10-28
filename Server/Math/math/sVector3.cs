
using System.Globalization;
using System.Runtime.CompilerServices;

public struct sVector3 : IEquatable<sVector3>, IFormattable
{
	public readonly sfloat kEpsilon = (sfloat)1E-05f;

	public readonly sfloat kEpsilonNormalSqrt = (sfloat)1E-15f;

	public sfloat x;

	public sfloat y;

	public sfloat z;

	private static readonly sVector3 zeroVector = new sVector3(sfloat.Zero, sfloat.Zero, sfloat.Zero);

	private static readonly sVector3 oneVector = new sVector3(sfloat.One, sfloat.One, sfloat.One);

	private static readonly sVector3 upVector = new sVector3(sfloat.Zero, sfloat.One, sfloat.Zero);

	private static readonly sVector3 downVector = new sVector3(sfloat.Zero, -sfloat.One, sfloat.Zero);

	private static readonly sVector3 leftVector = new sVector3(-sfloat.One, sfloat.Zero, sfloat.Zero);

	private static readonly sVector3 rightVector = new sVector3(sfloat.One, sfloat.Zero, sfloat.Zero);

	private static readonly sVector3 forwardVector = new sVector3(sfloat.Zero, sfloat.Zero, sfloat.One);

	private static readonly sVector3 backVector = new sVector3(sfloat.Zero, sfloat.Zero, -sfloat.One);

	private static readonly sVector3 positiveInfinityVector = new sVector3(sfloat.PositiveInfinity, sfloat.PositiveInfinity, sfloat.PositiveInfinity);

	private static readonly sVector3 negativeInfinityVector = new sVector3(sfloat.NegativeInfinity, sfloat.NegativeInfinity, sfloat.NegativeInfinity);

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
				_ => throw new IndexOutOfRangeException("Invalid sVector3 index!"),
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
				default:
					throw new IndexOutOfRangeException("Invalid sVector3 index!");
			}
		}
	}

	//
	// 요약:
	//     Returns this vector with a magnitude of 1 (Read Only).
	public sVector3 normalized
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Normalize(this);
		}
	}

	//
	// 요약:
	//     Returns the length of this vector (Read Only).
	public sfloat magnitude
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return sMathf.Sqrt(x * x + y * y + z * z);
		}
	}

	//
	// 요약:
	//     Returns the squared length of this vector (Read Only).
	public sfloat sqrMagnitude
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return x * x + y * y + z * z;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(0, 0, 0).
	public static sVector3 zero
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return zeroVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(1, 1, 1).
	public static sVector3 one
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return oneVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(0, 0, 1).
	public static sVector3 forward
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return forwardVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(0, 0, -1).
	public static sVector3 back
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return backVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(0, 1, 0).
	public static sVector3 up
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return upVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(0, -1, 0).
	public static sVector3 down
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return downVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(-1, 0, 0).
	public static sVector3 left
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return leftVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(1, 0, 0).
	public static sVector3 right
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return rightVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(sfloat.PositiveInfinity, sfloat.PositiveInfinity,
	//     sfloat.PositiveInfinity).
	public static sVector3 positiveInfinity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return positiveInfinityVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector3(sfloat.NegativeInfinity, sfloat.NegativeInfinity,
	//     sfloat.NegativeInfinity).
	public static sVector3 negativeInfinity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return negativeInfinityVector;
		}
	}
	//
	// 요약:
	//     Spherically interpolates between two vectors.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	//public static sVector3 Slerp(sVector3 a, sVector3 b, sfloat t)
	//{
	//	Slerp_Injected(ref a, ref b, t, out var ret);
	//	return ret;
	//}

	//
	// 요약:
	//     Spherically interpolates between two vectors.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	//public static sVector3 SlerpUnclamped(sVector3 a, sVector3 b, sfloat t)
	//{
	//	SlerpUnclamped_Injected(ref a, ref b, t, out var ret);
	//	return ret;
	//}

	//
	// 요약:
	//     Rotates a vector current towards target.
	//
	// 매개 변수:
	//   current:
	//     The vector being managed.
	//
	//   target:
	//     The vector.
	//
	//   maxRadiansDelta:
	//     The maximum angle in radians allowed for this rotation.
	//
	//   maxMagnitudeDelta:
	//     The maximum allowed change in vector magnitude for this rotation.
	//
	// 반환 값:
	//     The location that RotateTowards generates.
	//public static sVector3 RotateTowards(sVector3 current, sVector3 target, sfloat maxRadiansDelta, sfloat maxMagnitudeDelta)
	//{
	//	RotateTowards_Injected(ref current, ref target, maxRadiansDelta, maxMagnitudeDelta, out var ret);
	//	return ret;
	//}

	//
	// 요약:
	//     Linearly interpolates between two points.
	//
	// 매개 변수:
	//   a:
	//     Start value, returned when t = 0.
	//
	//   b:
	//     End value, returned when t = 1.
	//
	//   t:
	//     Value used to interpolate between a and b.
	//
	// 반환 값:
	//     Interpolated value, equals to a + (b - a) * t.
	//[MethodImpl(MethodImplOptions.AggressiveInlining)]
	//public static sVector3 Lerp(sVector3 a, sVector3 b, sfloat t)
	//{
	//	t = sMathf.Clamp01(t);
	//	return new sVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	//}

	//
	// 요약:
	//     Linearly interpolates between two vectors.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 LerpUnclamped(sVector3 a, sVector3 b, sfloat t)
	{
		return new sVector3(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t, a.z + (b.z - a.z) * t);
	}

	//
	// 요약:
	//     Calculate a position between the points specified by current and target, moving
	//     no farther than the distance specified by maxDistanceDelta.
	//
	// 매개 변수:
	//   current:
	//     The position to move from.
	//
	//   target:
	//     The position to move towards.
	//
	//   maxDistanceDelta:
	//     Distance to move current per call.
	//
	// 반환 값:
	//     The new position.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 MoveTowards(sVector3 current, sVector3 target, sfloat maxDistanceDelta)
	{
		sfloat num = target.x - current.x;
		sfloat num2 = target.y - current.y;
		sfloat num3 = target.z - current.z;
		sfloat num4 = num * num + num2 * num2 + num3 * num3;
		if (num4 == sfloat.Zero || (maxDistanceDelta >= sfloat.Zero && num4 <= maxDistanceDelta * maxDistanceDelta))
		{
			return target;
		}

		sfloat num5 = sMathf.Sqrt(num4);
		return new sVector3(current.x + num / num5 * maxDistanceDelta, current.y + num2 / num5 * maxDistanceDelta, current.z + num3 / num5 * maxDistanceDelta);
	}

	public static sVector3 SmoothDamp(sVector3 current, sVector3 target, ref sVector3 currentVelocity, sfloat smoothTime, sfloat maxSpeed, sfloat deltaTime)
	{
		sfloat num = sfloat.Zero;
		sfloat num2 = sfloat.Zero;
		sfloat num3 = sfloat.Zero;
		smoothTime = sMathf.Max(sfloat._0_0001, smoothTime);
		sfloat num4 = sfloat.Two / smoothTime;
		sfloat num5 = num4 * deltaTime;
		sfloat num6 = sfloat.One / (sfloat.One + num5 + (sfloat)0.48f * num5 * num5 + (sfloat)0.235f * num5 * num5 * num5);
		sfloat num7 = current.x - target.x;
		sfloat num8 = current.y - target.y;
		sfloat num9 = current.z - target.z;
		sVector3 vector = target;
		sfloat num10 = maxSpeed * smoothTime;
		sfloat num11 = num10 * num10;
		sfloat num12 = num7 * num7 + num8 * num8 + num9 * num9;
		if (num12 > num11)
		{
			sfloat num13 = sMathf.Sqrt(num12);
			num7 = num7 / num13 * num10;
			num8 = num8 / num13 * num10;
			num9 = num9 / num13 * num10;
		}

		target.x = current.x - num7;
		target.y = current.y - num8;
		target.z = current.z - num9;
		sfloat num14 = (currentVelocity.x + num4 * num7) * deltaTime;
		sfloat num15 = (currentVelocity.y + num4 * num8) * deltaTime;
		sfloat num16 = (currentVelocity.z + num4 * num9) * deltaTime;
		currentVelocity.x = (currentVelocity.x - num4 * num14) * num6;
		currentVelocity.y = (currentVelocity.y - num4 * num15) * num6;
		currentVelocity.z = (currentVelocity.z - num4 * num16) * num6;
		num = target.x + (num7 + num14) * num6;
		num2 = target.y + (num8 + num15) * num6;
		num3 = target.z + (num9 + num16) * num6;
		sfloat num17 = vector.x - current.x;
		sfloat num18 = vector.y - current.y;
		sfloat num19 = vector.z - current.z;
		sfloat num20 = num - vector.x;
		sfloat num21 = num2 - vector.y;
		sfloat num22 = num3 - vector.z;
		if (num17 * num20 + num18 * num21 + num19 * num22 > sfloat.Zero)
		{
			num = vector.x;
			num2 = vector.y;
			num3 = vector.z;
			currentVelocity.x = (num - vector.x) / deltaTime;
			currentVelocity.y = (num2 - vector.y) / deltaTime;
			currentVelocity.z = (num3 - vector.z) / deltaTime;
		}

		return new sVector3(num, num2, num3);
	}

	//
	// 요약:
	//     Creates a new vector with given x, y, z components.
	//
	// 매개 변수:
	//   x:
	//
	//   y:
	//
	//   z:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sVector3(sfloat x, sfloat y, sfloat z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sVector3(float x, float y, float z)
	{
		this.x = (sfloat)x;
		this.y = (sfloat)y;
		this.z = (sfloat)z;
	}

	//
	// 요약:
	//     Creates a new vector with given x, y components and sets z to zero.
	//
	// 매개 변수:
	//   x:
	//
	//   y:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sVector3(sfloat x, sfloat y)
	{
		this.x = x;
		this.y = y;
		z = sfloat.Zero;
	}

	//
	// 요약:
	//     Set x, y and z components of an existing sVector3.
	//
	// 매개 변수:
	//   newX:
	//
	//   newY:
	//
	//   newZ:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(sfloat newX, sfloat newY, sfloat newZ)
	{
		x = newX;
		y = newY;
		z = newZ;
	}

	//
	// 요약:
	//     Multiplies two vectors component-wise.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Scale(sVector3 a, sVector3 b)
	{
		return new sVector3(a.x * b.x, a.y * b.y, a.z * b.z);
	}

	//
	// 요약:
	//     Multiplies every component of this vector by the same component of scale.
	//
	// 매개 변수:
	//   scale:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Scale(sVector3 scale)
	{
		x *= scale.x;
		y *= scale.y;
		z *= scale.z;
	}

	//
	// 요약:
	//     Cross Product of two vectors.
	//
	// 매개 변수:
	//   lhs:
	//
	//   rhs:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Cross(sVector3 lhs, sVector3 rhs)
	{
		return new sVector3(lhs.y * rhs.z - lhs.z * rhs.y, lhs.z * rhs.x - lhs.x * rhs.z, lhs.x * rhs.y - lhs.y * rhs.x);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
	}

	//
	// 요약:
	//     Returns true if the given vector is exactly equal to this vector.
	//
	// 매개 변수:
	//   other:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object other)
	{
		if (!(other is sVector3))
		{
			return false;
		}

		return Equals((sVector3)other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(sVector3 other)
	{
		return x == other.x && y == other.y && z == other.z;
	}

	//
	// 요약:
	//     Reflects a vector off the plane defined by a normal.
	//
	// 매개 변수:
	//   inDirection:
	//
	//   inNormal:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Reflect(sVector3 inDirection, sVector3 inNormal)
	{
		sfloat num = (sfloat)(-2f) * Dot(inNormal, inDirection);
		return new sVector3(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y, num * inNormal.z + inDirection.z);
	}

	//
	// 요약:
	//     Makes this vector have a magnitude of 1.
	//
	// 매개 변수:
	//   value:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Normalize(sVector3 value)
	{
		sfloat num = Magnitude(value);
		if (num > (sfloat)1E-05f)
		{
			return value / num;
		}

		return zero;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		sfloat num = Magnitude(this);
		if (num > (sfloat)1E-05f)
		{
			this /= num;
		}
		else
		{
			this = zero;
		}
	}

	//
	// 요약:
	//     Dot Product of two vectors.
	//
	// 매개 변수:
	//   lhs:
	//
	//   rhs:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Dot(sVector3 lhs, sVector3 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
	}

	//
	// 요약:
	//     Projects a vector onto another vector.
	//
	// 매개 변수:
	//   vector:
	//
	//   onNormal:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Project(sVector3 vector, sVector3 onNormal)
	{
		sfloat num = Dot(onNormal, onNormal);
		if (num < sfloat.Epsilon)
		{
			return zero;
		}

		sfloat num2 = Dot(vector, onNormal);
		return new sVector3(onNormal.x * num2 / num, onNormal.y * num2 / num, onNormal.z * num2 / num);
	}

	//
	// 요약:
	//     Projects a vector onto a plane defined by a normal orthogonal to the plane.
	//
	// 매개 변수:
	//   planeNormal:
	//     The direction from the vector towards the plane.
	//
	//   vector:
	//     The location of the vector above the plane.
	//
	// 반환 값:
	//     The location of the vector on the plane.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 ProjectOnPlane(sVector3 vector, sVector3 planeNormal)
	{
		sfloat num = Dot(planeNormal, planeNormal);
		if (num < sfloat.Epsilon)
		{
			return vector;
		}

		sfloat num2 = Dot(vector, planeNormal);
		return new sVector3(vector.x - planeNormal.x * num2 / num, vector.y - planeNormal.y * num2 / num, vector.z - planeNormal.z * num2 / num);
	}

	//
	// 요약:
	//     Calculates the angle between vectors from and.
	//
	// 매개 변수:
	//   from:
	//     The vector from which the angular difference is measured.
	//
	//   to:
	//     The vector to which the angular difference is measured.
	//
	// 반환 값:
	//     The angle in degrees between the two vectors.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Angle(sVector3 from, sVector3 to)
	{
		sfloat num = sMathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
		if (num < (sfloat)1E-15f)
		{
			return sfloat.Zero;
		}

		sfloat num2 = sMathf.Clamp(Dot(from, to) / num, -sfloat.One, sfloat.One);
		return sMathf.Acos(num2) * (sfloat)57.29578f;
	}

	//
	// 요약:
	//     Calculates the signed angle between vectors from and to in relation to axis.
	//
	// 매개 변수:
	//   from:
	//     The vector from which the angular difference is measured.
	//
	//   to:
	//     The vector to which the angular difference is measured.
	//
	//   axis:
	//     A vector around which the other vectors are rotated.
	//
	// 반환 값:
	//     Returns the signed angle between from and to in degrees.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat SignedAngle(sVector3 from, sVector3 to, sVector3 axis)
	{
		sfloat num = Angle(from, to);
		sfloat num2 = from.y * to.z - from.z * to.y;
		sfloat num3 = from.z * to.x - from.x * to.z;
		sfloat num4 = from.x * to.y - from.y * to.x;
		sfloat num5 = sMathf.Sign(axis.x * num2 + axis.y * num3 + axis.z * num4);
		return num * num5;
	}

	//
	// 요약:
	//     Returns the distance between a and b.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Distance(sVector3 a, sVector3 b)
	{
		sfloat num = a.x - b.x;
		sfloat num2 = a.y - b.y;
		sfloat num3 = a.z - b.z;
		return sMathf.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	//
	// 요약:
	//     Returns a copy of vector with its magnitude clamped to maxLength.
	//
	// 매개 변수:
	//   vector:
	//
	//   maxLength:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 ClampMagnitude(sVector3 vector, sfloat maxLength)
	{
		sfloat num = vector.sqrMagnitude;
		if (num > maxLength * maxLength)
		{
			sfloat num2 = sMathf.Sqrt(num);
			sfloat num3 = vector.x / num2;
			sfloat num4 = vector.y / num2;
			sfloat num5 = vector.z / num2;
			return new sVector3(num3 * maxLength, num4 * maxLength, num5 * maxLength);
		}

		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Magnitude(sVector3 vector)
	{
		return sMathf.Sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat SqrMagnitude(sVector3 vector)
	{
		return vector.x * vector.x + vector.y * vector.y + vector.z * vector.z;
	}

	//
	// 요약:
	//     Returns a vector that is made from the smallest components of two vectors.
	//
	// 매개 변수:
	//   lhs:
	//
	//   rhs:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Min(sVector3 lhs, sVector3 rhs)
	{
		return new sVector3(sMathf.Min(lhs.x, rhs.x), sMathf.Min(lhs.y, rhs.y), sMathf.Min(lhs.z, rhs.z));
	}

	//
	// 요약:
	//     Returns a vector that is made from the largest components of two vectors.
	//
	// 매개 변수:
	//   lhs:
	//
	//   rhs:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 Max(sVector3 lhs, sVector3 rhs)
	{
		return new sVector3(sMathf.Max(lhs.x, rhs.x), sMathf.Max(lhs.y, rhs.y), sMathf.Max(lhs.z, rhs.z));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator +(sVector3 a, sVector3 b)
	{
		return new sVector3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator -(sVector3 a, sVector3 b)
	{
		return new sVector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator -(sVector3 a)
	{
		return new sVector3(sfloat.Zero - a.x, sfloat.Zero - a.y, sfloat.Zero - a.z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator *(sVector3 a, sfloat d)
	{
		return new sVector3(a.x * d, a.y * d, a.z * d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator *(sfloat d, sVector3 a)
	{
		return new sVector3(a.x * d, a.y * d, a.z * d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector3 operator /(sVector3 a, sfloat d)
	{
		return new sVector3(a.x / d, a.y / d, a.z / d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(sVector3 lhs, sVector3 rhs)
	{
		sfloat num = lhs.x - rhs.x;
		sfloat num2 = lhs.y - rhs.y;
		sfloat num3 = lhs.z - rhs.z;
		sfloat num4 = num * num + num2 * num2 + num3 * num3;
		return num4 < (sfloat)9.99999944E-11f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(sVector3 lhs, sVector3 rhs)
	{
		return !(lhs == rhs);
	}

	//
	// 요약:
	//     Returns a formatted string for this vector.
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
	//     Returns a formatted string for this vector.
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
	//     Returns a formatted string for this vector.
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
			format = "F2";
		}

		if (formatProvider == null)
		{
			formatProvider = CultureInfo.InvariantCulture.NumberFormat;
		}

		return String.Format("({0}, {1}, {2})", x.ToString(format, formatProvider), y.ToString(format, formatProvider), z.ToString(format, formatProvider));
	}
}