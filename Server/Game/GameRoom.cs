using System.Threading.Tasks;

using Server.Game.GameRule;
using Server.Logs;

namespace Server;

public class GameRoom
{
	public int Id { get; init; }
	public ushort MapId { get; init; }
	public bool GameStarted => _gameStarted == 1;
	public long CurrentTick { get => _currentTick; }
	public Player[] Players { get => _players; }
	public GameState State { get; private set; }

	private readonly object _lock = new();
	private readonly int _maxPlayerCount = 2;
	private readonly IEnumerator<float> _coHandle;
	private readonly JobQueue _sendQueue;
	private readonly ConcurrentQueue<Player> _enterBuffer = new();
	private int _gameStarted = 0;
	private short _playerCount = 0;
	private long _currentTick = 0;
	private Player[] _players;
	private NetWorld _world;

	public GameRoom(int id, ushort mapId)
	{
		Id = id;
		_players = new Player[_maxPlayerCount];
		State = GameState.Waiting;
		MapId = mapId;
		_sendQueue = JobMgr.GetQueue(Define.PacketSendQueueName);
		_coHandle = Co_Update();
	}

	~GameRoom()
	{
		Program.Update -= () => _coHandle.MoveNext();
	}


	private IEnumerator<float> Co_Update()
	{
		Player player;
		_playerCount = 0;
		var data = DataMgr.GetWorldData();
		_world = new(data, new GameRule00()
		{
			OnMatchOver = OnMatchOver,
			OnRoundEnd = OnRoundEnd,
			OnRoundStart = OnRoundStart,
		});

		#region Ready Game
		while (_enterBuffer.TryDequeue(out var p))
		{
			_players[_playerCount] = p;
			p.TeamId = _playerCount;
			p.Character = _world.ObjectBuilder.GetNewObject(NetObjectType.Character_Shelly).GetComponent<NetCharacter>();// (_playerCount, CharacterType.Knight);
			p.CurrentGame = this;
			p.Session.OnClosed.AddListener("GameRoomExit", () =>
			{
				Exit(_players[p.TeamId]);
				p.Session.OnClosed.RemoveListener("GameRoomExit");
			});

			_playerCount++;
		}
		#endregion

		Broadcast(new S_GameReady());

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
			CharacterTypeArr = _players.Select(p => (ushort)(p.Character.NetObj.Tag)).ToArray(),
		});
		#endregion

		_world.OnWorldStart();
		yield return 0f;

		Loggers.Game.Information("---------------StartGame----------------");
		GameFrameInfo frameInfo = new(_maxPlayerCount);
		while (State == GameState.Started)
		{
			Loggers.Game.Information("---------------Frame [{0}]----------------", _currentTick);
			S_GameFrameInfo packet = new();
			while (_world.Active is false)
			{
				yield return 0f;
				continue;
			}

			for (int i = 0; i < _maxPlayerCount; i++)
			{
				player = _players[i];
				if (player is null)
				{
					continue;
				}

				while (player.InputBuffer.IsEmpty && State == GameState.Started)
				{
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

			_world.InputInfo = frameInfo;
			_world.Update();
			Broadcast(packet);
			_currentTick++;
			Loggers.Game.Information("------------------------------------------");
			yield return 0f;
		}
	}

	public void StartGame()
	{
		if (Interlocked.CompareExchange(ref _gameStarted, 1, 0) == 1)
		{
			return;
		}

		State = GameState.Started;
		Program.Update += () => _coHandle.MoveNext();
	}

	public void Enter(Player player)
	{
		_enterBuffer.Enqueue(player);
		lock (_lock)
		{
			player.Session.RegisterSend(new S_EnterGame(_playerCount++, player));
			foreach (var p in _enterBuffer)
			{
				p.Session.RegisterSend(new S_BroadcastSearchPlayer((ushort)_playerCount));
			}

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

	public void StartNewRound()
	{
		_world.Reset();
		_world.Active = true;
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

		if (_playerCount == 0)
		{
			EndGame();
		}
	}

	private void OnRoundStart()
	{
		Loggers.Game.Information("Round Start");
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(result));
		var waitMilliseconds = 3000;
		Broadcast(new S_BroadcastStartNewRound()
		{
			WaitMilliseconds = waitMilliseconds
		});

		Task.Delay(waitMilliseconds).ContinueWith(t => StartNewRound());
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match End {0}", Enum.GetName(result));
	}

	private void EndGame()
	{
		State = GameState.Ended;
		GameMgr.EndGame(Id);
	}
}
