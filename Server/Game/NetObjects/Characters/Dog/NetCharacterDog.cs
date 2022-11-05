using static Enums;

namespace Server.Game
{
	public class NetCharacterDog : NetCharacter
	{
		public INetBaseSkill BasicAttack { get; set; }
		public INetBaseSkill Bash { get; set; }

		public NetCharacterDog(sVector3 position, sQuaternion rotation, NetWorld world)
			: base(position, rotation, NetObjectTag.Character, world)
		{
			BasicAttack = new NetDogBasicAttack(this);
			Bash = new NetDogBash(this);
			MaxHp = 100;
			Hp = MaxHp;
		}

		public override void Update()
		{
			base.Update();
			BasicAttack.Update();
			Bash.Update();
		}

		public override void UpdateInput(in InputData input)
		{
			base.UpdateInput(input);
			BasicAttack.HandleInput(in input);
			Bash.HandleInput(in input);
		}

		public void SetActiveOtherSkills(INetBaseSkill from, bool Active)
		{
			if (from != BasicAttack)
			{
				BasicAttack.Active = Active;
			}

			if (from != Bash)
			{
				Bash.Active = Active;
			}
		}

		public override void OnDead()
		{
			base.OnDead();
			BasicAttack.Performing = false;
			BasicAttack.Active = false;
			Bash.Performing = false;
			Bash.Active = false;
		}

		protected override void OnCCStart()
		{
			base.OnCCStart();
			BasicAttack.Cancel();
			BasicAttack.Active = false;
			Bash.Cancel();
			Bash.Active = false;
		}

		protected override void OnCCEnd()
		{
			base.OnCCEnd();
			BasicAttack.Active = true;
			Bash.Active = true;
		}
	}
}
