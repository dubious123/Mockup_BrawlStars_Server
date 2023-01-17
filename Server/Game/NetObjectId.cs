using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Enums;

namespace Server.Game
{
	public class NetObjectId
	{
		public NetObjectType Type => _type;
		public int InstanceId => _instanceId;

		private uint _id;
		private int _instanceId;
		private NetObjectType _type;

		public static NetObjectId FromRaw(uint id)
		{
			return new NetObjectId()
			{
				_id = id,
				_type = (NetObjectType)(id >> 16),
				_instanceId = (int)(id & 0x0000_ffff),
			};
		}

		public uint GetRaw() => _id;

		public override int GetHashCode()
		{
			return (int)_id;
		}
	}
}
