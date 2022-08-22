using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ServerCore
{
	public class Session
	{
		protected int _id;
		protected Socket _socket;
		public void Init(int id, Socket socket)
		{
			_id = id;
			_socket = socket;
		}
		public virtual void OnConnected()
		{

		}
		public void RegisterSend()
		{

		}

		public void Send()
		{

		}

		void OnSendCompleted()
		{

		}

		void RegisterRecv()
		{

		}

		void OnRecvCompleted()
		{

		}


	}
}
