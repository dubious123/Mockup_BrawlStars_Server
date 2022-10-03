namespace Server.Game.Characters.Base.Skill
{
	public abstract class BaseSkill
	{
		public uint Id;
		protected BaseCharacter _character;
		protected HitInfo _hitInfo;
		protected bool _enabled = true;
		protected GameRoom _game;
		protected HitInfo _info;
		public BaseSkill(BaseCharacter character, GameRoom game)
		{
			_character = character;
			_game = game;
			Id = 1;
		}
		public abstract void HandleInput(bool buttonPressed);
		public abstract void HandleOneFrame();
		public abstract void Cancel();
		public virtual void SetActive(bool set)
		{
			_enabled = set;
		}
	}
}
