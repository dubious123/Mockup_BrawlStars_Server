public class NCharacterShelly : NetCharacter
{
	public override void Start()
	{
		base.Start();
		BasicAttack = new NShellyBuckShot(this);
		SpecialAttack = new NShellySuperShell(this);
	}

	public override void Reset()
	{
		base.Reset();
		MaxHp = 100;
		Hp = MaxHp;
		BasicAttack?.Reset();
		SpecialAttack?.Reset();
	}

	public override void Update()
	{
		BasicAttack.Update();
		SpecialAttack.Update();
		base.Update();
	}

	public override void UpdateInput(in InputData input)
	{
		base.UpdateInput(input);
		BasicAttack.HandleInput(in input);
		SpecialAttack.HandleInput(in input);
	}

	public override void SetActiveOtherSkills(NetBaseSkill from, bool Active)
	{
		if (from != BasicAttack)
		{
			BasicAttack.Active = Active;
		}

		if (from != SpecialAttack)
		{
			SpecialAttack.Active = Active;
		}
	}

	public override void OnDead()
	{
		base.OnDead();
		BasicAttack.Active = false;
		SpecialAttack.Active = false;
	}

	protected override void OnCCStart()
	{
		base.OnCCStart();
		BasicAttack.Cancel();
		BasicAttack.Active = false;
		SpecialAttack.Cancel();
		SpecialAttack.Active = false;
	}

	protected override void OnCCEnd()
	{
		base.OnCCEnd();
		BasicAttack.Active = true;
		SpecialAttack.Active = true;
	}
}
