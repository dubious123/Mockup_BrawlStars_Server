using Server.Game;
using static Server.Utils.Enums;
using System.Numerics;
using Server.Utils;
using System.Collections.Generic;
using System.Threading;
using ServerCore;
using System.Diagnostics;
using Server.Game.Base;
using System;
using Server.Log;
using System.Linq;

namespace Server
{
	public class GameRoom
	{
		public int Id;
		public ushort MapId;
		public GameState State;
		public Player[] Players { get => _players; }
		public long CurrentTick { get => _currentTick; }
		public bool IsReady
		{
			get
			{
				for (int i = 0; i < 2; i++)
				{
					if (_players[i] is null || _players[i].GameSceneReady == false) return false;
				}
				return true;
			}
		}
		public bool GameStarted => _gameStarted == 1;
		public CoroutineHelper CoHelper => _coHelper;
		MapData _map;
		Player[] _players;
		CoroutineHelper _coHelper;

		short _playerCount;
		readonly object _lock = new();
		readonly float _moveLimit = 1.5f;
		readonly JobQueue _gameQueue;
		readonly JobQueue _sendQueue;
		long _currentTick = 0;
		int _gameStarted = 0;
		public GameRoom(int id, ushort mapId)
		{
			Id = id;
			_players = new Player[6];
			_coroutineHelper = new();
			State = GameState.Waiting;
			MapId = mapId;
			_map = MapMgr.GetMapData(mapId);
			_gameQueue = JobMgr.GetQueue(Define.PacketGameQueueName);
			_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		}

		public void StartGame()
		{
			if (Interlocked.CompareExchange(ref _gameStarted, 1, 0) == 1) return;
			Broadcast(new S_BroadcastStartGame(0f));
			LogMgr.Log("----------Start Game--------------", TraceSourceType.Debug);
			Program.Update += () => Co_Update().MoveNext();
		}

		~GameRoom()
		{
			Program.Update -= () => Co_Update().MoveNext();
		}

		//Stopwatch _sw = new();
		IEnumerator<float> Co_Update()
		{
			Player player;
			S_BroadcastGameState packet = new();
			while (true)
			{
				//_sw.Restart();
				for (int i = 0; i < 6; i++)
				{
					player = _players[i];
					if (player is null) continue;
					while (player.InputBuffer.IsEmpty)
					{
						//todo
					}
					player.InputBuffer.TryDequeue(out PlayerInput input);
					for (int j = player.InputBuffer.Count; j > 0; j--)
					{
						player.InputBuffer.TryDequeue(out var temp);
						input.Combine(in temp);
					}

					packet.PlayerMoveDirArr[i].X = input.MoveDirX;
					packet.PlayerMoveDirArr[i].Y = input.MoveDirY;
					packet.PlayerLookDirArr[i].X = input.LookDirX;
					packet.PlayerLookDirArr[i].Y = input.LookDirY;
					packet.MousePressed[i] = input.ButtonPressed;
					packet.TargetTick = input.ClientTargetTick == 0 ? _currentTick + 3 : input.ClientTargetTick;
					//packet.TargetTick = input.ClientTargetTick < _currentTick ? _currentTick : input.ClientTargetTick;
					packet.StartTick = _currentTick;
					player.Character.HandleInput(input);
				}
				LogMgr.Log($"----------moving one tick, current tick : [{_currentTick}]--------------", TraceSourceType.Debug);
				Broadcast(packet);
				for (int i = 0; i < 6; i++)
				{
					_players[i]?.Character.Update();
				}
				//_sw.Stop();
				//LogMgr.Log($"inner update time {_sw.ElapsedMilliseconds}, tick: {_sw.ElapsedTicks}", TraceSourceType.Network);
				_currentTick++;
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
				BaseCharacter character = new(this);
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
		public IEnumerable<BaseCharacter> FindCharacters(Func<BaseCharacter, bool> condition)
		{
			return from player in _players
				   where player is not null && condition(player.Character)
				   select player.Character;
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
