using Google.Protobuf;
using static ServerCore.Utils.Enums;
using System;
using System.Collections.Generic;
using Server.Packets;

namespace ServerCore.Managers
{
	public static class PacketParser
	{
		static readonly Dictionary<ushort, Func<ArraySegment<byte>, IMessage>> _parserDict;
		static PacketParser()
		{
			_parserDict = new Dictionary<ushort, Func<ArraySegment<byte>, IMessage>>();
			_parserDict.Add((ushort)PacketId.S_Chat, arr => S_Chat.Parser.ParseFrom(arr));
			_parserDict.Add((ushort)PacketId.C_Chat, arr => S_Chat.Parser.ParseFrom(arr));
		}
		public static IMessage Parse(ushort id, ArraySegment<byte> segment)
		{
			return _parserDict[id].Invoke(segment);
		}

	}
}
