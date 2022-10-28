namespace Server.Game.Base;

public class BaseCharacter
{
	public short TeamId { get; init; }
	public int Id { get; }
	public sVector3 Position { get; set; }
	public sVector3 LookDir => _targetLookDir;
	public sQuaternion Rotation { get; set; }
	public CharacterType CharacterType { get; set; } = CharacterType.Dog;

	private bool _controllable;
	private bool _interactable;
	private bool _moveControllEnabled = true;
	private bool _lookControllEnabled = true;
	private bool _isStun = false;
	private int _maxHp = 100;
	private int _currentHp = 100;
	private sfloat _currentMoveSpeed;
	private sfloat _walkSpeed = (sfloat)3f;
	private sfloat _runSpeed = (sfloat)6f;
	private sfloat _rotationSpeed = (sfloat)360f;
	private sfloat _smoothInputSpeed = (sfloat)0.01f;
	private sVector3 _smoothVelocity;
	private sVector3 _targetMoveDir;
	private sVector3 _targetLookDir;
	private GameRoom _game;
	private BaseSkill _basicAttack;
	private sQuaternion _targetRotation;
	private CoroutineHelper _coHelper;

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
			_currentMoveSpeed = _targetMoveDir == sVector3.zero ? sfloat.Zero : _runSpeed;
			var delta = _currentMoveSpeed * Timing.DeltaTime * _targetMoveDir;
			Position += delta;
		}

		#endregion
		#region Rotate
		if (_lookControllEnabled)
		{
			if (_targetLookDir != sVector3.zero)
			{
				_targetRotation = sQuaternion.LookRotation(Timing.DeltaTime * _targetLookDir, new sVector3(0, 1, 0));
			}

			Rotation = sQuaternion.RotateTowards(Rotation, _targetRotation, Timing.DeltaTime * _rotationSpeed);
		}

		#endregion

		#region Skills
		_basicAttack.HandleOneFrame();
		#endregion
	}

	public virtual void HandleInput(in PlayerInput input)
	{
		_targetMoveDir = sVector3.SmoothDamp(_targetMoveDir, new sVector3(input.MoveDirX, sfloat.Zero, input.MoveDirY), ref _smoothVelocity, _smoothInputSpeed, sfloat.PositiveInfinity, Timing.DeltaTime);
		_targetLookDir.x = input.LookDirX;
		_targetLookDir.y = input.LookDirY;
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
		//while (deltaTime < info.Duration)
		//{
		//	deltaTime += Timing.DeltaTime;
		//	yield return 0;
		//}
		yield return 0;
		_isStun = false;
	}

	protected virtual void OnDead()
	{
	}
}
