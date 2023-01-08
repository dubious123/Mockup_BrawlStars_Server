using System.Collections.Generic;
using System.Linq;

using Server.Game;

using static Enums;

public class NShellySuperShell : NetBaseSkill
{
	public bool Holding { get; private set; }
	public bool IsAttack { get; private set; }
	public int PalletsCountPerShot => _palletCountPerShell;
	public sfloat BulletAngle => _bulletAngle;
	public NetProjectile[] Pallets { get; private set; }

	private IEnumerator<int> _coHandler;
	private readonly NCharacterShelly _shelly;
	private readonly HitInfo _hitInfo;
	private int _palletCountPerShell, _reloadFrame, _currentReloadDeltaFrame,
		_waitFrameBeforePerform, _waitFrameAfterPerform;
	private sfloat _bulletAngle;

	private bool _nowPressed, _beforePressed;

	public NShellySuperShell(NCharacterShelly character)
	{
		_shelly = character;
		_palletCountPerShell = 9;
		_reloadFrame = 60;
		_waitFrameBeforePerform = 10;
		_waitFrameAfterPerform = 10;
		_bulletAngle = (sfloat)50f;
		_hitInfo = new HitInfo()
		{
			Damage = 25,
			KnockbackDuration = 20,
			KnockbackDistance = (sfloat)0.5f
		};

		Pallets = new NetProjectile[_palletCountPerShell];
		var degreeOffset = 90 - _bulletAngle * 0.5f;
		var degreeDelta = _bulletAngle / _palletCountPerShell;
		for (int j = 0; j < _palletCountPerShell; ++j)
		{
			var obj = _shelly.ObjectBuilder.GetNewObject(NetObjectType.Projectile_Shelly_Buckshot);
			Pallets[j] = obj.GetComponent<NetProjectile>()
				.SetAngle((degreeOffset + degreeDelta * j) * sMathf.Deg2Rad);
		}
	}

	public override void Update()
	{
		if (Active is false)
		{
			return;
		}

		HandleInputInternal();

		if (_currentReloadDeltaFrame < _reloadFrame)
		{
			++_currentReloadDeltaFrame;
		}
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 2) == 1;
	}

	protected override IEnumerator<int> Co_Perform()
	{
		_shelly.CanControlMove = false;
		_shelly.CanControlLook = false;
		for (int i = 0; i < _waitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		IsAttack = true;

		foreach (var pallet in Pallets)
		{
			pallet.Reset();
			pallet.NetObj.SetPositionAndRotation(_shelly.Position, _shelly.Rotation);
			pallet.Active = true;
		}

		yield return 0;

		IsAttack = false;
		for (int i = 0; i < _waitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		_shelly.CanControlMove = true;
		_shelly.CanControlLook = true;
		_shelly.SetActiveOtherSkills(this, true);
		Performing = false;
		yield break;
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	private void HandleInputInternal()
	{
		if (Performing is true)
		{
			_coHandler.MoveNext();
			return;
		}

		Holding = _beforePressed is true && _nowPressed is true;
		if (_beforePressed is true && _nowPressed is false && _currentReloadDeltaFrame > _reloadFrame)
		{
			_shelly.SetActiveOtherSkills(this, false);
			Performing = true;
			_coHandler = Co_Perform();
		}
	}

	private void OnHit(NetProjectile pallet, NetCollider2D target)
	{
		var wall = target.GetComponent<NetWall>();
		if (wall is not null)
		{
			pallet.Active = false;
			return;
		}

		var character = target.GetComponent<NetCharacter>();
		if (character is not null && _shelly.World.GameRule.CanSendHit(_shelly, character))
		{
			_shelly.SendHit(character, _hitInfo);
			pallet.Active = false;
			return;
		}
	}
}
