using Server.Game.Base;
using Server.Game.Characters.Base.Skill;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Characters.Dog
{
	public class Dog_Bash : BaseSkill
	{
		float _maxBashlength;
		int _maxChargeTime;
		int _holdingTimeLimit;
		float _bashSpeed;
		int _coolTime;
		protected IEnumerator<int> _coHandler;

		protected bool _buttonPressed;
		protected bool _performing;
		public Dog_Bash(BaseCharacter character, GameRoom game) : base(character, game)
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
			if (_enabled == false) return;
			_buttonPressed = buttonPressed;
			if (_performing == false && _buttonPressed)
			{
				_character.SetOtherSkillsActive(Id, false);
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
				float bashLength = 0;
				float bashTime = 0;
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
					_character.EnableLookControll(false);
					_character.EnableMoveControll(false);

					bashLength = _maxBashlength * MathF.Min(holdFrame / (float)_maxChargeTime, 1);
					bashTime = bashLength / _bashSpeed;

					for (float current = 0f; current <= bashTime; current += Timing.DeltaTime)
					{
						_character.Position += _character.LookDir * _bashSpeed * Timing.DeltaTime;
						yield return 0;
					}
					_character.EnableLookControll(true);
					_character.EnableMoveControll(true);
				}
				#endregion

				_character.SetOtherSkillsActive(Id, true);

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
