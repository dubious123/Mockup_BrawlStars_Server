using static Enums;

namespace Server.Game
{
	public class NetCharacterKnight : NetCharacter
	{
		public INetBaseSkill Whirlwind { get; set; }
		public INetBaseSkill Bash { get; set; }

		public NetCharacterKnight(sVector3 position, sQuaternion rotation, NetWorld world)
			: base(position, rotation, NetObjectTag.Character, world)
		{
			Whirlwind = new NetKnightWhirlwind(this);
			Bash = new NetKnightBash(this);
			MaxHp = 100;
			Hp = MaxHp;
		}

		public override void Update()
		{
			base.Update();
			Whirlwind.Update();
			Bash.Update();
		}

		public override void UpdateInput(in InputData input)
		{
			base.UpdateInput(input);
			Whirlwind.HandleInput(in input);
			Bash.HandleInput(in input);
		}

		public void SetActiveOtherSkills(INetBaseSkill from, bool Active)
		{
			if (from != Whirlwind)
			{
				Whirlwind.Active = Active;
			}

			if (from != Bash)
			{
				Bash.Active = Active;
			}
		}

		public override void OnDead()
		{
			base.OnDead();
			Whirlwind.Performing = false;
			Whirlwind.Active = false;
			Bash.Performing = false;
			Bash.Active = false;
		}

		protected override void OnCCStart()
		{
			base.OnCCStart();
			Whirlwind.Cancel();
			Whirlwind.Active = false;
			Bash.Cancel();
			Bash.Active = false;
		}

		protected override void OnCCEnd()
		{
			base.OnCCEnd();
			Whirlwind.Active = true;
			Bash.Active = true;
		}
	}
}
