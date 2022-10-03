using static Server.S_BroadcastGameState;
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
				for (int i = 0; i < 1; i++)
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
		readonly JobQueue _gameQueue;
		readonly JobQueue _sendQueue;
		long _currentTick = 0;
		int _gameStarted = 0;
		S_BroadcastGameState _packet;
		public GameRoom(int id, ushort mapId)
		{
			Id = id;
			_players = new Player[6];
			_coHelper = new();
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
			while (true)
			{
				//todo object pooling
				_packet = new();
				_coHelper.Update();
				for (int i = 0; i < 6; i++)
				{
					player = _players[i];
					if (player is null) continue;
					while (player.InputBuffer.IsEmpty)
					{
						//todo
					}
					player.InputBuffer.TryDequeue(out var input);
					for (int j = player.InputBuffer.Count; j > 0; j--)
					{
						player.InputBuffer.TryDequeue(out var temp);
						PlayerInput.Combine(in temp, in input, out input);
					}
					_packet.PlayerMoveDirArr[i].X = input.MoveDirX;
					_packet.PlayerMoveDirArr[i].Y = input.MoveDirY;
					_packet.PlayerLookDirArr[i].X = input.LookDirX;
					_packet.PlayerLookDirArr[i].Y = input.LookDirY;
					_packet.ButtonPressedArr[i] = input.ButtonPressed;
					_packet.TargetTick = input.ClientTargetTick;
					_packet.StartTick = _currentTick;
					player.Character.HandleInput(input);
				}
				LogMgr.Log($"----------moving one tick, current tick : [{_currentTick}]--------------", TraceSourceType.Debug);
				for (int i = 0; i < 6; i++)
				{
					_players[i]?.Character.HandleOneFrame();
				}
				Broadcast(_packet);
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
				BaseCharacter character = new(this, _playerCount);
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
		public void PushActionResult(uint actionCode, short subject, params short[] objects)
		{
			//Todo object pooling
			_packet.Actions.Add(new GameActionResult(actionCode, subject, objects));
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
