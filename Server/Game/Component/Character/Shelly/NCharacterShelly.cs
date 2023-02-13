public class NCharacterShelly : NetCharacter
{
	public override void Start()
	{
		BasicAttack = new NShellyBuckShot(this);
		SpecialAttack = new NShellySuperShell(this);
		base.Start();
		SpecialAttack.PowerUsagePerAttack = 0;
	}

	public override void Reset()
	{
		base.Reset();
		MaxHp = 100;
		Hp = MaxHp;
		MoveSpeed = (sfloat)6f;
	}
}
