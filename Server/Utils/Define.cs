using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
	public static class Define
	{
		#region JobQueue name
		public const string PacketHandlerQueueName = "PacketHandler Thread";
		public const string PacketSendQueueName = "Packet Send Thread";
		public const string PacketGameQueueName = "Packet Game Thread";
		#endregion

		#region TraceSource name 
		public const string Ts_Packet = "Packets";
		public const string Ts_Session = "Session";
		public const string Ts_Network = "Network";
		public const string Ts_Handler = "Handler";
		public const string Ts_Console = "Console";
		public const string Ts_Error = "Error";
		public const string Ts_PacketSend = "Send";
		public const string Ts_PacketRecv = "Recv";


		#endregion

	}
}
