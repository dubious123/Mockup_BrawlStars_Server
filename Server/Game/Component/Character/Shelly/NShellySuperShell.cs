public class NShellySuperShell : NetSpecialAttack
{
	public bool Holding { get; private set; }
	public bool IsAttack { get; private set; }
	public int PalletsCountPerShot => _palletCountPerShell;
	public sfloat BulletAngle => _bulletAngle;
	public NetProjectile[] Pallets { get; private set; }

	private IEnumerator<int> _coHandler;
	private readonly HitInfo _hitInfo;
	private int _palletCountPerShell, _waitFrameBeforePerform, _waitFrameAfterPerform;
	private sfloat _bulletAngle;

	private bool _nowPressed, _beforePressed;

	public NShellySuperShell(NCharacterShelly character)
	{
		Character = character;
		MaxPowerAmount = 200;
		PowerUsagePerAttack = MaxPowerAmount;
		CurrentPowerAmount = 0;
		_palletCountPerShell = 9;
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
			var obj = Character.ObjectBuilder.GetNewObject(NetObjectType.Projectile_Shelly_SuperShell);
			var pallet = obj.GetComponent<NetProjectile>()
				.SetAngle((degreeOffset + degreeDelta * j) * sMathf.Deg2Rad);
			obj.GetComponent<NetCollider2D>().OnCollisionEnter = target => OnHit(pallet, target);
			obj.Active = false;
			Pallets[j] = pallet;
		}
	}

	public override void Update()
	{
		if (Active is false)
		{
			return;
		}

		HandleInputInternal();
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 2) == 1;
	}

	protected override IEnumerator<int> Co_Perform()
	{
		Character.CanControlMove = false;
		Character.CanControlLook = false;
		for (int i = 0; i < _waitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		IsAttack = true;

		foreach (var pallet in Pallets)
		{
			pallet.Reset();
			pallet.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			pallet.NetObj.Active = true;
		}

		yield return 0;

		IsAttack = false;
		for (int i = 0; i < _waitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		Character.CanControlMove = true;
		Character.CanControlLook = true;
		Character.SetActiveOtherSkills(this, true);
		yield break;
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	private void HandleInputInternal()
	{
		//if (Performing is true)
		//{
		//	_coHandler.MoveNext();
		//	return;
		//}

		Holding = _beforePressed is true && _nowPressed is true;
		//if (_beforePressed is true && _nowPressed is false && CanAttack)
		//{
		//	Character.SetActiveOtherSkills(this, false);
		//	Performing = true;
		//	_coHandler = Co_Perform();
		//}
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
		if (character is not null && Character.World.GameRule.CanSendHit(Character, character))
		{
			Character.SendHit(character, _hitInfo);
			pallet.NetObj.Active = false;
			return;
		}
	}

	public override void Reset()
	{
		throw new NotImplementedException();
	}
}
