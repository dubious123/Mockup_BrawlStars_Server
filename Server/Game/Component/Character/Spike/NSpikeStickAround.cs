namespace Server.Game
{
	public class NSpikeStickAround : NetSpecialAttack
	{
#if CLIENT
		public sfloat MaxRadius => _maxRadius;
#endif
		private readonly HitInfo _hitInfo;

		private int _coHandle, _coAoeHandle, _currentDelay, _attackCount, _attackInterval;
		private bool _nowPressed, _beforePressed;
		private sfloat _maxRadius;
		private sVector3 _targetDir;

		public NSpikeStickAround(NetCharacter character) : base(character)
		{
			WaitFrameBeforePerform = 2;
			WaitFrameAfterPerform = 2;
			DelayFrameBetweenAttack = 5;
			(_attackCount, _attackInterval) = (5, 30);
			PowerUsagePerAttack = 100;
			MaxPowerAmount = 100;
			_maxRadius = (sfloat)7f;
			_hitInfo = new HitInfo()
			{
				Damage = 400,
				PowerChargeAmount = 13,
			};

			World.ProjectileSystem.Reserve(NetObjectType.Projectile_Spike_StickAround, 1);
			World.ProjectileSystem.Reserve(NetObjectType.Projectile_Spike_StickAround_Aoe, 1);
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

			_targetDir = Character.TargetLookDir.normalized * sMathf.Min(_maxRadius, Character.TargetLookDir.magnitude);
			_coHandle = World.NetTiming.RunCoroutine(Co_Perform());
		}

		public override bool CanAttack()
		{
			return base.CanAttack() && _currentDelay >= DelayFrameBetweenAttack;
		}

		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		public override void Reset()
		{
			Active = true;
			_beforePressed = _nowPressed = false;
			_currentDelay = 0;
			CurrentPowerAmount = 0;
		}

		private IEnumerator<int> Co_Perform()
		{
			OnPerform();
			for (int i = 0; i < WaitFrameBeforePerform; i++)
			{
				yield return 0;
			}

			World.ProjectileSystem.Awake(NetObjectType.Projectile_Spike_StickAround, InitGranade);

			for (int i = 0; i < WaitFrameAfterPerform; i++)
			{
				yield return 0;
			}

			Character.CanControlMove = true;
			Character.CanControlLook = true;
			Character.SetActiveOtherSkills(this, true);
			yield break;
		}

		private void InitGranade(NetProjectile projectile)
		{
			projectile.SetAngle((sfloat)90 * sMathf.Deg2Rad).SetOwner(Character.NetObj).SetMaxDistance(_targetDir.magnitude);
			projectile.NetObj.SetPositionAndRotation(Character.Position, sQuaternion.LookRotation(_targetDir));
			projectile.OnReachedMaxRadius = Explode;
			projectile.NetObj.Active = true;
		}

		private void Explode(NetProjectile from)
		{
			World.ProjectileSystem.Awake(NetObjectType.Projectile_Spike_StickAround_Aoe, (aoe) => InitAoe(aoe, from.Position));
		}

		private void InitAoe(NetProjectile aoe, sVector3 position)
		{
			aoe.NetObj.Position = position;
			aoe.SetOwner(Character.NetObj);
			aoe.Collider.OnCollisionEnter = OnHit;
			aoe.Active = true;
			aoe.Collider.Active = false;
			_coAoeHandle = World.NetTiming.RunCoroutine(Co_Aoe(aoe));
		}

		private IEnumerator<int> Co_Aoe(NetProjectile from)
		{
			for (int i = 0; i < _attackCount; ++i)
			{
				from.Collider.Active = true;
				yield return 0;
				from.Collider.Active = false;
				yield return _attackInterval - 1;
			}

			from.Reset();
			World.ProjectileSystem.Return(from);
			yield break;
		}

		private void OnHit(NetCollider2D target)
		{
			var character = target.GetComponent<NetCharacter>();
			if (character is not null && World.GameRule.CanSendHit(Character, character))
			{
				Character.SendHit(character, _hitInfo);
			}
		}
	}
}
