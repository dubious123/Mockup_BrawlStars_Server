using Server.Logs;

namespace Server;

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
	private Player[] _players;

	public GameRoom(int id, ushort mapId)
	{
		Id = id;
		_players = new Player[_maxPlayerCount];
		State = GameState.Waiting;
		MapId = mapId;
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

	private IEnumerator<float> Co_Update()
	{
		Player player;
		_playerCount = 0;
		var data = DataMgr.GetWorldData();
		NetWorld world = new(data);

		#region Ready Game
		while (_enterBuffer.TryDequeue(out var p))
		{
			_players[_playerCount] = p;
			p.TeamId = _playerCount;
			p.Character = new NetCharacterKnight(data.SpawnPoints[p.TeamId], sQuaternion.identity, world);
			world.AddNewNetObject((uint)p.TeamId, p.Character);
			p.CharType = CharacterType.Knight;
			p.CurrentGame = this;
			p.Session.OnClosed.AddListener("GameRoomExit", () =>
			{
				Exit(_players[p.TeamId]);
				p.Session.OnClosed.RemoveListener("GameRoomExit");
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


		#region Broadcast start game
		Broadcast(new S_BroadcastStartGame(0f)
		{
			CharacterTypeArr = _players.Select(p => (ushort)(p.CharType)).ToArray(),
		});
		#endregion

		yield return 0f;

		Loggers.Game.Information("---------------StartGame----------------");
		GameFrameInfo frameInfo = new(_maxPlayerCount);
		while (true)
		{
			Loggers.Game.Information("---------------Frame [{0}]----------------", _currentTick);
			S_GameFrameInfo packet = new();
			for (int i = 0; i < _maxPlayerCount; i++)
			{
				player = _players[i];
				if (player is null)
				{
					continue;
				}

				while (player.InputBuffer.IsEmpty)
				{
					// todo
				}

				player.InputBuffer.TryDequeue(out var input);
				for (int j = player.InputBuffer.Count; j > 0; j--)
				{
					player.InputBuffer.TryDequeue(out var temp);
					InputData.Combine(in temp, in input, out input);
				}

				frameInfo.Inputs[i] = input;
				packet.PlayerMoveDirXArr[i] = input.MoveInput.x.RawValue;
				packet.PlayerMoveDirYArr[i] = input.MoveInput.z.RawValue;
				packet.PlayerLookDirXArr[i] = input.LookInput.x.RawValue;
				packet.PlayerLookDirYArr[i] = input.LookInput.z.RawValue;
				packet.ButtonPressedArr[i] = input.ButtonInput;
			}

			world.InputInfo = frameInfo;
			world.Update();
			Broadcast(packet);
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
			if (_playerCount == _maxPlayerCount)
			{
				StartGame();
			}
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

	private void Exit(Player player)
	{
		if (_players[player.TeamId] != player)
		{
			Loggers.Console.Error("ERRRRRRRRRRRRRRRRRRRRRRRRrrrrrrrrrrrrrrr");
			//throw new System.Exception();
		}

		_players[player.TeamId] = null;
		_playerCount--;
	}
}
