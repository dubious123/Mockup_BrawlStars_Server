using Tools = Server.Game.Base.Utils.Tools;

namespace Server.Game.Base
{
	public class BaseCharacter
	{
		public short TeamId { get; init; }
		public int Id { get; }
		public Vector3 Position { get; set; }
		public Vector3 LookDir => _targetLookDir;
		public Quaternion Rotation { get; set; }
		public CharacterType CharacterType { get; set; } = CharacterType.Dog;

		private bool _controllable;
		private bool _interactable;
		private bool _isCharging;
		private bool _isAttacking;
		private bool _moveControllEnabled = true;
		private bool _lookControllEnabled = true;
		private bool _isStun = false;
		private int _maxHp = 100;
		private int _currentHp = 100;
		private float _currentMoveSpeed;
		private float _walkSpeed = 3f;
		private float _runSpeed = 6f;
		private float _rotationSpeed = 360f;
		private float _smoothInputSpeed;
		private Vector3 _smoothVelocity;
		private Vector3 _targetMoveDir;
		private Vector3 _targetLookDir;
		private GameRoom _game;
		private BaseSkill _basicAttack;
		private Quaternion _targetRotation;
		private CoroutineHelper _coHelper;
		private HitInfo _basicAttackHitInfo;

		public BaseCharacter(GameRoom game, short teamId)
		{
			TeamId = teamId;
			_interactable = true;
			_controllable = true;
			_game = game;
			_coHelper = game.CoHelper;
			_basicAttack = new BaseBasicAttack(this, game);
		}

		public void EnableMoveControll(bool value) => _moveControllEnabled = value;

		public void EnableLookControll(bool value) => _lookControllEnabled = value;

		public virtual void HandleOneFrame()
		{
			#region Move
			if (_moveControllEnabled)
			{
				_currentMoveSpeed =
					_targetMoveDir == Vector3.Zero ? 0f :
					(_isAttacking || _isCharging) ? _walkSpeed :
					_runSpeed;
				Position += _currentMoveSpeed * Timing.DeltaTime * _targetMoveDir;
			}

			#endregion
			#region Rotate
			if (_lookControllEnabled)
			{
				if (_targetLookDir != Vector3.Zero) _targetRotation = Tools.LookRotation(Timing.DeltaTime * _targetLookDir, Vector3.UnitY);
				Rotation = Tools.RotateTowards(Rotation, _targetRotation, Timing.DeltaTime * _rotationSpeed);
			}

			#endregion

			#region Skills
			_basicAttack.HandleOneFrame();
			#endregion

			LogMgr.Log($"{Position}", TraceSourceType.Debug);
		}

		public virtual void HandleInput(in PlayerInput input)
		{
			_targetMoveDir = _targetMoveDir.SmoothDamp(new Vector3(input.MoveDirX, 0, input.MoveDirY), ref _smoothVelocity, _smoothInputSpeed, float.MaxValue, Timing.DeltaTime);
			_targetLookDir.X = input.LookDirX;
			_targetLookDir.Y = input.LookDirY;
			_basicAttack.HandleInput((input.ButtonPressed & 1) == 1);
		}

		public virtual void SetOtherSkillsActive(uint skillId, bool active)
		{
			if (_basicAttack.Id == skillId == false) _basicAttack.SetActive(active);
		}

		public virtual void OnGetHit(in HitInfo info)
		{
			_currentHp = (int)MathF.Max(0, _currentHp - info.Damage);
			if (_currentHp == 0)
			{
				OnDead();
				return;
			}

			if (info.KnockbackInfo is not null)
			{
				Co_OnKnockback(info.KnockbackInfo);
				return;
			}

			if (info.StunInfo is not null)
			{
				_coHelper.RunCoroutine(Co_OnStun(info.StunInfo));
				Co_OnStun(info.StunInfo);
				return;
			}
		}

		public virtual void Co_OnKnockback(in KnockbackInfo info)
		{
		}

		public virtual IEnumerator<int> Co_OnStun(StunInfo info)
		{
			float deltaTime = 0f;
			_isStun = true;
			while (deltaTime < info.Duration)
			{
				deltaTime += Timing.DeltaTime;
				yield return 0;
			}

			_isStun = false;
		}

		protected virtual void OnDead()
		{
		}
	}
}
