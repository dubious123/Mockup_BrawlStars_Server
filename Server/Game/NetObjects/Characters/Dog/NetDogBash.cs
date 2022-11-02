using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.Annotations;

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

		private NetCharacterDog _dog;
		private IEnumerator<int> _coHandler;
		private bool _holdBtnPressed;
		private bool _cancelBtnPressed;
		private int _holdFrame;

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
						//OnHoldCancel();
						break;
					}

					//OnHolding();
					yield return 0;
				}

				Holding = false;
				Character.MoveSpeed = (sfloat)6f;
				Character.CanControlMove = false;
				Character.CanControlLook = false;

				BashFrame = (int)((sfloat)_holdFrame / (sfloat)MaxHoldingFrame * MaxBashDistance / BashSpeed * 60f);
				for (int i = 0; i <= BashFrame; i++)
				{
					Character.Move(Character.TargetLookDir * BashSpeed * Define.FixedDeltaTime);
					yield return 0;
				}

				Character.CanControlMove = true;
				Character.CanControlLook = true;
				_dog.SetActiveOtherSkills(this, true);
				for (int i = 0; i < CoolTimeFrame; i++)
				{
					yield return 0;
				}

				Performing = false;
				yield return 0;
			}
		}
	}
}
