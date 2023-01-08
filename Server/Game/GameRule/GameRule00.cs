using System;

using Server.Logs;

using static Enums;
using static Server.Game.GameRule.GameRule00;

namespace Server.Game.GameRule
{
	public class GameRule00 : BaseGameRule
	{
		public const int MAX_ROUND_COUNT = 3;
		public const int TEAM_MEMBER_COUNT = 1;//3;
		public const int REQUIRED_WIN_COUNT = 2;
		public const int ROUND_WAIT_FRAMECOUNT = 60;

		//public NetCharacter[] NetCharacters => World.NetCharacters;
		public Action<RoundResult> OnRoundEnd { private get; set; }
		public Action<GameResult> OnGameEnd { private get; set; }
		public Action<NetCharacter> OnPlayerDead { private get; set; }
		public Action OnRoundStart { private get; set; }
		public int CurrentRound { get; private set; } //0,1,2
		public int BlueWinCount { get; private set; }
		public int RedWinCount { get; private set; }
		public int BluePlayerDeadCount { get; private set; }
		public int RedPlayerDeadCount { get; private set; }
		public int CurrentRoundWaitFrameCount { get; private set; }
		public int CurrentRoundFrameCount { get; private set; }

		private bool _gameStarted = false;

		public override TeamType GetTeamType(NetObject netObj)
		{
			var instanceId = netObj.ObjectId.InstanceId;
			if (instanceId < 6)
			{
				return instanceId % 2 == 0 ? TeamType.Blue : TeamType.Red;
			}

			Loggers.Error.Error("Something is wrong");
			return TeamType.None;
		}

		public override bool CanSendHit(NetBaseComponent from, NetBaseComponent to)
		{
			if (from is NetCharacter && to is NetCharacter)
			{
				var target = to as NetCharacter;
				return (from as NetCharacter).Team != target.Team && target.Active;
			}

			return to is ITakeHit;
		}

		public override void UpdateGameLogic()
		{
			if (_gameStarted is false)
			{
				HandleGameStart();
			}

			if (CurrentRoundWaitFrameCount > 0)
			{
				--CurrentRoundWaitFrameCount;
				return;
			}

			if (CurrentRoundFrameCount++ == 0)
			{
				HandleRoundStart();
			}

			var roundResult = GetRoundResult();
			if (roundResult == RoundResult.None)
			{
				return;
			}

			HandleRoundEnd(roundResult);
			if (BlueWinCount >= REQUIRED_WIN_COUNT && RedWinCount >= REQUIRED_WIN_COUNT && CurrentRound >= MAX_ROUND_COUNT)
			{
				HandleGameEnd();
			}
		}

		private void HandleGameStart()
		{
			foreach (var player in World.CharacterSystem.ComponentDict.Values)
			{
				if (player is null)
				{
					continue;
				}

				player.OnCharacterDead = () => HandlePlayerDead(player);
			}

			_gameStarted = true;
		}

		private void HandleGameEnd()
		{
			OnGameEnd?.Invoke(GetGameResult());
			foreach (var player in World.CharacterSystem.ComponentDict.Values)
			{
				if (player is null)
				{
					continue;
				}

				player.Active = false;
			}
		}

		private void HandleRoundStart()
		{
			OnRoundStart?.Invoke();
			BluePlayerDeadCount = 0;
			RedPlayerDeadCount = 0;
		}

		private void HandleRoundEnd(RoundResult roundResult)
		{
			OnRoundEnd?.Invoke(roundResult);
			CurrentRoundFrameCount = 0;
			CurrentRoundWaitFrameCount = 300;
			if (roundResult == RoundResult.Blue)
			{
				BlueWinCount++;
			}
			else
			{
				RedWinCount++;
			}
		}

		private void HandlePlayerDead(NetCharacter character)
		{
			OnPlayerDead?.Invoke(character);
			character.Active = false;

			if (character.Team == TeamType.Blue)
			{
				++BluePlayerDeadCount;
			}
			else
			{
				++RedPlayerDeadCount;
			}
		}

		private RoundResult GetRoundResult()
		{
			RoundResult roundResult = RoundResult.None;
			if (BluePlayerDeadCount == TEAM_MEMBER_COUNT)
			{
				roundResult |= RoundResult.Red;
			}

			if (RedPlayerDeadCount == TEAM_MEMBER_COUNT)
			{
				roundResult |= RoundResult.Blue;
			}

			return roundResult;
		}

		private GameResult GetGameResult() => BlueWinCount > RedWinCount ? GameResult.Blue :
											  BlueWinCount < RedWinCount ? GameResult.Red :
											  GameResult.Draw;
		[Flags]
		public enum RoundResult
		{
			None = 0,
			Blue = 1,
			Red = 2,
			Draw = 3,
		}

		public enum GameResult
		{
			None,
			Blue,
			Red,
			Draw
		}
	}
}
