using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	//server => client : check avg RTT
	//server => client : start game after 3 sec - avg tick
	//client start game after 3sec - avg tick
	//
	//----------after 3 sec--------------
	//client, server game start & client, server tick check start
	//
	//client input happen
	//client => server : send input packet with client tick
	//
	//----------after RTT/2--------------
	//server calculate ping (server tick - client tick)
	//server handle input 
	//server => client input result, other players input
	//
	//----------after RTT/2--------------
	//
	//
	//client adjust state, and handle other players input immediately
	//
	//공격, 스킬등등은 animation, aftereffects 통해 눈가림 가능,
	//이동, 방향전환등은? 
	//client -> 우선 즉시 반영, 이후 후보정
	//
	//
	//client => send packet each frame (60 per sec)
	//server => wait until all packet arrive (30 per sec)

	// client -----||---send--check--adjust or rollback-||-------------------------
	// server -----||---lock------------broadcast-------||--------------||--------------------
	// client -----||---send-----||------predict--------||--------------||---------------------------
	public struct PlayerInput
	{
		public long ClientTargetTick;
		public long ReceivedTick;
		public float LookDirX;
		public float LookDirY;
		public float MoveDirX;
		public float MoveDirY;
		public ushort ButtonPressed;
		public void Combine(in PlayerInput other)
		{
			LookDirX = (LookDirX + other.LookDirX) / 2;
			LookDirY = (LookDirY + other.LookDirY) / 2;
			MoveDirX = (MoveDirX + other.MoveDirX) / 2;
			MoveDirY = (MoveDirY + other.MoveDirY) / 2;
			ButtonPressed = (ushort)(ButtonPressed | other.ButtonPressed);
		}
	}
}
