using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Skills
{
	public interface INetHoldable
	{
		public void OnHoldStart();
		public void OnHolding();
		public void OnHoldEnd();
		public void OnHoldCancel();
	}
}
