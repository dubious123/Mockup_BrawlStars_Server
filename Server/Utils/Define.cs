using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
	public static class Define
	{
		public const string PacketHandlerQueueName = "PacketHandler Thread";
		public const string PacketSendQueueName = "Packet Send Thread";
		public const string PacketGameQueueName = "Packet Game Thread";
	}
}
