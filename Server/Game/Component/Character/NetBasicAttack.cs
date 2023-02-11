namespace Server.Game
{
	public abstract class NetBasicAttack : NetBaseSkill
	{
		public int CurrentShellCount { get; protected set; }
		public int MaxShellCount { get; protected set; }
		public int CurrentReloadDeltaFrame { get; protected set; }
		public int ReloadFrame { get; protected set; }

		protected NetBasicAttack(NetCharacter character) : base(character) { }

		public override void Update()
		{
			if (CurrentShellCount >= MaxShellCount)
			{
				return;
			}

			if (CurrentReloadDeltaFrame < ReloadFrame)
			{
				++CurrentReloadDeltaFrame;
			}
			else
			{
				++CurrentShellCount;
				CurrentReloadDeltaFrame = 0;
			}
		}

		public virtual void OnPerform()
		{
			--CurrentShellCount;
		}

		public override bool CanAttack()
		{
			return Active && CurrentShellCount > 0;
		}
	}
}
