namespace Server.Game
{
	public abstract class NetBaseSkill
	{
		public NetCharacter Character { get; set; }
		public NetWorld World => Character.World;
		public bool Active { get; set; } = true;
		public bool NeedRevert { get; protected set; }
		public bool IsAttack { get; protected set; }
		public int WaitFrameBeforePerform { get; protected set; }
		public int WaitFrameAfterPerform { get; protected set; }
		public int DelayFrameBetweenAttack { get; protected set; }
		public int PowerChargeAmount { get; protected set; }

		public NetBaseSkill(NetCharacter character)
		{
			Character = character;
		}

		public abstract void HandleInput(in InputData input);
		public abstract void Update();
		public abstract void Cancel();
		public abstract bool CanAttack();
		public abstract void Reset();
	}
}
