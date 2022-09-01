using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
	public static class Extensions
	{
		public static void EnterGame(this GameRoom room, Player player)
		{
			GameMgr.EnterGame(room, player);
		}
	}
}
