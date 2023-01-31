namespace Server.Game
{
	public abstract class NetBaseSkill
	{
		public NetCharacter Character { get; set; }
		public NetWorld World => Character.World;
		public bool Active { get; set; } = true;
		public abstract void HandleInput(in InputData input);
		public abstract void Update();
		public abstract void Cancel();
		public abstract bool CanAttack();
		public abstract void Reset();
		protected abstract IEnumerator<int> Co_Perform();
	}
}
