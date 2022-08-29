using Google.Protobuf;
using static ServerCore.Utils.Enums;

namespace ServerCore
{
	public class PacketContext
	{
		public readonly PacketId Id;
		public readonly IMessage Message;
		public PacketContext(PacketId id, IMessage message)
		{
			Id = id;
			Message = message;
		}
	}
}
