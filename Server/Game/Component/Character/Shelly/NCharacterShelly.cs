public class NCharacterShelly : NetCharacter
{
	public override void Start()
	{
		BasicAttack = new NShellyBuckShot(this);
		SpecialAttack = new NShellySuperShell(this);
		base.Start();
	}

	public override void Reset()
	{
		base.Reset();
		MaxHp = 3800;
		Hp = MaxHp;
		MoveSpeed = (sfloat)6f;
	}
}
