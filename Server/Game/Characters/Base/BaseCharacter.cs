using Tools = Server.Game.Base.Utils.Tools;

namespace Server.Game.Base
{
	public class BaseCharacter
	{
		public short TeamId { get; init; }
		public int Id { get; }
		public Vector3 Position;
		public Quaternion Rotation;
		public CharacterType CharacterType { get; set; } = CharacterType.Dog;
		protected GameRoom _game;
		protected bool _controllable;
		protected bool _interactable;
		protected bool _isCharging;
		protected bool _isAttacking;
		protected float _currentMoveSpeed;
		protected float _walkSpeed = 3f;
		protected float _runSpeed = 6f;
		protected float _rotationSpeed = 360f;
		protected Vector3 _smoothVelocity;
		protected float _smoothInputSpeed;
		protected Vector3 _targetMoveDir;
		protected Vector3 _targetLookDir;
		public Vector3 LookDir => _targetLookDir;
		protected Quaternion _targetRotation;


		#region Coroutine
		CoroutineHelper _coHelper;
		#endregion

		#region Stat
		protected int _maxHp = 100;
		protected int _currentHp = 100;
		#endregion

		#region Skills
		private BaseSkill _basicAttack;
		#endregion

		#region  Hit Info
		protected HitInfo _basicAttackHitInfo;
		#endregion

		#region State
		public void EnableMoveControll(bool value) => _moveControllEnabled = value;
		protected bool _moveControllEnabled = true;
		public void EnableLookControll(bool value) => _lookControllEnabled = value;
		protected bool _lookControllEnabled = true;
		protected bool _isStun = false;
		#endregion


		public BaseCharacter(GameRoom game, short teamId)
		{
			TeamId = teamId;
			_interactable = true;
			_controllable = true;
			_game = game;
			_coHelper = game.CoHelper;
			_basicAttack = new BaseBasicAttack(this, game);
		}

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
