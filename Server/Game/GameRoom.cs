using Server.Game;
using static Server.Utils.Enums;
using System.Numerics;
using Server.Utils;
using System.Collections.Generic;
using ServerCore.Packets;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public ushort MapId;
		public GameState State;
		MapData _map;
		Player[] _players;
		short _playerCount;
		readonly object _lock = new();
		readonly float _moveLimit = 1f;
		readonly JobQueue _gameQueue;
		public GameRoom(int id, ushort mapId)
		{
			Id = id;
			_players = new Player[6];
			State = GameState.Waiting;
			MapId = mapId;
			_map = MapMgr.GetMapData(mapId);
			_gameQueue = JobMgr.GetQueue(Define.PacketGameQueueName);
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
				_players[_playerCount] = player;
				player.Position = _map.SpawnPosArr[_playerCount];
				player.CurrentGame = this;
				return _playerCount++;
			}
		}
		public bool Move(int userId, int teamId, Vector2 movePos, Vector2 lookDir)
		{
			if (teamId < 0 || 6 <= teamId || (_map.CanGo(movePos) == false)) return false;
			if (_players[teamId].UserId != userId) return false;
			Player player = _players[teamId];
			if ((player.Position - movePos).Length() > _moveLimit) return false;
			player.Position = movePos;
			player.LookDir = lookDir;
			return true;
		}
		public void Broadcast()
		{
			lock (_lock)
			{
				if (_playerCount <= 0) return;
				S_BroadcastGameState packet = new S_BroadcastGameState(this.Id, (ushort)_playerCount);
				for (int i = 0; i < _playerCount; i++)
				{
					var player = _players[i];
					packet.PlayerPosArr[i] = player.Position;
					packet.PlayerLookDirArr[i] = player.LookDir;
					packet.CharacterTypeArr[i] = (ushort)player.CharacterType;
				}
				for (int i = 0; i < _playerCount; i++)
				{
					var player = _players[i];
					player.Session.RegisterSend(packet);
				}
			}
		}
	}
}
