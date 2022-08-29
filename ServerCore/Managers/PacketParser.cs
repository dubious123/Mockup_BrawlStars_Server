using System;
using Server.Packets;
using System.Collections.Concurrent;
using static ServerCore.Utils.Enums;

namespace ServerCore.Managers
{
	public static class PacketParser
	{
		static readonly ConcurrentDictionary<ushort, Func<ArraySegment<byte>, PacketContext>> _parserDict;
		static PacketParser()
		{
			_parserDict = new ConcurrentDictionary<ushort, Func<ArraySegment<byte>, PacketContext>>();
			_parserDict.TryAdd((ushort)PacketId.S_Chat, arr => new PacketContext(PacketId.S_Chat, S_Chat.Parser.ParseFrom(arr)));
			_parserDict.TryAdd((ushort)PacketId.C_Chat, arr => new PacketContext(PacketId.C_Chat, C_Chat.Parser.ParseFrom(arr)));
		}
		public static PacketContext Parse(ushort id, ArraySegment<byte> segment)
		{
			if (_parserDict.TryGetValue(id, out Func<ArraySegment<byte>, PacketContext> func) == false)
				throw new Exception();
			return func.Invoke(segment);
		}

	}
}
