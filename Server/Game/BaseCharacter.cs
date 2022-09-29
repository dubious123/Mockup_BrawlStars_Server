using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Server.Utils;
using Server.Game.Base.Utils;
using Server.Log;
using static Server.Utils.Enums;

namespace Server.Game.Base
{
	public class BaseCharacter
	{
		public BaseCharacter(GameRoom game)
		{
			_interactable = true;
			_controllable = true;
			_game = game;
		}
		public Vector3 Position;
		public Quaternion Rotation;
		public Enums.CharacterType CharacterType { get; set; } = Enums.CharacterType.Dog;
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
		protected Quaternion _targetRotation;

		protected float _attackCooldown;

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

			#region Cooldown
			if (_attackCooldown > 0) _attackCooldown -= Timing.DeltaTime;
			#endregion
			LogMgr.Log($"{Position}", TraceSourceType.Debug);

		}
		public virtual void HandleInput(in PlayerInput input)
		{
			_targetMoveDir = _targetMoveDir.SmoothDamp(new Vector3(input.MoveDirX, 0, input.MoveDirY), ref _smoothVelocity, _smoothInputSpeed, float.MaxValue, Timing.DeltaTime);
			_targetLookDir.X = input.LookDirX;
			_targetLookDir.Y = input.LookDirY;
			if ((input.ButtonPressed & 1) == 1) PerformBasicAttack();

		}
		void PerformBasicAttack()
		{
			if (_attackCooldown > 0f) return;

			var others = _game.FindCharacters(other =>
			{
				if (other == this) return false;
				var targetDir = other.Position - Position;
				if (targetDir.LengthSquared() > 2.89f) return false;
				var angle = Tools.Angle(targetDir, Tools.Rotate(in Rotation, Vector3.UnitZ));
				if (angle < -45f || angle > 45f) return false;
				return true;
			});
			foreach (var other in others)
			{
				LogMgr.Log("Hit", TraceSourceType.Debug);
			}

			_attackCooldown = 0.25f;
		}
	}
}
