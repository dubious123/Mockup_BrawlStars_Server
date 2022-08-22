using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace TestClient
{
	public class ServerSession : Session
	{
		public override void OnConnected()
		{
			Console.WriteLine($"[client] connecting to {_socket.RemoteEndPoint} completed");
		}
	}
}
