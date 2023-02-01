public class NCharacterShelly : NetCharacter
{
	public NetBaseSkill SuperShell { get; set; }

	public override void Start()
	{
		base.Start();
		BasicAttack = new NShellyBuckShot(this);
		SuperShell = new NShellySuperShell(this);
	}

	public override void Reset()
	{
		base.Reset();
		MaxHp = 100;
		Hp = MaxHp;
		BasicAttack?.Reset();

		if (SuperShell is not null)
		{
			SuperShell.Active = true;
		}
	}

	public override void Update()
	{
		BasicAttack.Update();
		SuperShell.Update();
		base.Update();
	}

	public override void UpdateInput(in InputData input)
	{
		base.UpdateInput(input);
		BasicAttack.HandleInput(in input);
		SuperShell.HandleInput(in input);
	}

	public override void SetActiveOtherSkills(NetBaseSkill from, bool Active)
	{
		if (from != BasicAttack)
		{
			BasicAttack.Active = Active;
		}

		if (from != SuperShell)
		{
			SuperShell.Active = Active;
		}
	}

	public override void OnDead()
	{
		base.OnDead();
		BasicAttack.Active = false;
		SuperShell.Active = false;
	}

	protected override void OnCCStart()
	{
		base.OnCCStart();
		BasicAttack.Cancel();
		BasicAttack.Active = false;
		SuperShell.Cancel();
		SuperShell.Active = false;
	}

	protected override void OnCCEnd()
	{
		base.OnCCEnd();
		BasicAttack.Active = true;
		SuperShell.Active = true;
	}
}
