namespace Server.Game
{
	public abstract class NetCharacter : NetBaseComponent, INetUpdatable, ITakeHit, ISendHit
	{
		public sfloat MoveSpeed { get; set; }
		public sfloat LookSpeed { get; set; }
		public sfloat MoveSmoothTime { get; set; }
		public sVector2 KnockbackDelta { get; protected set; }
		public sVector3 TargetMoveDir { get; set; }
		public sVector3 TargetLookDir { get; set; }
		public sVector3 Forward => NetObj.Rotation * sVector3.forward;
		public Action OnCharacterDead { private get; set; }
		public CCFlags CCFlag { get; protected set; }
		public TeamType Team { get; set; }
		public NetCollider2D Collider { get; protected set; }
		public NetBasicAttack BasicAttack { get; protected set; }
		public NetSpecialAttack SpecialAttack { get; protected set; }
		public int KnockbackDuration { get; protected set; }
		public int StunDuration { get; protected set; }
		public int MaxHp { get; protected set; }
		public int Hp { get; protected set; }
		public bool CanControlMove { get; set; }
		public bool CanControlLook { get; set; }
		public bool IsVisible { get; set; }

		protected IEnumerator<int> KnockbackCoHandler { get; set; }
		protected IEnumerator<int> StunCoHandler { get; set; }

		private sVector3 _smoothVelocity;

		public override void Start()
		{
			Collider = this.GetComponent<NetCollider2D>();
			Team = World.GameRule.GetTeamType(NetObj);
			KnockbackCoHandler = CoKnockback();
			StunCoHandler = CoStun();
			OnCharacterDead += () => World.GameRule.OnCharacterDead(this);
			Reset();
		}

		public virtual void Reset()
		{
			BasicAttack?.Reset();
			SpecialAttack?.Reset();
			LookSpeed = (sfloat)360f;
			MoveSmoothTime = (sfloat)0.01f;
			CanControlMove = true;
			CanControlLook = true;
			IsVisible = true;
			Active = true;
		}

		public virtual void Update()
		{
			HandleCC();

			if (CanControlMove && TargetMoveDir != sVector3.zero)
			{
				Move(MoveSpeed * Define.FixedDeltaTime * TargetMoveDir);
			}

			if (CanControlLook && TargetLookDir != sVector3.zero)
			{
				var targetRotation = sQuaternion.LookRotation(TargetLookDir, sVector3.up);
				NetObj.Rotation = sQuaternion.RotateTowards(NetObj.Rotation, targetRotation, Define.FixedDeltaTime * LookSpeed);
			}

			BasicAttack.Update();
			SpecialAttack.Update();
		}

		public virtual void Move(sVector3 deltaPos)
		{
			var beforePos = NetObj.Position;
			NetObj.Position += deltaPos;
			if (NetObj.World.ColliderSystem.DetectCollision(Collider, NetObjectType.Env_Wall))
			{
				NetObj.Position = beforePos;
			}
		}

		public virtual void UpdateInput(in InputData input)
		{
			TargetMoveDir = sVector3.SmoothDamp(TargetMoveDir, input.MoveInput, ref _smoothVelocity, MoveSmoothTime, sfloat.PositiveInfinity, Define.FixedDeltaTime);
			TargetLookDir = input.LookInput;
			BasicAttack.HandleInput(in input);
			SpecialAttack.HandleInput(in input);
		}

		public virtual void SendHit(ITakeHit target, in HitInfo info)
		{
			if (info.Damage > 0)
			{
				target.TakeMeleeDamage(info.Damage);
			}

			if (info.KnockbackDuration > 0)
			{
				var delta = Forward.normalized * (info.KnockbackDistance / (sfloat)info.KnockbackDuration);
				target.TakeKnockback(info.KnockbackDuration, delta);
			}

			if (info.StunDuration > 0)
			{
				target.TakeStun(info.StunDuration);
			}

			SpecialAttack.ChargePower(info.PowerChargeAmount);
		}

		public virtual bool CanBeHit()
		{
			return IsDead() == false;
		}

		public virtual void TakeMeleeDamage(int damage)
		{
			Hp -= damage;

			if (IsDead())
			{
				OnDead();
			}
		}

		public virtual void HandleCC()
		{
			if (CCFlag == CCFlags.None)
			{
				return;
			}

			KnockbackCoHandler.MoveNext();
			StunCoHandler.MoveNext();
			if (CCFlag == CCFlags.None)
			{
				OnCCEnd();
			}
		}

		public virtual void TakeKnockback(int duration, sVector3 delta)
		{
			KnockbackDuration = duration;
			KnockbackDelta = delta;
			KnockbackCoHandler = CoKnockback();
		}

		public virtual void TakeStun(int duration)
		{
			StunDuration = duration;
			StunCoHandler = CoStun();
		}

		public virtual bool IsDead()
		{
			return Hp <= 0;
		}

		public virtual void OnDead()
		{
			CanControlMove = false;
			CanControlLook = false;
			BasicAttack.Active = false;
			SpecialAttack.Active = false;
			OnCharacterDead?.Invoke();
		}

		public virtual void SetActiveOtherSkills(NetBaseSkill from, bool Active)
		{
			if (from != BasicAttack)
			{
				BasicAttack.Active = Active;
			}

			if (from != SpecialAttack)
			{
				SpecialAttack.Active = Active;
			}
		}

		protected virtual IEnumerator<int> CoKnockback()
		{
			OnCCStart();
			for (; KnockbackDuration > 0; KnockbackDuration--)
			{
				Move(KnockbackDelta);
				yield return 0;
			}

			CCFlag ^= CCFlags.Knockback;
			yield break;
		}

		protected virtual IEnumerator<int> CoStun()
		{
			OnCCStart();
			for (; StunDuration > 0; StunDuration--)
			{
				yield return 0;
			}

			CCFlag ^= CCFlags.Stun;
			yield break;
		}

		protected virtual void OnCCStart()
		{
			CanControlMove = false;
			CanControlLook = false;
			BasicAttack.Cancel();
			BasicAttack.Active = false;
			SpecialAttack.Cancel();
			SpecialAttack.Active = false;
		}

		protected virtual void OnCCEnd()
		{
			CanControlMove = true;
			CanControlLook = true;
			BasicAttack.Active = true;
			SpecialAttack.Active = true;
		}
	}
}
