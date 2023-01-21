using System.Collections;
using System.Collections.Generic;

using Server.Game;

using static Enums;

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
		if (BasicAttack is not null)
		{
			BasicAttack.Active = true;
		}

		if (SuperShell is not null)
		{
			SuperShell.Active = true;
		}
	}

	public override void Update()
	{
		base.Update();
		BasicAttack.Update();
		SuperShell.Update();
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
		BasicAttack.Performing = false;
		BasicAttack.Active = false;
		SuperShell.Performing = false;
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
