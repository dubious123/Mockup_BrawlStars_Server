using Server.Logs;

using static Enums;

namespace Server.Game
{
	public class NetDogBash : INetBaseSkill
	{
		public NetCharacter Character => _dog;
		public int Id { get; init; }
		public int MaxHoldingFrame { get; set; }
		public int CoolTimeFrame { get; set; }
		public bool Performing { get; set; }
		public bool Active { get; set; }
		public bool Holding { get; set; }
		public sfloat MaxBashDistance { get; set; }
		public sfloat BashSpeed { get; set; }
		public int BashFrame { get; private set; }
		public bool IsHit { get; private set; }

		private NetCharacterDog _dog;
		private IEnumerator<int> _coHandler;
		private bool _holdBtnPressed;
		private bool _cancelBtnPressed;
		private int _holdFrame;
		private HitInfo _hitInfo;

		public NetDogBash(NetCharacterDog dog)
		{
			_dog = dog;
			Id = 0;
			MaxHoldingFrame = 180;
			MaxBashDistance = (sfloat)8f;
			BashSpeed = (sfloat)15f;
			CoolTimeFrame = 300;
			Performing = false;
			Active = true;
			_coHandler = Co_Perform();
			_hitInfo = new HitInfo
			{
				Damage = 50,
			};
		}

		public void Update()
		{
			if (Performing)
			{
				_coHandler.MoveNext();
			}
		}

		public void HandleInput(in InputData input)
		{
			_holdBtnPressed = (input.ButtonInput & 2) != 0;
			_cancelBtnPressed = (input.ButtonInput & 4) != 0;
			if (Active == false || Performing || !_holdBtnPressed)
			{
				return;
			}

			_dog.SetActiveOtherSkills(this, false);
			Performing = true;
		}

		public IEnumerator<int> Co_Perform()
		{
			while (true)
			{
				Character.MoveSpeed = (sfloat)3f;
				Holding = true;
				for (_holdFrame = 0; _holdFrame < MaxHoldingFrame && _holdBtnPressed; _holdFrame++)
				{
					if (_cancelBtnPressed is true)
					{
						OnBashEnd();
						goto CoolTime;
					}

					yield return 0;
				}

				Holding = false;
				Character.CanControlMove = false;
				Character.CanControlLook = false;
				BashFrame = (int)((sfloat)_holdFrame / (sfloat)MaxHoldingFrame * MaxBashDistance / BashSpeed * 60f);
				for (int i = 0; i < BashFrame; i++)
				{
					Character.Move(Character.TargetLookDir * BashSpeed * Define.FixedDeltaTime);
					IsHit = Character.World.FindAllAndBroadcast(netObj =>
					{
						if (netObj is INetCollidable2D && netObj is ITakeHit && netObj != Character)
						{
							return (netObj as INetCollidable2D).Collider.CheckCollision(Character.Collider);
						}

						return false;
					},
					netObj => Character.SendHit(netObj as ITakeHit, _hitInfo));

					if (IsHit)
					{
						break;
					}

					yield return 0;
				}

				OnBashEnd();

			CoolTime:
				for (int i = 0; i < CoolTimeFrame; i++)
				{
					yield return 0;
				}

				Performing = false;
				yield return 0;
			}
		}

		public void OnBashEnd()
		{
			Character.CanControlMove = true;
			Character.CanControlLook = true;
			Character.MoveSpeed = (sfloat)6f;
			Holding = false;
			_dog.SetActiveOtherSkills(this, true);
		}
	}
}
