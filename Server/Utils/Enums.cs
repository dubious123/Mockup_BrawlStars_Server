﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils
{
	public static class Enums
	{
		public enum GameType
		{
			Team3vs3,
		}
		public enum GameState
		{
			Waiting,
			Started,
			Ended
		}
		public enum CharacterType
		{
			Dog,
		}
	}
}
