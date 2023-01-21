using System.Collections.Generic;
using System.Linq;

using Server.Game;

using static Enums;

public class NShellyBuckShot : NetBasicAttack
{
	public bool Holding { get; private set; }
	public int BulletAmountPerAttack => _palletCountPerShell;
	public sfloat BulletAngle => _bulletAngle;
	public NetProjectile[,] Shots { get; private set; }

	private IEnumerator<int> _coHandler;
	private readonly HitInfo _hitInfo;
	private int _palletCountPerShell;
	private sfloat _bulletAngle;

	private bool _nowPressed, _beforePressed;

	public NShellyBuckShot(NCharacterShelly character)
	{
		Character = character;
		MaxShellCount = 3;
		CurrentShellCount = MaxShellCount;
		_palletCountPerShell = 5;
		ReloadFrame = 120;
		WaitFrameBeforePerform = 10;
		WaitFrameAfterPerform = 10;
		_bulletAngle = (sfloat)30f;
		_hitInfo = new HitInfo()
		{
			Damage = 20,
		};

		Shots = new NetProjectile[MaxShellCount, _palletCountPerShell];
		var degreeOffset = 90 - _bulletAngle * 0.5f;
		var degreeDelta = _bulletAngle / _palletCountPerShell;

		for (int i = 0; i < MaxShellCount; i++)
		{
			for (int j = 0; j < _palletCountPerShell; ++j)
			{
				var obj = Character.ObjectBuilder.GetNewObject(NetObjectType.Projectile_Shelly_Buckshot);
				var pallet = obj.GetComponent<NetProjectile>()
					.SetAngle((degreeOffset + degreeDelta * j) * sMathf.Deg2Rad);
				obj.GetComponent<NetCollider2D>().OnCollisionEnter = target => OnHit(pallet, target);
				obj.Active = false;
				Shots[i, j] = pallet;
			}
		}
	}

	public override void Update()
	{
		if (Active is false)
		{
			return;
		}

		base.Update();

		if (Performing is true)
		{
			_coHandler.MoveNext();
			return;
		}
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 1) == 1;
		Holding = _beforePressed is true && _nowPressed is true;
		if (Performing)
		{
			return;
		}

		if (_beforePressed is true && _nowPressed is false && CurrentShellCount > 0)
		{
			Performing = true;
			Character.SetActiveOtherSkills(this, false);
			_coHandler = Co_Perform();
		}
	}

	protected override IEnumerator<int> Co_Perform()
	{
		base.OnPerform();
		Character.CanControlMove = false;
		Character.CanControlLook = false;
		for (int i = 0; i < WaitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		IsAttack = true;
		for (int i = 0; i < _palletCountPerShell; i++)
		{
			var bullet = Shots[CurrentShellCount, i];
			bullet.Reset();
			bullet.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			bullet.NetObj.Active = true;
		}

		yield return 0;

		IsAttack = false;
		for (int i = 0; i < WaitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		Character.CanControlMove = true;
		Character.CanControlLook = true;
		Character.SetActiveOtherSkills(this, true);
		Performing = false;
		yield break;
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	private void OnHit(NetProjectile pallet, NetCollider2D target)
	{
		var wall = target.GetComponent<NetWall>();
		if (wall is not null)
		{
			pallet.NetObj.Active = false;
			return;
		}

		var character = target.GetComponent<NetCharacter>();
		if (character is not null && Character.World.GameRule.CanSendHit(Character, character))
		{
			Character.SendHit(character, _hitInfo);
			pallet.NetObj.Active = false;
			return;
		}
	}
}
