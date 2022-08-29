using System.Net;
using System.Net.Sockets;

namespace ServerCore.Utils
{
	public static class Tools
	{
		static IPAddress GetIPv4Address()
		{
			IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in ipHost.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetworkV6) return ip;
			}
			return null;
		}
		public static IPEndPoint GetNewEndPoint(int port)
		{
			return new IPEndPoint(GetIPv4Address(), port);
		}
	}
}
