using System.Net.WebSockets;
using System.Threading.Tasks;

using Server.Game.GameRule;
using Server.Logs;

namespace Server;
public class GameRoom
{
	public int Id { get; init; }
	public ushort MapId { get; init; }
	public bool GameStarted => _gameStarted;
	public Player[] Players { get => _players; }
	public GameState State { get; private set; }

	private bool _gameStarted = false;
	private readonly object _lock = new();
	private readonly int _maxPlayerCount = Config.MAX_PLAYER_COUNT;
	private readonly IEnumerator<float> _coGameLoop;
	private readonly ConcurrentQueue<Player> _enterBuffer = new();
	private Player[] _players;
	private NetWorld _world;
	private int _playerCount = 0;

	public GameRoom(int id, ushort mapId)
	{
		Id = id;
		_players = new Player[_maxPlayerCount];
		State = GameState.Waiting;
		MapId = mapId;
		_coGameLoop = Co_ServerGameLoop();
	}

	~GameRoom()
	{
		Loggers.Debug.Debug("GameRoom{0} disposed", Id);
	}

	public void Enter(Player player)
	{
		lock (_lock)
		{
#if DEBUG
			if (_playerCount >= _maxPlayerCount)
			{
				throw new Exception();
			}
#endif
			_enterBuffer.Enqueue(player);
			player.Session.RegisterSend(new S_EnterGame((short)_playerCount++, player));
			foreach (var p in _enterBuffer)
			{
				p.Session.RegisterSend(new S_BroadcastFoundPlayer(_playerCount));
			}

			if (_playerCount == _maxPlayerCount)
			{
				StartGame();
			}
		}
	}

	public void HandlePlayerInput(Player player, InputData input)
	{
		lock (_lock)
		{
			if (_gameStarted is false)
			{
				Loggers.Error.Error("game not started but trying to put input");
				throw new Exception();
			}

			if (_world.GameRule.CurrentRoundFrameCount < input.FrameNum)
			{
				Loggers.Debug.Debug("Discard input {0} from player {1}", input.FrameNum, player.TeamId);
				return;
			}

			player.InputBuffer.Enqueue(input);
			foreach (var p in _players)
			{
				if (p.InputBuffer.IsEmpty)
				{
					return;
				}
			}

			_coGameLoop.MoveNext();
		}
	}

	private void StartGame()
	{
		_gameStarted = true;
		State = GameState.Started;
		var data = DataMgr.GetWorldData();
		_world = new(data, new GameRule00()
		{
			OnMatchStart = OnMatchStart,
			OnRoundStart = OnRoundStart,
			OnRoundEnd = OnRoundEnd,
			OnRoundClear = OnRoundClear,
			OnRoundReset = OnRoundReset,
			OnMatchOver = OnMatchOver,
			OnPlayerDead = OnPlayerDead,
		});

		for (int i = 0; _enterBuffer.TryDequeue(out var player); ++i)
		{
			_players[i] = player;
			player.TeamId = (short)i;
			player.Character = _world.ObjectBuilder.GetNewObject(NetObjectType.Character_Shelly).GetComponent<NetCharacter>();
			player.CurrentGame = this;
			player.Session.OnClosed.AddListener("GameRoomExit", () =>
			{
				Exit(_players[player.TeamId]);
				player.Session.OnClosed.RemoveListener("GameRoomExit");
			});
		}

		Broadcast(new S_BroadcastStartGame()
		{
			CharacterTypeArr = _players.Select(p => (ushort)(p.Character.NetObj.Tag)).ToArray(),
		});
	}

	private IEnumerator<float> Co_ServerGameLoop()
	{
		_world.Reset();
		GameFrameInfo frameInfo = new(_maxPlayerCount);
		Loggers.Game.Information("---------------StartGame----------------");
		while (State == GameState.Started)
		{
			Loggers.Game.Information("---------------Frame [{0}]----------------", _world.GameRule.CurrentRoundFrameCount);
			S_GameFrameInfo packet = new();
			foreach (var player in _players)
			{
				if (player is null)
				{
					continue;
				}

				if (player.InputBuffer.TryDequeue(out var input) is false || _world.GameRule.CurrentRoundFrameCount != input.FrameNum)
				{
					throw new Exception();
				}

				frameInfo.Inputs[player.TeamId] = input;
				packet.PlayerMoveDirXArr[player.TeamId] = input.MoveInput.x.RawValue;
				packet.PlayerMoveDirYArr[player.TeamId] = input.MoveInput.z.RawValue;
				packet.PlayerLookDirXArr[player.TeamId] = input.LookInput.x.RawValue;
				packet.PlayerLookDirYArr[player.TeamId] = input.LookInput.z.RawValue;
				packet.ButtonPressedArr[player.TeamId] = input.ButtonInput;
			}

			packet.FrameNum = _world.GameRule.CurrentRoundFrameCount;
			_world.UpdateInputs(frameInfo);
			_world.Update();
			Broadcast(packet);
			Loggers.Game.Information("------------------------------------------");
			yield return 0f;
		}
	}

	private void Broadcast(BasePacket packet)
	{
		for (int i = 0; i < _maxPlayerCount; i++)
		{
			var player = _players[i];
			if (player is null) continue;
			player.Session.RegisterSend(packet);
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

		if (_playerCount == 0)
		{
			EndGame();
		}
	}

	private void OnMatchStart()
	{
		Loggers.Game.Information("Match Start");
	}

	private void OnRoundStart()
	{
		Loggers.Game.Information("Round Start");
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(result));
	}

	private void OnRoundClear()
	{
		Loggers.Game.Information("Round Clear");
	}

	private void OnRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		foreach (var player in _players)
		{
			player.InputBuffer.Clear();
		}
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(result));
		EndGame();
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Loggers.Game.Information("Player dead {0}", character.NetObjId.InstanceId);
	}

	private void EndGame()
	{
		State = GameState.Ended;
		Broadcast(new S_BroadcastMatchOver());
		foreach (var p in _players)
		{
			if (p is null)
			{
				continue;
			}

			p.GameSceneReady = false;
			p.Session.OnClosed.RemoveListener("GameRoomExit");
			p.CurrentGame = null;
		}

		Program.Update -= () => _coGameLoop.MoveNext();
		GameMgr.EndGame(Id);
	}
}
