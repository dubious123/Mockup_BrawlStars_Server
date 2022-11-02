namespace Server.Game
{
	public class NetDogBasicAttack : INetBaseSkill
	{
		public NetCharacter Character => _dog;
		public int Id { get; init; }
		public bool Performing { get; set; }
		public bool Active { get; set; }

		private NetCharacterDog _dog;
		private IEnumerator<int> _coHandler;

		public NetDogBasicAttack(NetCharacterDog dog)
		{
			_dog = dog;
			Id = 0;
			Performing = false;
			Active = true;
			_coHandler = Co_Perform();
		}

		public virtual void Update()
		{
			if (Performing)
			{
				_coHandler.MoveNext();
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
