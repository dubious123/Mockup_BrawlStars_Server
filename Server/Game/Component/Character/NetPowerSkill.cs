namespace Server.Game
{
	public abstract class NetSpecialAttack : NetBaseSkill
	{
		public int MaxPowerAmount { get; set; }
		public int PowerUsagePerAttack { get; set; }
		public int CurrentPowerAmount { get; set; }
		public override bool CanAttack() => CurrentPowerAmount >= PowerUsagePerAttack;

		public override void Update()
		{
		}

		public virtual void ChargePower(int amount)
		{
			CurrentPowerAmount = Math.Min(CurrentPowerAmount + amount, MaxPowerAmount);
		}

		protected virtual void OnPerform()
		{
		}
	}
}
