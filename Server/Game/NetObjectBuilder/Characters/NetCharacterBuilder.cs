using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public static class NetCharacterBuilder
	{
		public static NetObject CreateShelly(NetObjectBuilder builder)
		{
			var obj = builder.GetRawObject(NetObjectType.Character_Shelly);
			obj.AddComponent<NetCircleCollider2D>()
				.SetOffsetAndRadius(sVector2.zero, (sfloat)0.5f);

			obj.AddComponent<NCharacterShelly>();
			return obj;
		}
	}
}
