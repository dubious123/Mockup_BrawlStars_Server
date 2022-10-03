﻿namespace Server.Game.Characters.Dog
{
	public class Dog_Character : BaseCharacter
	{
		Dog_Bash _bash;
		public Dog_Character(GameRoom game, short teamId) : base(game, teamId)
		{

		}
		public override void HandleOneFrame()
		{
			base.HandleOneFrame();
			_bash.HandleOneFrame();
		}
		public override void HandleInput(in PlayerInput input)
		{
			base.HandleInput(input);
			_bash.HandleInput(((input.ButtonPressed >> 1) & 1) == 1);
			if (((input.ButtonPressed >> 2) & 1) == 1)
			{

			}
		}
		public override void SetOtherSkillsActive(uint skillId, bool active)
		{
			base.SetOtherSkillsActive(skillId, active);
			if (_bash.Id == skillId == false) _bash.SetActive(active);
		}
		public override void OnGetHit(in HitInfo info)
		{
			base.OnGetHit(info);
		}
		protected override void OnDead()
		{
			base.OnDead();
			//_bash.OnDead();
		}

	}
}
