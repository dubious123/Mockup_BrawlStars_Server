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
	public readonly struct PlayerInput
	{
		public readonly long ClientTargetTick { get; init; }
		public readonly long ReceivedTick { get; init; }
		public readonly float LookDirX { get; init; }
		public readonly float LookDirY { get; init; }
		public readonly float MoveDirX { get; init; }
		public readonly float MoveDirY { get; init; }
		public readonly ushort ButtonPressed { get; init; }
		public static void Combine(in PlayerInput left, in PlayerInput right, out PlayerInput res)
		{
			res = new PlayerInput
			{
				LookDirX = (left.LookDirX + right.LookDirX) / 2,
				LookDirY = (left.LookDirY + right.LookDirY) / 2,
				MoveDirX = (left.MoveDirX + right.MoveDirX) / 2,
				MoveDirY = (left.MoveDirY + right.MoveDirY) / 2,
				ButtonPressed = (ushort)(left.ButtonPressed | right.ButtonPressed),
			};
		}
	}
}
