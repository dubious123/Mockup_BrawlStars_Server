public class NShellyBuckShot : NetBasicAttack
{
	public int BulletAmountPerAttack => _palletCountPerShell;
	public sfloat BulletAngle => _bulletAngle;

	private readonly HitInfo _hitInfo;
	private readonly int _palletCountPerShell;
	private readonly sfloat _bulletAngle, _degreeOffset, _degreeDelta;

	private bool _nowPressed, _beforePressed;

	public NShellyBuckShot(NCharacterShelly character) : base(character)
	{
		MaxShellCount = 3;
		PowerChargeAmount = 50;
		CurrentShellCount = MaxShellCount;
		ReloadFrame = 120;
		_palletCountPerShell = 5;
		_bulletAngle = (sfloat)30f;
		_degreeOffset = 90 - _bulletAngle * 0.5f;
		_degreeDelta = _bulletAngle / _palletCountPerShell;
		_hitInfo = new HitInfo()
		{
			Damage = 20,
		};

		World.ProjectileSystem.Reserve(NetObjectType.Projectile_Shelly_Buckshot, MaxShellCount * _palletCountPerShell);
		Active = true;
	}

	public override void Update()
	{
		if (Active is false)
		{
			return;
		}

		base.Update();
	}

	public override void HandleInput(in InputData input)
	{
		_beforePressed = _nowPressed;
		_nowPressed = (input.ButtonInput & 1) == 1;

		IsAttack = _beforePressed is true && _nowPressed is false && CanAttack();
		if (IsAttack is false)
		{
			return;
		}

		OnPerform();
		World.ProjectileSystem.Awake(NetObjectType.Projectile_Shelly_Buckshot, _palletCountPerShell, (count, pallet) =>
		{
			pallet.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			pallet.SetAngle((_degreeOffset + _degreeDelta * count) * sMathf.Deg2Rad).SetOwner(Character.NetObj);
			pallet.Collider.OnCollisionEnter = (target) => OnHit(pallet, target);
			pallet.NetObj.Active = true;
		});
	}

	public override void Cancel()
	{
		throw new System.NotImplementedException();
	}

	public override void Reset()
	{
		Active = true;
		_beforePressed = _nowPressed = false;
		CurrentShellCount = MaxShellCount;
		CurrentReloadDeltaFrame = 0;
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
			Character.SpecialAttack.ChargePower(PowerChargeAmount);
			goto Return;
		}

		return;
	Return:
		pallet.Reset();
		World.ProjectileSystem.Return(pallet);
	}
}
