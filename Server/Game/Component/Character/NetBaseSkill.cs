using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public abstract class NetBaseSkill
	{
		public NetCharacter Character { get; set; }
		public bool Performing { get; set; }
		public bool Active { get; set; } = true;
		public abstract void HandleInput(in InputData input);
		public abstract void Update();
		public abstract void Cancel();
		protected abstract IEnumerator<int> Co_Perform();
	}
}
