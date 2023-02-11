public class NShellySuperShell : NetSpecialAttack
{
	private readonly int _palletCountPerShell;
	private readonly sfloat _bulletAngle, _degreeOffset, _degreeDelta;
	private readonly HitInfo _hitInfo;

	private int _coHandle, _currentDelay;
	private bool _nowPressed, _beforePressed;

	public NShellySuperShell(NCharacterShelly character) : base(character)
	{
		_palletCountPerShell = 9;
		WaitFrameBeforePerform = 2;
		WaitFrameAfterPerform = 2;
		DelayFrameBetweenAttack = 5;
		PowerUsagePerAttack = 100;
		MaxPowerAmount = 100;
		_bulletAngle = (sfloat)40f;
		_hitInfo = new HitInfo()
		{
			Damage = 25,
			PowerChargeAmount = 20,
			KnockbackDuration = 20,
			KnockbackDistance = (sfloat)0.5f
		};

		_degreeOffset = 90 - _bulletAngle * 0.5f;
		_degreeDelta = _bulletAngle / _palletCountPerShell;
		World.ProjectileSystem.Reserve(NetObjectType.Projectile_Shelly_SuperShell, _palletCountPerShell);
	}

	public override void Update()
	{
		_currentDelay = Math.Min(_currentDelay + 1, DelayFrameBetweenAttack);
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 2) != 0;
#if CLIENT
		Holding = _beforePressed is true && _nowPressed is true && CanAttack();
#endif
		IsAttack = _beforePressed is true && _nowPressed is false && CanAttack();
		if (IsAttack is false)
		{
			return;
		}

		_coHandle = World.NetTiming.RunCoroutine(Co_Perform());
	}

	public override bool CanAttack()
	{
		return base.CanAttack() && _currentDelay >= DelayFrameBetweenAttack;
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	public override void Reset()
	{
		Active = true;
		_beforePressed = _nowPressed = false;
		_currentDelay = 0;
		CurrentPowerAmount = 0;
	}

	protected override void OnPerform()
	{
		base.OnPerform();
		_currentDelay = 0;
		Character.CanControlMove = false;
		Character.CanControlLook = false;
	}

	private IEnumerator<int> Co_Perform()
	{
		OnPerform();
		for (int i = 0; i < WaitFrameBeforePerform; i++)
		{
			yield return 0;
		}

		World.ProjectileSystem.Awake(NetObjectType.Projectile_Shelly_SuperShell, _palletCountPerShell, (count, pallet) =>
		{
			pallet.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			pallet.SetAngle((_degreeOffset + _degreeDelta * count) * sMathf.Deg2Rad).SetOwner(Character.NetObj);
			pallet.Collider.OnCollisionEnter = (target) => OnHit(pallet, target);
			pallet.NetObj.Active = true;
		});

		for (int i = 0; i < WaitFrameAfterPerform; i++)
		{
			yield return 0;
		}

		Character.CanControlMove = true;
		Character.CanControlLook = true;
		Character.SetActiveOtherSkills(this, true);
		yield break;
	}

	private void OnHit(NetProjectile pallet, NetCollider2D target)
	{
		var wall = target.GetComponent<NetWall>();
		if (wall is not null)
		{
			goto Return;
		}

		var character = target.GetComponent<NetCharacter>();
		if (character is not null && World.GameRule.CanSendHit(Character, character))
		{
			Character.SendHit(character, _hitInfo);
			goto Return;
		}

		return;
	Return:
		pallet.Reset();
		World.ProjectileSystem.Return(pallet);
	}
}
