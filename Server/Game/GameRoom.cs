using Server.Game;
using static Server.Utils.Enums;
using System.Numerics;
using Server.Utils;
using System.Collections.Generic;
using System.Threading;
using ServerCore;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public ushort MapId;
		public GameState State;
		public Player[] Players { get => _players; }
		MapData _map;
		Player[] _players;

		short _playerCount;
		readonly object _lock = new();
		readonly float _moveLimit = 1f;
		readonly JobQueue _gameQueue;
		readonly JobQueue _sendQueue;
		public GameRoom(int id, ushort mapId)
		{
			Id = id;
			_players = new Player[6];
			State = GameState.Waiting;
			MapId = mapId;
			_map = MapMgr.GetMapData(mapId);
			_gameQueue = JobMgr.GetQueue(Define.PacketGameQueueName);
			_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		}
		/// <summary>
		/// -1 : room is full
		/// 0~2 : blue
		/// 3~5 : red
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public short Enter(Player player)
		{
			lock (_lock)
			{
				if (_playerCount >= 6) return -1;
				player.Position = _map.SpawnPosArr[_playerCount];
				player.CurrentGame = this;
				player.TeamId = _playerCount;
				S_BroadcastEnterGame packet = new((ushort)player.CharacterType, player.TeamId);
				Broadcast(packet);
				_players[_playerCount] = player;
				player.Session.OnClosed.AddListener("GameRoomExit", () =>
				{
					Exit(player);
					player.Session.OnClosed.RemoveListener("GameRoomExit");
				});
				return _playerCount++;
			}
		}
		public bool Move(Player player, Vector2 movePos, Vector2 lookDir)
		{
			lock (_lock)
			{
				if (_map.CanGo(movePos) == false) return false;
				if (_players[player.TeamId] != player) return false;
				if ((player.Position - movePos).Length() > _moveLimit) return false;
				player.Position = movePos;
				player.LookDir = lookDir;
				return true;
			}
		}
		public void Broadcast()
		{
			lock (_lock)
			{
				if (_playerCount <= 0) return;
				S_BroadcastGameState packet = new(this.Id, (ushort)_playerCount);
				for (int i = 0; i < 6; i++)
				{
					var player = _players[i];
					if (player is null) continue;
					packet.PlayerPosArr[i] = player.Position;
					packet.PlayerLookDirArr[i] = player.LookDir;
				}
				for (int i = 0; i < 6; i++)
				{
					var player = _players[i];
					if (player is null) continue;
					player.Session.RegisterSend(packet);
				}
			}
		}
		public void Broadcast(BasePacket packet)
		{
			for (int i = 0; i < 6; i++)
			{
				var player = _players[i];
				if (player is null) continue;
				_sendQueue.Push(() => player.Session.RegisterSend(packet));
			}
		}

		void Exit(Player player)
		{
			lock (_lock)
			{
				if (_players[player.TeamId] != player)
				{
					System.Console.WriteLine("ERRRRRRRRRRRRRRRRRRRRRRRRrrrrrrrrrrrrrrr");
					throw new System.Exception();
				}
				_players[player.TeamId] = null;
				_playerCount--;
			}
		}
	}
}
