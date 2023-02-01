public class NShellyBuckShot : NetBasicAttack
{
	public int BulletAmountPerAttack => _palletCountPerShell;
	public sfloat BulletAngle => _bulletAngle;

	private readonly HitInfo _hitInfo;
	private readonly int _palletCountPerShell;
	private readonly sfloat _bulletAngle;
	private readonly sfloat _degreeOffset;
	private readonly sfloat _degreeDelta;

	private int _coHandler;
	private bool _nowPressed, _beforePressed;

	public NShellyBuckShot(NCharacterShelly character)
	{
		Character = character;
		MaxShellCount = 3;
		CurrentShellCount = MaxShellCount;
		WaitFrameBeforePerform = 10;
		WaitFrameAfterPerform = 10;
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
		if (_beforePressed is true && _nowPressed is false && CanAttack())
		{
			Character.SetActiveOtherSkills(this, false);
			_coHandler = World.NetTiming.RunCoroutine(Co_Perform());
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
		World.ProjectileSystem.Awake(NetObjectType.Projectile_Shelly_Buckshot, _palletCountPerShell, (count, pallet) =>
		{
			pallet.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			pallet.SetAngle((_degreeOffset + _degreeDelta * count) * sMathf.Deg2Rad).SetOwner(Character.NetObj);
			pallet.Collider.OnCollisionEnter = (target) => OnHit(pallet, target);
			pallet.NetObj.Active = true;
		});

		yield return 0;

		IsAttack = false;
		for (int i = 0; i < WaitFrameAfterPerform; i++)
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

	public override void Reset()
	{
		Active = true;
		_beforePressed = _nowPressed = false;
		CurrentShellCount = MaxShellCount;
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
