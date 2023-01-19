using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public class NetTree : NetEnv
	{
		private NetCollider2D _collider;

		public override void Start()
		{
			_collider = this.GetComponent<NetCollider2D>();
			_collider.OnCollisionEnter = OnCollisionEnter;
			_collider.OnCollisionExit = OnCollisionExit;
		}

		private void OnCollisionEnter(NetCollider2D other)
		{
			var character = other.GetComponent<NetCharacter>();
			if (character is null)
			{
				return;
			}

			World.EnvSystem.OnCharacterEnterTree(this, character);
		}

		private void OnCollisionExit(NetCollider2D other)
		{
			var character = other.GetComponent<NetCharacter>();
			if (character is null)
			{
				return;
			}

			World.EnvSystem.OnCharacterExitTree(this, character);
		}
	}
}
