using System.Globalization;
using System.Runtime.CompilerServices;

public struct sVector2 : IEquatable<sVector2>, IFormattable
{
	//
	// 요약:
	//     X component of the vector.
	public sfloat x;

	//
	// 요약:
	//     Y component of the vector.
	public sfloat y;

	private static readonly sVector2 zeroVector = new sVector2(0f, 0f);

	private static readonly sVector2 oneVector = new sVector2(1f, 1f);

	private static readonly sVector2 upVector = new sVector2(0f, 1f);

	private static readonly sVector2 downVector = new sVector2(0f, -1f);

	private static readonly sVector2 leftVector = new sVector2(-1f, 0f);

	private static readonly sVector2 rightVector = new sVector2(1f, 0f);

	private static readonly sVector2 positiveInfinityVector = new sVector2(sfloat.PositiveInfinity, sfloat.PositiveInfinity);

	private static readonly sVector2 negativeInfinityVector = new sVector2(sfloat.NegativeInfinity, sfloat.NegativeInfinity);

	public readonly sfloat kEpsilon = (sfloat)1E-05f;

	public readonly sfloat kEpsilonNormalSqrt = (sfloat)1E-15f;

	public sfloat this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return index switch
			{
				0 => x,
				1 => y,
				_ => throw new IndexOutOfRangeException("Invalid sVector2 index!"),
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
				default:
					throw new IndexOutOfRangeException("Invalid sVector2 index!");
			}
		}
	}

	//
	// 요약:
	//     Returns this vector with a magnitude of 1 (Read Only).
	public sVector2 normalized
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			sVector2 result = new sVector2(x, y);
			result.Normalize();
			return result;
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
			return (sfloat)sMathf.Sqrt(x * x + y * y);
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
			return x * x + y * y;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(0, 0).
	public static sVector2 zero
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return zeroVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(1, 1).
	public static sVector2 one
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return oneVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(0, 1).
	public static sVector2 up
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return upVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(0, -1).
	public static sVector2 down
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return downVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(-1, 0).
	public static sVector2 left
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return leftVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(1, 0).
	public static sVector2 right
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return rightVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(sfloat.PositiveInfinity, sfloat.PositiveInfinity).
	public static sVector2 positiveInfinity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return positiveInfinityVector;
		}
	}

	//
	// 요약:
	//     Shorthand for writing sVector2(sfloat.NegativeInfinity, sfloat.NegativeInfinity).
	public static sVector2 negativeInfinity
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return negativeInfinityVector;
		}
	}

	//
	// 요약:
	//     Constructs a new vector with given x, y components.
	//
	// 매개 변수:
	//   x:
	//
	//   y:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sVector2(float x, float y)
	{
		this.x = (sfloat)x;
		this.y = (sfloat)y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sVector2(sfloat x, sfloat y)
	{
		this.x = x;
		this.y = y;
	}

	//
	// 요약:
	//     Set x and y components of an existing sVector2.
	//
	// 매개 변수:
	//   newX:
	//
	//   newY:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Set(sfloat newX, sfloat newY)
	{
		x = newX;
		y = newY;
	}

	//
	// 요약:
	//     Linearly interpolates between vectors a and b by t.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 Lerp(sVector2 a, sVector2 b, sfloat t)
	{
		t = sMathf.Clamp01(t);
		return new sVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	//
	// 요약:
	//     Linearly interpolates between vectors a and b by t.
	//
	// 매개 변수:
	//   a:
	//
	//   b:
	//
	//   t:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 LerpUnclamped(sVector2 a, sVector2 b, sfloat t)
	{
		return new sVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
	}

	//
	// 요약:
	//     Moves a point current towards target.
	//
	// 매개 변수:
	//   current:
	//
	//   target:
	//
	//   maxDistanceDelta:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 MoveTowards(sVector2 current, sVector2 target, sfloat maxDistanceDelta)
	{
		sfloat num = target.x - current.x;
		sfloat num2 = target.y - current.y;
		sfloat num3 = num * num + num2 * num2;
		if (num3 == (sfloat)0f || (maxDistanceDelta >= (sfloat)0f && num3 <= maxDistanceDelta * maxDistanceDelta))
		{
			return target;
		}

		sfloat num4 = (sfloat)sMathf.Sqrt(num3);
		return new sVector2(current.x + num / num4 * maxDistanceDelta, current.y + num2 / num4 * maxDistanceDelta);
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
	public static sVector2 Scale(sVector2 a, sVector2 b)
	{
		return new sVector2(a.x * b.x, a.y * b.y);
	}

	//
	// 요약:
	//     Multiplies every component of this vector by the same component of scale.
	//
	// 매개 변수:
	//   scale:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Scale(sVector2 scale)
	{
		x *= scale.x;
		y *= scale.y;
	}

	//
	// 요약:
	//     Makes this vector have a magnitude of 1.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Normalize()
	{
		sfloat num = magnitude;
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

		return string.Format("({0}, {1})", x.ToString(format, formatProvider), y.ToString(format, formatProvider));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
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
		if (!(other is sVector2))
		{
			return false;
		}

		return Equals((sVector2)other);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(sVector2 other)
	{
		return x == other.x && y == other.y;
	}

	//
	// 요약:
	//     Reflects a vector off the vector defined by a normal.
	//
	// 매개 변수:
	//   inDirection:
	//
	//   inNormal:
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 Reflect(sVector2 inDirection, sVector2 inNormal)
	{
		sfloat num = (sfloat)(-2f) * Dot(inNormal, inDirection);
		return new sVector2(num * inNormal.x + inDirection.x, num * inNormal.y + inDirection.y);
	}

	//
	// 요약:
	//     Returns the 2D vector perpendicular to this 2D vector. The result is always rotated
	//     90-degrees in a counter-clockwise direction for a 2D coordinate system where
	//     the positive Y axis goes up.
	//
	// 매개 변수:
	//   inDirection:
	//     The input direction.
	//
	// 반환 값:
	//     The perpendicular direction.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 Perpendicular(sVector2 inDirection)
	{
		return new sVector2((sfloat)0f - inDirection.y, inDirection.x);
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
	public static sfloat Dot(sVector2 lhs, sVector2 rhs)
	{
		return lhs.x * rhs.x + lhs.y * rhs.y;
	}

	//
	// 요약:
	//     Gets the unsigned angle in degrees between from and to.
	//
	// 매개 변수:
	//   from:
	//     The vector from which the angular difference is measured.
	//
	//   to:
	//     The vector to which the angular difference is measured.
	//
	// 반환 값:
	//     The unsigned angle in degrees between the two vectors.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat Angle(sVector2 from, sVector2 to)
	{
		sfloat num = sMathf.Sqrt(from.sqrMagnitude * to.sqrMagnitude);
		if (num < (sfloat)1E-15f)
		{
			return (sfloat)0f;
		}

		sfloat num2 = sMathf.Clamp(Dot(from, to) / num, (sfloat)(-1f), (sfloat)1f);
		return sMathf.Acos(num2) * (sfloat)57.29578f;
	}

	//
	// 요약:
	//     Gets the signed angle in degrees between from and to.
	//
	// 매개 변수:
	//   from:
	//     The vector from which the angular difference is measured.
	//
	//   to:
	//     The vector to which the angular difference is measured.
	//
	// 반환 값:
	//     The signed angle in degrees between the two vectors.
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat SignedAngle(sVector2 from, sVector2 to)
	{
		sfloat num = Angle(from, to);
		sfloat num2 = sMathf.Sign(from.x * to.y - from.y * to.x);
		return num * num2;
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
	public static sfloat Distance(sVector2 a, sVector2 b)
	{
		sfloat num = a.x - b.x;
		sfloat num2 = a.y - b.y;
		return (sfloat)sMathf.Sqrt(num * num + num2 * num2);
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
	public static sVector2 ClampMagnitude(sVector2 vector, sfloat maxLength)
	{
		sfloat num = vector.sqrMagnitude;
		if (num > maxLength * maxLength)
		{
			sfloat num2 = (sfloat)sMathf.Sqrt(num);
			sfloat num3 = vector.x / num2;
			sfloat num4 = vector.y / num2;
			return new sVector2(num3 * maxLength, num4 * maxLength);
		}

		return vector;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sfloat SqrMagnitude(sVector2 a)
	{
		return a.x * a.x + a.y * a.y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sfloat SqrMagnitude()
	{
		return x * x + y * y;
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
	public static sVector2 Min(sVector2 lhs, sVector2 rhs)
	{
		return new sVector2(sMathf.Min(lhs.x, rhs.x), sMathf.Min(lhs.y, rhs.y));
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
	public static sVector2 Max(sVector2 lhs, sVector2 rhs)
	{
		return new sVector2(sMathf.Max(lhs.x, rhs.x), sMathf.Max(lhs.y, rhs.y));
	}

	public static sVector2 SmoothDamp(sVector2 current, sVector2 target, ref sVector2 currentVelocity, sfloat smoothTime, sfloat maxSpeed, sfloat deltaTime)
	{
		smoothTime = sMathf.Max((sfloat)0.0001f, smoothTime);
		sfloat num = (sfloat)2f / smoothTime;
		sfloat num2 = num * deltaTime;
		sfloat num3 = (sfloat)1f / ((sfloat)1f + num2 + (sfloat)0.48f * num2 * num2 + (sfloat)0.235f * num2 * num2 * num2);
		sfloat num4 = current.x - target.x;
		sfloat num5 = current.y - target.y;
		sVector2 vector = target;
		sfloat num6 = maxSpeed * smoothTime;
		sfloat num7 = num6 * num6;
		sfloat num8 = num4 * num4 + num5 * num5;
		if (num8 > num7)
		{
			sfloat num9 = sMathf.Sqrt(num8);
			num4 = num4 / num9 * num6;
			num5 = num5 / num9 * num6;
		}

		target.x = current.x - num4;
		target.y = current.y - num5;
		sfloat num10 = (currentVelocity.x + num * num4) * deltaTime;
		sfloat num11 = (currentVelocity.y + num * num5) * deltaTime;
		currentVelocity.x = (currentVelocity.x - num * num10) * num3;
		currentVelocity.y = (currentVelocity.y - num * num11) * num3;
		sfloat num12 = target.x + (num4 + num10) * num3;
		sfloat num13 = target.y + (num5 + num11) * num3;
		sfloat num14 = vector.x - current.x;
		sfloat num15 = vector.y - current.y;
		sfloat num16 = num12 - vector.x;
		sfloat num17 = num13 - vector.y;
		if (num14 * num16 + num15 * num17 > (sfloat)0f)
		{
			num12 = vector.x;
			num13 = vector.y;
			currentVelocity.x = (num12 - vector.x) / deltaTime;
			currentVelocity.y = (num13 - vector.y) / deltaTime;
		}

		return new sVector2(num12, num13);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator +(sVector2 a, sVector2 b)
	{
		return new sVector2(a.x + b.x, a.y + b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator -(sVector2 a, sVector2 b)
	{
		return new sVector2(a.x - b.x, a.y - b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator *(sVector2 a, sVector2 b)
	{
		return new sVector2(a.x * b.x, a.y * b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator /(sVector2 a, sVector2 b)
	{
		return new sVector2(a.x / b.x, a.y / b.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator -(sVector2 a)
	{
		return new sVector2((sfloat)0f - a.x, (sfloat)0f - a.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator *(sVector2 a, sfloat d)
	{
		return new sVector2(a.x * d, a.y * d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator *(sfloat d, sVector2 a)
	{
		return new sVector2(a.x * d, a.y * d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static sVector2 operator /(sVector2 a, sfloat d)
	{
		return new sVector2(a.x / d, a.y / d);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(sVector2 lhs, sVector2 rhs)
	{
		sfloat num = lhs.x - rhs.x;
		sfloat num2 = lhs.y - rhs.y;
		return num * num + num2 * num2 < (sfloat)9.99999944E-11f;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(sVector2 lhs, sVector2 rhs)
	{
		return !(lhs == rhs);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator sVector2(sVector3 v)
	{
		return new sVector2(v.x, v.y);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator sVector3(sVector2 v)
	{
		return new sVector3(v.x, v.y, (sfloat)0f);
	}
}