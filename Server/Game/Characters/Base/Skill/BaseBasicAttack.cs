using Tools = Server.Game.Base.Utils.Tools;

namespace Server.Game.Characters.Base.Skill
{
	public class BaseBasicAttack : BaseSkill
	{
		protected IEnumerator<int> _coHandler;
		protected bool _performing = false;
		protected readonly float _squaredRange = 2.89f;
		protected readonly float _angle = 45f;
		public BaseBasicAttack(BaseCharacter character, GameRoom game) : base(character, game)
		{
			_coHandler = Co_Perform();
			Id = 0;
			_hitInfo = new HitInfo
			{
				Damage = 10,
			};
		}
		public override void HandleOneFrame()
		{
			if (_performing) _coHandler.MoveNext();
		}
		public override void HandleInput(bool buttonPressed)
		{
			if (_enabled == false) return;
			if (_performing) return;
			if (buttonPressed == false) return;
			_character.SetOtherSkillsActive(Id, false);
			_character.EnableLookControll(false);
			_character.EnableMoveControll(false);

			_performing = true;
		}
		public override void Cancel()
		{
			_coHandler = null;
		}
		protected IEnumerator<int> Co_Perform()
		{
			while (true)
			{
				for (int i = 0; i < 30; i++)
				{
					yield return 0;
				}
				GetResult();
				for (int i = 0; i < 30; i++)
				{
					yield return 0;
				}
				_performing = false;
				_character.SetOtherSkillsActive(Id, true);
				_character.EnableLookControll(true);
				_character.EnableMoveControll(true);
				yield return 0;
			}
		}

		protected virtual void GetResult()
		{
			var others = _game.FindCharacters(other =>
			{
				if (other == _character) return false;
				var targetDir = other.Position - _character.Position;
				if (targetDir.LengthSquared() > _squaredRange) return false;
				var angle = Tools.Angle(targetDir, Tools.Rotate(in _character.Rotation, Vector3.UnitZ));
				if (angle < -_angle || angle > _angle) return false;
				return true;
			});
			foreach (var other in others)
			{
				other.OnGetHit(in _hitInfo);
			}
			_game.PushActionResult(Id, _character.TeamId, others.Select(other => other.TeamId).ToArray());
		}
	}
}
