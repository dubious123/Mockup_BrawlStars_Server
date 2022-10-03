namespace Server.Utils
{
	public static class Define
	{
		#region JobQueue name
		public const string PacketHandlerQueueName = "PacketHandler Thread";
		public const string PacketParserQueueName = "PacketParser Thread";
		public const string PacketSendQueueName = "Packet Send Thread";
		public const string PacketGameQueueName = "Packet Game Thread";
		#endregion

		#region TraceSource name
		public const string TsPacket = "Packets";
		public const string TsSession = "Session";
		public const string TsNetwork = "Network";
		public const string TsHandler = "Handler";
		public const string TsConsole = "Console";
		public const string TsError = "Error";
		public const string TsPacketSend = "Send";
		public const string TsPacketRecv = "Recv";
		public const string TsDebug = "Debug";
		public const string TsGame = "Game";
		#endregion

	}
}
