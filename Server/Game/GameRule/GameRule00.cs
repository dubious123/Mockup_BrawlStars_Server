using System;

using static Enums;

namespace Server.Game.GameRule
{
	public class GameRule00 : BaseGameRule
	{
		public int MAX_ROUND_COUNT = Config.MAX_ROUND_COUNT;
		public int TEAM_MEMBER_COUNT = Config.TEAM_MEMBER_COUNT;
		public int REQUIRED_WIN_COUNT = Config.REQUIRED_WIN_COUNT;
		public int ROUND_END_WAIT_FRAMECOUNT = Config.ROUND_END_WAIT_FRAMECOUNT;
		public int ROUND_CLEAR_WAIT_FRAMECOUNT = Config.ROUND_CLEAR_WAIT_FRAMECOUNT;
		public int ROUND_RESET_WAIT_FRAMECOUNT = Config.ROUND_RESET_WAIT_FRAMECOUNT;
		public int MAX_FRAME_COUNT = Config.MAX_FRAME_COUNT; //60 * 60 * 3;
		public int CurrentRound { get; private set; } //0,1,2
		public int BlueWinCount { get; private set; }
		public int RedWinCount { get; private set; }
		public int BluePlayerDeadCount { get; private set; }
		public int RedPlayerDeadCount { get; private set; }
		public Action<RoundResult> OnRoundEnd { private get; set; }
		public Action<MatchResult> OnMatchOver { private get; set; }
		public Action<NetCharacter> OnPlayerDead { private get; set; }

		public GameRule00()
		{
			MaxFrameCount = MAX_FRAME_COUNT;
			CurrentRoundFrameCount = -Config.FRAME_BUFFER_COUNT;
		}

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

		public override void Update()
		{
			if (Active is false)
			{
				return;
			}

			++CurrentRoundFrameCount;
			var result = GetRoundResult();
			if (result == RoundResult.None)
			{
				return;
			}

			HandleRoundEnd(result);
		}

		public override void Reset()
		{
			Active = true;
			CurrentRoundFrameCount = -Config.FRAME_BUFFER_COUNT;
			BluePlayerDeadCount = 0;
			RedPlayerDeadCount = 0;
		}

		public override void OnCharacterDead(NetCharacter character)
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

		private void HandleRoundEnd(RoundResult roundResult)
		{
			if (roundResult == RoundResult.Blue)
			{
				BlueWinCount++;
			}
			else if (roundResult == RoundResult.Red)
			{
				RedWinCount++;
			}

			++CurrentRound;
			Active = false;
			if (BlueWinCount >= REQUIRED_WIN_COUNT || RedWinCount >= REQUIRED_WIN_COUNT || CurrentRound >= MAX_ROUND_COUNT)
			{
				HandleMatchOver();
			}
			else
			{
				OnRoundEnd?.Invoke(roundResult);
			}
		}

		private void HandleMatchOver()
		{
			OnMatchOver?.Invoke(GetMatchResult());
		}

		private RoundResult GetRoundResult()
		{
			RoundResult roundResult = RoundResult.None;
			if (CurrentRoundFrameCount >= MAX_FRAME_COUNT)
			{
				return RoundResult.Draw;
			}

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

		private MatchResult GetMatchResult() => BlueWinCount > RedWinCount ? MatchResult.Blue :
											  BlueWinCount < RedWinCount ? MatchResult.Red :
											  MatchResult.Draw;
		[Flags]
		public enum RoundResult
		{
			None = 0,
			Blue = 1,
			Red = 2,
			Draw = 3,
		}

		public enum MatchResult
		{
			None,
			Blue,
			Red,
			Draw
		}
	}
}
