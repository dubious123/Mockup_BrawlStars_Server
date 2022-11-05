using static Enums;

namespace Server.Game
{
	public class NetDogBasicAttack : INetBaseSkill
	{
		public NetCharacter Character => _dog;
		public int Id { get; init; }
		public sfloat Length { get; set; }
		public sfloat Angle { get; set; }
		public sfloat SqrLength { get; set; }
		public bool Performing { get; set; }
		public bool Active { get; set; }

		private NetCharacterDog _dog;
		private IEnumerator<int> _coHandler;
		private HitInfo _hitInfo;

		public NetDogBasicAttack(NetCharacterDog dog)
		{
			_dog = dog;
			Id = 0;
			Performing = false;
			Active = true;
			_coHandler = Co_Perform();
			Length = (sfloat)1.5f;
			SqrLength = Length * Length;
			Angle = (sfloat)45f;
			_hitInfo = new HitInfo
			{
				Damage = 20,
			};
		}

		public virtual void Update()
		{
			if (Performing)
			{
				_coHandler.MoveNext();
			}
		}

		public void Cancel()
		{
			if (Performing)
			{
				_coHandler.Reset();
				Performing = false;
			}
		}

		public virtual void HandleInput(in InputData input)
		{
			if (Active == false || Performing || (input.ButtonInput & 1) == 0)
			{
				return;
			}

			_dog.SetActiveOtherSkills(this, false);
			Performing = true;
			_dog.CanControlMove = false;
			_dog.CanControlLook = false;
		}

		public virtual IEnumerator<int> Co_Perform()
		{
			while (true)
			{
				_dog.SetActiveOtherSkills(this, false);
				for (int i = 0; i < 30; i++)
				{
					yield return 0;
				}

				Character.World.FindAllAndBroadcast(target =>
				{
					if (target.Tag is not NetObjectTag.Character || target == Character || target is not ITakeHit || (target as ITakeHit).CanBeHit() is false)
					{
						return false;
					}

					var dir = target.Position - Character.Position;
					if (dir.sqrMagnitude > SqrLength)
					{
						return false;
					}

					var angle = sVector3.Angle(Character.Forward, dir);
					if (angle > Angle)
					{
						return false;
					}

					return true;
				}, target => Character.SendHit(target as ITakeHit, _hitInfo));

				for (int i = 0; i < 30; i++)
				{
					yield return 0;
				}

				Performing = false;
				_dog.SetActiveOtherSkills(this, true);
				_dog.CanControlMove = true;
				_dog.CanControlLook = true;
				yield return 0;
			}
		}
	}
}
