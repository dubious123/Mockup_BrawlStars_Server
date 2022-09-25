using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Server.Utils;
using Server.Game.Base.Utils;

namespace Server.Game.Base
{
	public class BaseCharacter
	{
		public BaseCharacter()
		{
			_interactable = true;
			_controllable = true;

		}
		public Vector3 Position;
		public Quaternion Rotation;
		public Enums.CharacterType CharacterType { get; set; } = Enums.CharacterType.Dog;
		protected bool _controllable;
		protected bool _interactable;
		protected bool _isCharging;
		protected bool _isAttacking;
		protected float _currentMoveSpeed;
		protected float _walkSpeed;
		protected float _runSpeed;
		protected float _rotationSpeed;
		protected Vector3 _smoothVelocity;
		protected float _smoothInputSpeed;
		protected Vector3 _targetMoveDir;
		protected Vector3 _targetLookDir;
		protected Quaternion _targetRotation;

		public void Update()
		{
			if (_controllable == false || _interactable == false) return;
			#region Move
			_currentMoveSpeed =
				_targetMoveDir == Vector3.Zero ? 0f :
				(_isAttacking || _isCharging) ? _walkSpeed :
				_runSpeed;
			Position += _currentMoveSpeed * Timing.DeltaTime * _targetMoveDir;
			#endregion
			#region Rotate
			if (_targetLookDir != Vector3.Zero) _targetRotation = Tools.LookRotation(Timing.DeltaTime * _targetLookDir, Vector3.UnitY);
			Rotation = Tools.RotateTowards(Rotation, _targetRotation, Timing.DeltaTime * _rotationSpeed);
			#endregion
		}
		public void Move(Vector3 moveDir)
		{
			_targetMoveDir = _targetMoveDir.SmoothDamp(moveDir, ref _smoothVelocity, _smoothInputSpeed, float.MaxValue, Timing.DeltaTime);
		}
		public void Look(Vector3 lookDir)
		{
			_targetLookDir = lookDir;
		}
	}
}
