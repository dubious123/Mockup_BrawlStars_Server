using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	//[skill code][team id][list of target id] => action info
	public record HitInfo
	{
		public int Damage { get; init; }
		public StunInfo StunInfo { get; init; }
		public KnockbackInfo KnockbackInfo { get; init; }
	}
	public record KnockbackInfo
	{
		public Vector3 Direction { get; set; }
		public float Duration { get; init; }
		public float Speed { get; init; }
	}
	public record StunInfo
	{
		public float Duration { get; init; }
	}
}
