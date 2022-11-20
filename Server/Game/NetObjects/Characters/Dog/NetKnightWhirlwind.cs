using System;
using System.Collections.Generic;
using System.Data;

using static Enums;

namespace Server.Game
{
	public class NetKnightWhirlwind : INetBaseSkill
	{
		public NetCharacter Character => _dog;
		public int Id { get; init; }
		public int CoolTimeFrame { get; set; }
		public int CurrentCoolTimeFrame { get; set; }
		public int ReadyFrame { get; set; }
		public int SpinIntervalFrame { get; set; }
		public int CurrentSpinFrame { get; set; }
		public sfloat Range { get; set; }
		public sfloat SqrRange { get; init; }
		public sfloat MoveSpeed { get; set; }
		public bool Performing { get; set; }
		public bool Active { get; set; }

		private readonly NetCharacterKnight _dog;
		private readonly HitInfo _hitInfo;
		private IEnumerator<int> _coHandler;
		private bool _btnPressed;

		public NetKnightWhirlwind(NetCharacterKnight dog)
		{
			_dog = dog;
			Id = 0;
			Performing = false;
			Active = true;
			Range = (sfloat)1.5f;
			SqrRange = Range * Range;
			MoveSpeed = (sfloat)3f;
			CoolTimeFrame = 30;
			SpinIntervalFrame = 15;
			ReadyFrame = 3;
			_hitInfo = new HitInfo
			{
				Damage = 5,
			};
		}

		public virtual void Update()
		{
			if (Performing)
			{
				_coHandler.MoveNext();
			}

			if (CurrentCoolTimeFrame > 0)
			{
				CurrentCoolTimeFrame--;
			}
		}

		public void Cancel()
		{
			throw new NotImplementedException();
		}

		public virtual void HandleInput(in InputData input)
		{
			_btnPressed = (input.ButtonInput & 1) == 1;
			if ((Active is false) || Performing || (_btnPressed is false) || CurrentCoolTimeFrame <= 0)
			{
				return;
			}

			_dog.SetActiveOtherSkills(this, false);
			_dog.CanControlLook = false;
			_coHandler = Co_Perform();
			Performing = true;
		}

		public virtual IEnumerator<int> Co_Perform()
		{
			_dog.SetActiveOtherSkills(this, false);
			_dog.MoveSpeed = MoveSpeed;
			for (int i = 0; i < ReadyFrame && _btnPressed; i++)
			{
				yield return 0;
			}

			for (CurrentSpinFrame = SpinIntervalFrame; _btnPressed; CurrentSpinFrame++)
			{
				if (CurrentSpinFrame < SpinIntervalFrame)
				{
					yield return 0;
					continue;
				}

				Character.World.FindAllAndBroadcast(target =>
				{
					if (Character.World.GameRule.CanSendHit(Character, target) is true)
					{
						return (target.Position - Character.Position).sqrMagnitude <= SqrRange;
					}

					return false;
				}, target => Character.SendHit(target as ITakeHit, _hitInfo));

				yield return 0;
				CurrentSpinFrame = 0;
			}

			//todo	
			CurrentSpinFrame = 0;
			_dog.MoveSpeed = (sfloat)6f;
			_dog.SetActiveOtherSkills(this, true);
			_dog.CanControlLook = true;
			CurrentCoolTimeFrame = CoolTimeFrame;
			Performing = false;
			yield break;
		}
	}
}
