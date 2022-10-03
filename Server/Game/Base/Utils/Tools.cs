namespace Server.Game.Base.Utils
{
	public class Tools
	{
		private const float RadToDeg = (float)(180.0 / Math.PI);
		private const float DegToRad = (float)(Math.PI / 180.0);
		#region Vector3
		public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
		{
			float output_x = 0f;
			float output_y = 0f;
			float output_z = 0f;

			// Based on Game Programming Gems 4 Chapter 1.10
			smoothTime = MathF.Max(0.0001F, smoothTime);
			float omega = 2F / smoothTime;

			float x = omega * deltaTime;
			float exp = 1F / (1F + x + 0.48F * x * x + 0.235F * x * x * x);

			float change_x = current.X - target.X;
			float change_y = current.Y - target.Y;
			float change_z = current.Z - target.Z;
			Vector3 originalTo = target;

			// Clamp maximum speed
			float maxChange = maxSpeed * smoothTime;

			float maxChangeSq = maxChange * maxChange;
			float sqrmag = change_x * change_x + change_y * change_y + change_z * change_z;
			if (sqrmag > maxChangeSq)
			{
				var mag = (float)Math.Sqrt(sqrmag);
				change_x = change_x / mag * maxChange;
				change_y = change_y / mag * maxChange;
				change_z = change_z / mag * maxChange;
			}

			target.X = current.X - change_x;
			target.Y = current.Y - change_y;
			target.Z = current.Z - change_z;

			float temp_x = (currentVelocity.X + omega * change_x) * deltaTime;
			float temp_y = (currentVelocity.Y + omega * change_y) * deltaTime;
			float temp_z = (currentVelocity.Z + omega * change_z) * deltaTime;

			currentVelocity.X = (currentVelocity.X - omega * temp_x) * exp;
			currentVelocity.Y = (currentVelocity.Y - omega * temp_y) * exp;
			currentVelocity.Z = (currentVelocity.Z - omega * temp_z) * exp;

			output_x = target.X + (change_x + temp_x) * exp;
			output_y = target.Y + (change_y + temp_y) * exp;
			output_z = target.Z + (change_z + temp_z) * exp;

			// Prevent overshooting
			float origMinusCurrent_x = originalTo.X - current.X;
			float origMinusCurrent_y = originalTo.Y - current.Y;
			float origMinusCurrent_z = originalTo.Z - current.Z;
			float outMinusOrig_x = output_x - originalTo.X;
			float outMinusOrig_y = output_y - originalTo.Y;
			float outMinusOrig_z = output_z - originalTo.Z;

			if (origMinusCurrent_x * outMinusOrig_x + origMinusCurrent_y * outMinusOrig_y + origMinusCurrent_z * outMinusOrig_z > 0)
			{
				output_x = originalTo.X;
				output_y = originalTo.Y;
				output_z = originalTo.Z;

				currentVelocity.X = (output_x - originalTo.X) / deltaTime;
				currentVelocity.Y = (output_y - originalTo.Y) / deltaTime;
				currentVelocity.Z = (output_z - originalTo.Z) / deltaTime;
			}

			return new Vector3(output_x, output_y, output_z);
		}

		public static float Angle(Vector3 from, Vector3 to)
		{
			// sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
			float denominator = (float)Math.Sqrt(from.LengthSquared() * to.LengthSquared());
			if (denominator < 1e-15F)
				return 0F;

			float dot = Clamp(Vector3.Dot(from, to) / denominator, -1F, 1F);
			return ((float)Math.Acos(dot)) * RadToDeg;
		}

		public static float Clamp(float value, float min, float max)
		{
			if (value < min)
				value = min;
			else if (value > max)
				value = max;
			return value;
		}
		#endregion
		#region Quaternion
		public static Quaternion LookRotation(Vector3 forward, Vector3 up)
		{
			forward = Vector3.Normalize(forward);
			Vector3 right = Vector3.Normalize(Vector3.Cross(up, forward));
			up = Vector3.Cross(forward, right);
			var m00 = right.X;
			var m01 = right.Y;
			var m02 = right.Z;
			var m10 = up.X;
			var m11 = up.Y;
			var m12 = up.Z;
			var m20 = forward.X;
			var m21 = forward.Y;
			var m22 = forward.Z;
			float num8 = (m00 + m11) + m22;
			var quaternion = default(Quaternion);
			if (num8 > 0f)
			{
				var num = (float)System.Math.Sqrt(num8 + 1f);
				quaternion.W = num * 0.5f;
				num = 0.5f / num;
				quaternion.X = (m12 - m21) * num;
				quaternion.Y = (m20 - m02) * num;
				quaternion.Z = (m01 - m10) * num;
				return quaternion;
			}

			if ((m00 >= m11) && (m00 >= m22))
			{
				var num7 = (float)System.Math.Sqrt(((1f + m00) - m11) - m22);
				var num4 = 0.5f / num7;
				quaternion.X = 0.5f * num7;
				quaternion.Y = (m01 + m10) * num4;
				quaternion.Z = (m02 + m20) * num4;
				quaternion.W = (m12 - m21) * num4;
				return quaternion;
			}

			if (m11 > m22)
			{
				var num6 = (float)System.Math.Sqrt(((1f + m11) - m00) - m22);
				var num3 = 0.5f / num6;
				quaternion.X = (m10 + m01) * num3;
				quaternion.Y = 0.5f * num6;
				quaternion.Z = (m21 + m12) * num3;
				quaternion.W = (m20 - m02) * num3;
				return quaternion;
			}

			var num5 = (float)System.Math.Sqrt(((1f + m22) - m00) - m11);
			var num2 = 0.5f / num5;
			quaternion.X = (m20 + m02) * num2;
			quaternion.Y = (m21 + m12) * num2;
			quaternion.Z = 0.5f * num5;
			quaternion.W = (m01 - m10) * num2;
			return quaternion;
		}

		public static Quaternion RotateTowards(Quaternion from, Quaternion to, float maxDegreesDelta)
		{
			float num = Angle(from, to);
			if (num == 0f)
			{
				return to;
			}

			float t = Math.Min(1f, maxDegreesDelta / num);
			return SlerpUnclamped(from, to, t);
		}

		public static float Angle(Quaternion a, Quaternion b)
		{
			float f = Quaternion.Dot(a, b);
			return MathF.Acos(MathF.Min(MathF.Abs(f), 1f)) * 2f * RadToDeg;
		}

		public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, float t)
		{
			return SlerpUnclamped(ref a, ref b, t);
		}

		public static Vector3 Rotate(in Quaternion rotation, in Vector3 point)
		{
			float x = rotation.X * 2F;
			float y = rotation.Y * 2F;
			float z = rotation.Z * 2F;
			float xx = rotation.X * x;
			float yy = rotation.Y * y;
			float zz = rotation.Z * z;
			float xy = rotation.X * y;
			float xz = rotation.X * z;
			float yz = rotation.Y * z;
			float wx = rotation.W * x;
			float wy = rotation.W * y;
			float wz = rotation.W * z;

			return new Vector3(
				(1F - (yy + zz)) * point.X + (xy - wz) * point.Y + (xz + wy) * point.Z,
				(xy + wz) * point.X + (1F - (xx + zz)) * point.Y + (yz - wx) * point.Z,
				(xz - wy) * point.X + (yz + wx) * point.Y + (1F - (xx + yy)) * point.Z);
		}

		private static Quaternion SlerpUnclamped(ref Quaternion a, ref Quaternion b, float t)
		{
			// if either input is zero, return the other.
			if (a.LengthSquared() == 0.0f)
			{
				if (b.LengthSquared() == 0.0f)
				{
					return new Quaternion(0f, 0f, 0f, 1f);
				}

				return b;
			}
			else if (b.LengthSquared() == 0.0f)
			{
				return a;
			}

			float cosHalfAngle = (a.W * b.W) + Vector3.Dot(new Vector3(a.X, a.Y, a.Z), new Vector3(b.X, b.Y, b.Z));
			if (cosHalfAngle >= 1.0f || cosHalfAngle <= -1.0f)
			{
				// angle = 0.0f, so just return one input.
				return a;
			}
			else if (cosHalfAngle < 0.0f)
			{
				b.X = -b.X;
				b.Y = -b.Y;
				b.Z = -b.Z;
				b.W = -b.W;
				cosHalfAngle = -cosHalfAngle;
			}

			float blendA;
			float blendB;
			if (cosHalfAngle < 0.99f)
			{
				// do proper slerp for big angles
				float halfAngle = (float)System.Math.Acos(cosHalfAngle);
				float sinHalfAngle = (float)System.Math.Sin(halfAngle);
				float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
				blendA = (float)System.Math.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
				blendB = (float)System.Math.Sin(halfAngle * t) * oneOverSinHalfAngle;
			}
			else
			{
				// do lerp if angle is really small.
				blendA = 1.0f - t;
				blendB = t;
			}

			Quaternion result = new Quaternion(blendA * new Vector3(a.X, a.Y, a.Z) + blendB * new Vector3(b.X, b.Y, b.Z), blendA * a.W + blendB * b.W);
			if (result.LengthSquared() > 0.0f)
				return Quaternion.Normalize(result);
			else
				return Quaternion.Identity;
		}
		#endregion
	}
}
