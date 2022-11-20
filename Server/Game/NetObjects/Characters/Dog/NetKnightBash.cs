using System.Collections.Generic;

using static Enums;

namespace Server.Game
{
	public class NetKnightBash : INetBaseSkill
	{
		public NetCharacter Character => _dog;
		public int Id { get; init; }
		public int MaxHoldingFrame { get; set; }
		public int CurrentHoldFrame { get; protected set; }
		public int CoolTimeFrame { get; protected set; }
		public int CurrentCooltime { get; protected set; }
		public bool Performing { get; set; }
		public bool Active { get; set; }
		public bool Holding { get; protected set; }
		public bool Bashing { get; protected set; }
		public sfloat MaxBashDistance { get; set; }
		public sfloat BashSpeed { get; set; }
		public int BashFrame { get; private set; }
		public bool IsHit { get; private set; }

		private NetCharacterKnight _dog;
		private IEnumerator<int> _coHandler;
		private bool _holdBtnPressed;
		private bool _cancelBtnPressed;
		private HitInfo _hitInfo;

		public NetKnightBash(NetCharacterKnight dog)
		{
			_dog = dog;
			Id = 0;
			MaxHoldingFrame = 60;
			MaxBashDistance = (sfloat)8f;
			BashSpeed = (sfloat)15f;
			CoolTimeFrame = 300;
			Performing = false;
			Bashing = false;
			Active = true;
			_coHandler = Co_Perform();
			_hitInfo = new HitInfo
			{
				Damage = 30,
				KnockbackDistance = (sfloat)0.5f,
				KnockbackDuration = 10,
				StunDuration = 300
			};
		}

		public void Update()
		{
			if (Performing)
			{
				_coHandler.MoveNext();
			}

			if (CurrentCooltime > 0)
			{
				--CurrentCooltime;
			}
		}

		public void HandleInput(in InputData input)
		{
			_holdBtnPressed = (input.ButtonInput & 2) != 0;
			_cancelBtnPressed = (input.ButtonInput & 4) != 0;
			if (Active == false || Performing || !_holdBtnPressed || CurrentCooltime > 0)
			{
				return;
			}

			_dog.SetActiveOtherSkills(this, false);
			Performing = true;
		}

		public void Cancel()
		{
			if (Performing)
			{
				Performing = false;
				Bashing = false;
				Holding = false;
				_coHandler = Co_Perform();
			}
		}

		public IEnumerator<int> Co_Perform()
		{
			while (true)
			{
				Character.MoveSpeed = (sfloat)3f;
				Holding = true;
				for (CurrentHoldFrame = 0; CurrentHoldFrame < MaxHoldingFrame && _holdBtnPressed; CurrentHoldFrame++)
				{
					if (_cancelBtnPressed is true)
					{
						goto OnBashEnd;
					}

					yield return 0;
				}

				Holding = false;
				Character.CanControlMove = false;
				Character.CanControlLook = false;
				BashFrame = (int)((sfloat)CurrentHoldFrame / (sfloat)MaxHoldingFrame * MaxBashDistance / BashSpeed * 60f);
				Bashing = true;
				var dir = Character.TargetLookDir;
				for (int i = 0; i < BashFrame; i++)
				{
					Character.Move(dir * BashSpeed * Define.FixedDeltaTime);
					IsHit = Character.World.FindAllAndBroadcast(netObj =>
					{
						if (Character.World.GameRule.CanSendHit(Character, netObj))
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

			OnBashEnd:
				Bashing = false;
				CurrentCooltime = CoolTimeFrame;
				Character.CanControlMove = true;
				Character.CanControlLook = true;
				Character.MoveSpeed = (sfloat)6f;
				Holding = false;
				_dog.SetActiveOtherSkills(this, true);
				Performing = false;
				yield return 0;
			}
		}
	}
}
