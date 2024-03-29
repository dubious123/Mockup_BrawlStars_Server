﻿using Server.Game.GameRule;

namespace Server;
public class GameRoom
{
	public int Id { get; init; }
	public ushort MapId { get; init; }
	public bool GameStarted => _gameStarted;
	public Player[] Players { get => _players; }
	public GameState State => _state;

	private bool _gameStarted = false;
	private readonly object _lock = new();
	private readonly int _maxPlayerCount = Config.MAX_PLAYER_COUNT;
	private readonly ConcurrentQueue<Player> _enterBuffer = new();
	private Player[] _players;
	private NetWorld _world;
	private int _playerCount = 0;
	private GameState _state;
	private GameFrameInfo _frameInfo = new(Config.MAX_PLAYER_COUNT);

	public GameRoom(int id, ushort mapId)
	{
		Id = id;
		_players = new Player[_maxPlayerCount];
		_state = GameState.Waiting;
		MapId = mapId;
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

	public void HandleInput(Player player, in InputData data)
	{
		player.InputBuffer.Enqueue(data);
	}

	private void StartGame()
	{
		Loggers.Game.Information("---------------StartGame----------------");
		_gameStarted = true;
		_world = new(DataMgr.GetWorldData(), new GameRule00()
		{
			OnRoundEnd = OnRoundEnd,
			OnMatchOver = OnMatchOver,
			OnPlayerDead = OnPlayerDead,
		});

		for (int i = 0; _enterBuffer.TryDequeue(out var player); ++i)
		{
			_players[i] = player;
			player.TeamId = (short)i;
			player.Character = _world.ObjectBuilder.GetNewObject(player.CharacterType).GetComponent<NetCharacter>();
			player.CurrentGame = this;
			player.InputBuffer.Clear();
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

		HandleMatchStart();
		GameMgr.StartGame(this);
	}

	public void Update()
	{
		if (_state == GameState.Started)
		{
			foreach (var player in Players)
			{
				//todo
				if (player.InputBuffer.IsEmpty)
				{
					return;
				}
			}

			HandleOneFrame();
		}
		else if (_state == GameState.Waiting)
		{
			foreach (var player in Players)
			{
				while (player.InputBuffer.TryDequeue(out var input))
				{
					if (input.FrameNum != -Config.FRAME_BUFFER_COUNT)
					{
						continue;
					}

					if (player.InputBuffer.IsEmpty is false)
					{
						player.InputBuffer.TryPeek(out var invalid);
						Loggers.Debug.Error("forward Input from player [{0}] of frameNum {1}", player.TeamId, invalid.FrameNum);
						player.InputBuffer.Clear();
					}

					player.InputBuffer.Enqueue(input);
					goto Continue;
				}

				return;
			Continue:
				continue;
			}

			HandleRoundStart();
		}
	}

	private void HandleOneFrame()
	{
		Loggers.Game.Information("---------------Frame [{0}]----------------", _world.GameRule.FrameNum);
		//todo obj pooling
		var packet = new S_GameFrameInfo();
		foreach (var player in _players)
		{
			if (player is null)
			{
				continue;
			}

#if DEBUG
			Debug.Assert(player.InputBuffer.TryDequeue(out var input) && _world.GameRule.FrameNum == input.FrameNum);
#elif RELEASE
			player.InputBuffer.TryDequeue(out var input);
			if (_world.GameRule.FrameNum != input.FrameNum)
			{
				Loggers.Error.Error("from player [{0}] now : {1} but {2}", player.TeamId, _world.GameRule.FrameNum, input.FrameNum);
			}
#endif
			_frameInfo.Inputs[player.TeamId] = input;
			packet.C2STTime[player.TeamId] = input.C2STTime;
			packet.PlayerMoveDirXArr[player.TeamId] = input.MoveInput.x.RawValue;
			packet.PlayerMoveDirYArr[player.TeamId] = input.MoveInput.z.RawValue;
			packet.PlayerLookDirXArr[player.TeamId] = input.LookInput.x.RawValue;
			packet.PlayerLookDirYArr[player.TeamId] = input.LookInput.z.RawValue;
			packet.ButtonPressedArr[player.TeamId] = input.ButtonInput;
		}

		packet.FrameNum = _world.GameRule.FrameNum;
		packet.ServerSendTime = DateTime.UtcNow.ToFileTimeUtc();
		Broadcast(packet);
		_world.UpdateInputs(_frameInfo);
		_world.Update();
		foreach (var player in _world.CharacterSystem.ComponentDict)
		{
			Loggers.Game.Information("Player [{0}]", player.TeamId);
			Loggers.Game.Information("Position [{0:x},{1:x},{2:x}]] : ", player.Position.x.RawValue, player.Position.y.RawValue, player.Position.z.RawValue);
		}

		Loggers.Game.Information("------------------------------------------");
		if (_state == GameState.Waiting) //for logging
		{
			HandleRoundClear();
			HandleRoundReset();
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

	private void HandleMatchStart()
	{
		Loggers.Game.Information("Match Start");
		_world.Reset();
	}

	private void HandleRoundStart()
	{
		Loggers.Game.Information("Round Start");
		_state = GameState.Started;
	}

	private void OnRoundEnd(GameRule00.RoundResult result)
	{
		Loggers.Game.Information("Round End {0}", Enum.GetName(result));
		_state = GameState.Waiting;
	}

	private void HandleRoundClear()
	{
		Loggers.Game.Information("Round Clear");
		_world.Clear();
	}

	private void HandleRoundReset()
	{
		Loggers.Game.Information("Round Reset");
		foreach (var player in _players)
		{
			player.InputBuffer.Clear();
		}

		_world.Reset();
	}

	private void OnMatchOver(GameRule00.MatchResult result)
	{
		Loggers.Game.Information("Match Over {0}", Enum.GetName(result));
		EndGame();
	}

	private void OnPlayerDead(NetCharacter character)
	{
		Loggers.Game.Information("Player dead {0}", character.TeamId);
	}

	private void EndGame()
	{
		_state = GameState.Ended;
		//Broadcast(new S_BroadcastMatchOver());
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

		GameMgr.EndGame(Id);
	}
}
