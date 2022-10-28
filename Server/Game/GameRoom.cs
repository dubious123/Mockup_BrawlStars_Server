namespace Server;

using static Server.S_BroadcastGameState;

public class GameRoom
{
	private readonly object _lock = new();
	private readonly int _maxPlayerCount = 1;
	private readonly IEnumerator<float> _coHandle;
	private readonly JobQueue _gameQueue;
	private readonly JobQueue _sendQueue;
	private readonly ConcurrentQueue<Player> _enterBuffer = new();
	private int _gameStarted = 0;
	private short _playerCount = 0;
	private long _currentTick = 0;
	private MapData _map;
	private Player[] _players;
	private CoroutineHelper _coHelper;
	private S_BroadcastGameState _packet;

	public GameRoom(int id, ushort mapId)
	{
		Id = id;
		_players = new Player[_maxPlayerCount];
		_coHelper = new();
		State = GameState.Waiting;
		MapId = mapId;
		_map = MapMgr.GetMapData(mapId);
		_gameQueue = JobMgr.GetQueue(Define.PacketGameQueueName);
		_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		_coHandle = Co_Update();
	}

	~GameRoom()
	{
		Program.Update -= () => _coHandle.MoveNext();
	}

	public int Id { get; init; }
	public ushort MapId { get; init; }
	public bool GameStarted => _gameStarted == 1;
	public long CurrentTick { get => _currentTick; }
	public Player[] Players { get => _players; }
	public GameState State { get; init; }
	public CoroutineHelper CoHelper => _coHelper;

	private IEnumerator<float> Co_Update()
	{
		Player player;
		_playerCount = 0;

		#region Ready Game
		while (_enterBuffer.TryDequeue(out player))
		{
			_players[_playerCount] = player;
			player.TeamId = _playerCount;
			player.Character = GameMgr.CreateNewCharacter(this, player.TeamId, player.CharType);
			player.Character.Position = new sVector3(_map.SpawnPosArr[_playerCount].x, sfloat.Zero, _map.SpawnPosArr[_playerCount].y);
			player.CurrentGame = this;
			player.Session.OnClosed.AddListener("GameRoomExit", () =>
			{
				Exit(player);
				player.Session.OnClosed.RemoveListener("GameRoomExit");
			});

			_playerCount++;
		}
		#endregion

		#region Check if clients are ready
		while (true)
		{
			bool allReady = true;
			foreach (var p in _players)
			{
				allReady &= p.GameSceneReady;
			}
			if (allReady is true) break;
			yield return 0f;
		}
		#endregion

		LogMgr.LogInfo($"\n[{DateTime.Now}.{DateTime.Now.Millisecond:000}]\n-------------Start Game--------------", Id, TraceSourceType.Game);

		#region Broadcast start game
		Broadcast(new S_BroadcastStartGame(0f)
		{
			CharacterTypeArr = _players.Select(p => (ushort)(p.Character.CharacterType)).ToArray(),
		});
		#endregion

		yield return 0f;
		StringBuilder sb = new("\n");
		while (true)
		{
			_packet = new();
			sb.AppendLine($"[{DateTime.Now}.{DateTime.Now.Millisecond:000}]");
			sb.AppendLine($"-------------Start Frame [{_currentTick}]--------------");
			_coHelper.Update();
			for (int i = 0; i < _maxPlayerCount; i++)
			{
				player = _players[i];
				if (player is null) continue;
				while (player.InputBuffer.IsEmpty)
				{
					// todo
				}

				player.InputBuffer.TryDequeue(out var input);
				for (int j = player.InputBuffer.Count; j > 0; j--)
				{
					player.InputBuffer.TryDequeue(out var temp);
					PlayerInput.Combine(in temp, in input, out input);
				}
				_packet.PlayerMoveDirArr[i].X = (float)input.MoveDirX;
				_packet.PlayerMoveDirArr[i].Y = (float)input.MoveDirY;
				_packet.PlayerLookDirArr[i].X = (float)input.LookDirX;
				_packet.PlayerLookDirArr[i].Y = (float)input.LookDirY;
				_packet.ButtonPressedArr[i] = input.ButtonPressed;
				_packet.TargetTick = input.ClientTargetTick;
				_packet.StartTick = _currentTick;
				player.Character.HandleInput(input);
			}

			for (int i = 0; i < _maxPlayerCount; i++)
			{
				_players[i]?.Character.HandleOneFrame();
				var pos = _players[i]?.Character.Position;
				sb.AppendLine($"({pos?.x},{pos?.y},{pos?.z})");
				sb.AppendLine(_players[i]?.Character.LookDir.ToString());
			}
			LogMgr.Log(sb.ToString(), TraceSourceType.Game);
			sb.Clear();

			Broadcast(_packet);
			_currentTick++;
			yield return 0f;
		}
	}

	public void StartGame()
	{
		if (Interlocked.CompareExchange(ref _gameStarted, 1, 0) == 1) return;
		Program.Update += () => _coHandle.MoveNext();
	}

	public void Enter(Player player)
	{
		_enterBuffer.Enqueue(player);
		lock (_lock)
		{
			player.Session.RegisterSend(new S_EnterGame(_playerCount, player));
			_playerCount++;
			if (_playerCount == _maxPlayerCount) StartGame();
		}
	}

	public void Broadcast(BasePacket packet)
	{
		for (int i = 0; i < _maxPlayerCount; i++)
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
		// Todo object pooling
		_packet.Actions.Add(new GameActionResult(actionCode, subject, objects));
	}

	private void Exit(Player player)
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
