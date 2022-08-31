using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets
{
	public class BasePacket
	{
		public ushort Id;

		public BasePacket()
		{

		}
		public virtual void WriteTo(ref ArraySegment<byte> dest)
		{

		}
		//public virtual BasePacket Populate()
		//{

		//}
		//public virtual int CalculateSize()
		//{

		//}

	}
}
