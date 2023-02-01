namespace Server.Game
{
	public abstract class NetSpecialAttack : NetBaseSkill
	{
#if CLIENT
		public bool Holding { get; protected set; }
#endif
		public int MaxPowerAmount { get; set; }
		public int PowerUsagePerAttack { get; set; }
		public int CurrentPowerAmount { get; set; }

		protected NetSpecialAttack(NetCharacter character) : base(character) { }

		public override bool CanAttack() => CurrentPowerAmount >= PowerUsagePerAttack && Active;

		public virtual void ChargePower(int amount)
		{
			CurrentPowerAmount = Math.Min(CurrentPowerAmount + amount, MaxPowerAmount);
		}

		protected virtual void OnPerform()
		{
			CurrentPowerAmount -= PowerUsagePerAttack;
		}
	}
}
