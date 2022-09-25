using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Base
{
	public class Component
	{
		public GameObject gameObject;
		public Component(GameObject go)
		{
			go.Update += Update;
		}
		protected virtual void Update()
		{

		}
	}
}
