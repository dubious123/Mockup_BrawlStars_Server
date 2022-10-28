
namespace Server.Game.Characters.Base.Skill
{
	public class BaseBasicAttack : BaseSkill
	{
		private readonly sfloat _squaredRange = (sfloat)2.89f;
		private readonly sfloat _angle = (sfloat)45f;
		private bool _performing = false;
		private IEnumerator<int> _coHandler;

		public BaseBasicAttack(BaseCharacter character, GameRoom game)
			: base(character, game)
		{
			_coHandler = Co_Perform();
			Id = 0;
			SkillHitInfo = new HitInfo
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
			if (Enabled == false) return;
			if (_performing) return;
			if (buttonPressed == false) return;
			Character.SetOtherSkillsActive(Id, false);
			Character.EnableLookControll(false);
			Character.EnableMoveControll(false);

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
				Character.SetOtherSkillsActive(Id, true);
				Character.EnableLookControll(true);
				Character.EnableMoveControll(true);
				yield return 0;
			}
		}

		protected virtual void GetResult()
		{
			var others = Game.FindCharacters(other =>
			{
				if (other == Character) return false;
				var targetDir = other.Position - Character.Position;
				if (targetDir.sqrMagnitude > _squaredRange) return false;
				var angle = sVector3.Angle(targetDir, sQuaternion.Rotate(Character.Rotation, sVector3.forward));
				if (angle < -_angle || angle > _angle) return false;
				return true;
			});

			foreach (var other in others)
			{
				other.OnGetHit(SkillHitInfo);
			}

			Game.PushActionResult(Id, Character.TeamId, others.Select(other => other.TeamId).ToArray());
		}
	}
}
