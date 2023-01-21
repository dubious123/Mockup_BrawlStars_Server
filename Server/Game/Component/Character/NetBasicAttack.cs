using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
	public abstract class NetBasicAttack : NetBaseSkill
	{
		public int CurrentShellCount { get; protected set; }
		public int MaxShellCount { get; protected set; }
		public int CurrentReloadDeltaFrame { get; protected set; }
		public int ReloadFrame { get; protected set; }
		public int WaitFrameBeforePerform { get; protected set; }
		public int WaitFrameAfterPerform { get; protected set; }
		public bool IsAttack { get; protected set; }

		public override void Update()
		{
			if (CurrentShellCount >= MaxShellCount)
			{
				return;
			}

			if (CurrentReloadDeltaFrame < ReloadFrame)
			{
				++CurrentReloadDeltaFrame;
			}
			else
			{
				++CurrentShellCount;
				CurrentReloadDeltaFrame = 0;
			}
		}

		public virtual void OnPerform()
		{
			--CurrentShellCount;
		}
	}
}
