namespace Server.Game.Characters.Base.Skill
{
	public abstract class BaseSkill
	{
		public uint Id { get; set; }
		protected BaseCharacter Character { get; set; }
		protected HitInfo SkillHitInfo { get; init; }
		protected bool Enabled { get; set; } = true;
		protected GameRoom Game { get; set; }
		public BaseSkill(BaseCharacter character, GameRoom game)
		{
			Character = character;
			Game = game;
			Id = 1;
		}

		public abstract void HandleInput(bool buttonPressed);
		public abstract void HandleOneFrame();
		public abstract void Cancel();
		public virtual void SetActive(bool set)
		{
			Enabled = set;
		}
	}
}
