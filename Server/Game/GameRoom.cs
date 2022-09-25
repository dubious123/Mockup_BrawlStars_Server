using Server.Game;
using static Server.Utils.Enums;
using System.Numerics;
using Server.Utils;
using System.Collections.Generic;
using System.Threading;
using ServerCore;
using System.Diagnostics;
using Server.Game.Base;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public ushort MapId;
		public GameState State;
		public Player[] Players { get => _players; }
		public long CurrentTick { get => _currentTick; }
		MapData _map;
		Player[] _players;

		short _playerCount;
		readonly object _lock = new();
		readonly float _moveLimit = 1.5f;
		readonly JobQueue _gameQueue;
		readonly JobQueue _sendQueue;
		long _currentTick = 0;
		public GameRoom(int id, ushort mapId)
		{
			Id = id;
			_players = new Player[6];
			State = GameState.Waiting;
			MapId = mapId;
			_map = MapMgr.GetMapData(mapId);
			_gameQueue = JobMgr.GetQueue(Define.PacketGameQueueName);
			_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
			var co_update = Co_Update();
			Program.Update += () => co_update.MoveNext();
		}
		~GameRoom()
		{
			Program.Update -= () => Co_Update().MoveNext();
		}
		IEnumerator<float> Co_Update()
		{
			Player player;
			PlayerInput input;
			S_BroadcastGameState packet = new();
			while (true)
			{
				_currentTick++;
				for (int i = 0; i < 6; i++)
				{
					player = _players[i];
					if (player is null) continue;

					while ((player.InputBuffer.TryDequeue(out input)) == false)
					{
						//todo set timeout
						yield return 0f;
					}
					packet.PlayerMoveDirArr[i].X = input.MoveDirX;
					packet.PlayerMoveDirArr[i].Y = input.MoveDirY;
					packet.PlayerLookDirArr[i].X = input.LookDirX;
					packet.PlayerLookDirArr[i].Y = input.LookDirY;
					packet.StartTick = _currentTick;
					packet.TargetTick = _currentTick + 3;
					player.Character.Move(new Vector3(input.MoveDirX, 0, input.MoveDirY));
					player.Character.Look(new Vector3(input.LookDirX, 0, input.LookDirY));
				}
				Broadcast(packet);
				for (int i = 0; i < 6; i++)
				{
					_players[i]?.Character.Update();
				}
				yield return 0f;
			}
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
				BaseCharacter character = new();
				character.Position = new Vector3(_map.SpawnPosArr[_playerCount].X, 0, _map.SpawnPosArr[_playerCount].Y);
				player.CurrentGame = this;
				player.TeamId = _playerCount;
				player.Character = character;
				S_BroadcastEnterGame packet = new((ushort)player.Character.CharacterType, player.TeamId);
				Broadcast(player.TeamId, packet);
				_players[_playerCount] = player;
				player.Session.OnClosed.AddListener("GameRoomExit", () =>
				{
					Exit(player);
					player.Session.OnClosed.RemoveListener("GameRoomExit");
				});
				return _playerCount++;
			}
		}
		public bool Move(Player player, Vector2 moveDir, Vector2 lookDir)
		{
			lock (_lock)
			{
				//if (_map.CanGo(moveDir) == false) return false;
				Debug.Assert(_players[player.TeamId] == player);
				//if (_players[player.TeamId] != player) return false;
				//if ((player.Position - movePos).Length() > _moveLimit) return false;
				player.Character.Move(new Vector3(moveDir.X, 0, moveDir.Y));
				player.Character.Look(new Vector3(lookDir.X, 0, lookDir.Y));
				System.Console.WriteLine(player.TeamId);
				Broadcast(player.TeamId,
					new S_BroadcastMove(player.TeamId, new Vector2(moveDir.X, moveDir.Y), new Vector2(lookDir.X, lookDir.Y)));
				return true;
			}
		}
		//public void Broadcast()
		//{
		//	lock (_lock)
		//	{
		//		if (_playerCount <= 0) return;
		//		S_BroadcastGameState packet = new(this.Id, (ushort)_playerCount);
		//		for (int i = 0; i < 6; i++)
		//		{
		//			var go = _players[i].Character.gameObject;
		//			if (go is null) continue;
		//			var pos = go.Position;
		//			var rot = go.Rotation;
		//			packet.PlayerMoveDirArr[i] = new Vector2(pos.X, pos.Z);
		//			packet.PlayerLookDirArr[i] = new Vector2(rot.X, rot.Z);
		//		}
		//		for (int i = 0; i < 6; i++)
		//		{
		//			var player = _players[i];
		//			if (player is null) continue;
		//			_sendQueue.Push(() => player.Session.RegisterSend(packet));
		//		}
		//	}
		//}
		public void Broadcast(BasePacket packet)
		{
			for (int i = 0; i < 6; i++)
			{
				var player = _players[i];
				if (player is null) continue;
				_sendQueue.Push(() => player.Session.RegisterSend(packet));
			}
		}
		public void Broadcast(short hostTeamId, BasePacket packet)
		{
			for (int i = 0; i < 6; i++)
			{
				var player = _players[i];
				if (player is null || i == hostTeamId) continue;
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
