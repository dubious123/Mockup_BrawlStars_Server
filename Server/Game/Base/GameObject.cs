using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Server.Game.Base
{
	public class GameObject
	{
		#region Transform
		public Vector3 Position { get; set; }
		public Quaternion Rotation { get; set; }
		#endregion
		public Action Update;
		List<Component> _components;
		public GameObject()
		{
			_components = new();
			Program.Update += Update;
		}
		public GameObject(params Type[] components)
		{
			_components = new(components.Length);
			for (int i = 0; i < components.Length; i++)
			{
				_components.Add((Component)Activator.CreateInstance(components[i], this));
			}
		}
		~GameObject()
		{
			Program.Update -= Update;
		}
		public T AddComponent<T>() where T : Component
		{
			var component = (T)Activator.CreateInstance(typeof(T), this);
			_components.Add(component);
			return component;
		}
		public T GetComponent<T>() where T : Component
		{
			return (T)_components.Find(c => c.GetType() == typeof(T));
		}
	}
}
