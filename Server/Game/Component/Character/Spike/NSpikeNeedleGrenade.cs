namespace Server.Game
{
	public class NSpikeNeedleGrenade : NetBasicAttack
	{
		private readonly HitInfo _granadeHitInfo, _spikeHitInfo;
		private bool _nowPressed, _beforePressed;

		public NSpikeNeedleGrenade(NetCharacter character) : base(character)
		{
			MaxShellCount = 4;
			CurrentShellCount = MaxShellCount;
			ReloadFrame = 120;
			_granadeHitInfo = new HitInfo()
			{
				Damage = 500,
				PowerChargeAmount = 15,
			};

			_spikeHitInfo = new HitInfo()
			{
				Damage = 560,
				PowerChargeAmount = 10,
			};

			World.ProjectileSystem.Reserve(NetObjectType.Projectile_Spike_NeedleGranade, MaxShellCount);
			World.ProjectileSystem.Reserve(NetObjectType.Projectile_Spike_NeedleGranade_Needle, MaxShellCount * 6);
			Active = true;
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
			World.ProjectileSystem.Awake(NetObjectType.Projectile_Spike_NeedleGranade, InitGranade);
		}

		public override void Update()
		{
			if (Active)
			{
				base.Update();
			}
		}

		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		public override void Reset()
		{
			Active = true;
			_beforePressed = _nowPressed = false;
			CurrentShellCount = MaxShellCount;
			CurrentReloadDeltaFrame = 0;
		}

		private void InitGranade(NetProjectile projectile)
		{
			projectile.NetObj.SetPositionAndRotation(Character.Position, Character.Rotation);
			projectile.SetAngle((sfloat)90 * sMathf.Deg2Rad).SetOwner(Character.NetObj);
			projectile.Collider.OnCollisionEnter = target => OnHit(projectile, target);
			projectile.OnReachedMaxRadius = Explode;
			projectile.NetObj.Active = true;
		}

		private void OnHit(NetProjectile projectile, NetCollider2D target)
		{
			var wall = target.GetComponent<NetWall>();
			if (wall is not null)
			{
				goto Return;
			}

			var character = target.GetComponent<NetCharacter>();
			if (character is not null && World.GameRule.CanSendHit(Character, character))
			{
				Character.SendHit(character, _granadeHitInfo);
				goto Return;
			}

			return;
		Return:
			Explode(projectile);
			projectile.Reset();
			World.ProjectileSystem.Return(projectile);
		}

		private void OnNeedleHit(NetProjectile projectile, NetCollider2D target)
		{
			var wall = target.GetComponent<NetWall>();
			if (wall is not null)
			{
				goto Return;
			}

			var character = target.GetComponent<NetCharacter>();
			if (character is not null && World.GameRule.CanSendHit(Character, character))
			{
				Character.SendHit(character, _granadeHitInfo);
				goto Return;
			}

			return;
		Return:
			projectile.Reset();
			World.ProjectileSystem.Return(projectile);
		}

		private void Explode(NetProjectile from)
		{
			World.ProjectileSystem.Awake(NetObjectType.Projectile_Spike_NeedleGranade_Needle, 6, (count, projectile) =>
			{
				projectile.NetObj.SetPositionAndRotation(from.Position, from.Rotation);
				projectile.SetAngle((60 * count) * sMathf.Deg2Rad).SetOwner(Character.NetObj);
				projectile.Collider.OnCollisionEnter = target => OnNeedleHit(projectile, target);
				projectile.NetObj.Active = true;
			});
		}
	}
}
