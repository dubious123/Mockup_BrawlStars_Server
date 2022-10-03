namespace Server.Game.Characters.Dog
{
	public class Dog_Bash : BaseSkill
	{
		private IEnumerator<int> _coHandler;
		private bool _buttonPressed;
		private bool _performing;

		private float _maxBashlength = 10f;
		private int _maxChargeTime = 90;
		private int _holdingTimeLimit = 300;
		private float _bashSpeed = 24;
		private int _coolTime = 300;

		public Dog_Bash(BaseCharacter character, GameRoom game)
			: base(character, game)
		{
			_coHandler = Co_Perform();
			_performing = false;
			Id = 2;
		}

		public override void HandleOneFrame()
		{
			if (_performing) _coHandler.MoveNext();
		}

		public override void HandleInput(bool buttonPressed)
		{
			if (Enabled == false) return;
			_buttonPressed = buttonPressed;
			if (_performing == false && _buttonPressed)
			{
				Character.SetOtherSkillsActive(Id, false);
				_performing = true;
				return;
			}
		}

		public override void Cancel()
		{
		}

		protected IEnumerator<int> Co_Perform()
		{
			while (true)
			{
				float bashLength;
				float bashTime;
				int holdFrame = 0;
				#region Charge
				{
					for (; holdFrame < _holdingTimeLimit; holdFrame++)
					{
						yield return 0;
						if (_buttonPressed == false) break;
					}
				}
				#endregion

				#region Bash
				{
					Character.EnableLookControll(false);
					Character.EnableMoveControll(false);
					bashLength = _maxBashlength * MathF.Min(holdFrame / (float)_maxChargeTime, 1);
					bashTime = bashLength / _bashSpeed;

					for (float current = 0f; current <= bashTime; current += Timing.DeltaTime)
					{
						Character.Position += Character.LookDir * _bashSpeed * Timing.DeltaTime;
						yield return 0;
					}

					Character.EnableLookControll(true);
					Character.EnableMoveControll(true);
				}
				#endregion

				Character.SetOtherSkillsActive(Id, true);

				#region Wait For CoolTime
				{
					for (int i = _coolTime; i > 0; i--)
					{
						yield return 0;
					}
				}
				#endregion

				_performing = false;
				yield return 0;
			}
		}
	}
}
