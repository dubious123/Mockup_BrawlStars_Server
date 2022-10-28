namespace Server.Game.Characters.Dog
{
	public class Dog_Bash : BaseSkill
	{
		private IEnumerator<int> _coHandler;
		private bool _buttonPressed;
		private bool _performing;

		private sfloat _maxBashlength = (sfloat)10f;
		private int _maxChargeTime = 90;
		private int _holdingTimeLimit = 300;
		private sfloat _bashSpeed = (sfloat)24;
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
				sfloat bashLength;
				sfloat bashTime;
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
					bashLength = _maxBashlength * sMathf.Min((sfloat)(holdFrame / _maxChargeTime), sfloat.One);
					bashTime = bashLength / _bashSpeed;
					Console.WriteLine($"bashTime : {bashTime}, bashLength : {bashLength}");
					for (sfloat current = sfloat.Zero; current <= bashTime; current += Timing.DeltaTime)
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
