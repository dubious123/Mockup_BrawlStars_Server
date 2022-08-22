using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
	public class ClientSession : Session
	{
		public override void OnConnected()
		{
			Console.WriteLine($"[server] connecting to {_socket.RemoteEndPoint} completed");
		}
	}
}
