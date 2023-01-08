using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_EDITOR
using Newtonsoft.Json;
#endif

namespace Server.Game.Data
{
	public class NetCollider2DData
	{
		public sVector2 Offset { get; init; }
	}
}
