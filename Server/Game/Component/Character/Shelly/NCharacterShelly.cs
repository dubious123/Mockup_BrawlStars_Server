using System.Collections;
using System.Collections.Generic;

using Server.Game;

using static Enums;

public class NCharacterShelly : NetCharacter
{
	public NetBaseSkill BuckShot { get; set; }
	public NetBaseSkill SuperShell { get; set; }

	public override void Start()
	{
		base.Start();
		BuckShot = new NShellyBuckShot(this);
		SuperShell = new NShellySuperShell(this);
	}

	public override void Reset()
	{
		base.Reset();
		MaxHp = 100;
		Hp = MaxHp;
	}

	public override void Update()
	{
		base.Update();
		BuckShot.Update();
		SuperShell.Update();
	}

	public override void UpdateInput(in InputData input)
	{
		base.UpdateInput(input);
		BuckShot.HandleInput(in input);
		SuperShell.HandleInput(in input);
	}

	public void SetActiveOtherSkills(NetBaseSkill from, bool Active)
	{
		if (from != BuckShot)
		{
			BuckShot.Active = Active;
		}

		if (from != SuperShell)
		{
			SuperShell.Active = Active;
		}
	}

	public override void OnDead()
	{
		base.OnDead();
		BuckShot.Performing = false;
		BuckShot.Active = false;
		SuperShell.Performing = false;
		SuperShell.Active = false;
	}

	protected override void OnCCStart()
	{
		base.OnCCStart();
		BuckShot.Cancel();
		BuckShot.Active = false;
		SuperShell.Cancel();
		SuperShell.Active = false;
	}

	protected override void OnCCEnd()
	{
		base.OnCCEnd();
		BuckShot.Active = true;
		SuperShell.Active = true;
	}
}
