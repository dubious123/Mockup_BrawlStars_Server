namespace Server.Game
{
	public class NCharacterSpike : NetCharacter
	{
		public override void Start()
		{
			BasicAttack = new NSpikeNeedleGrenade(this);
			SpecialAttack = new NSpikeStickAround(this);
			base.Start();
			SpecialAttack.PowerUsagePerAttack = 0;
		}

		public override void Reset()
		{
			base.Reset();
			MaxHp = 250;
			Hp = MaxHp;
			MoveSpeed = (sfloat)6f;
		}
	}
}
