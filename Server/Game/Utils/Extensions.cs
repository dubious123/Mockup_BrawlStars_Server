using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

using static Server.Utils.Enums;

namespace Server.Game
{
	public static partial class Extensions
	{
		public static T GetComponent<T>(this NetObject netObj) where T : NetBaseComponent
		{
			var type = typeof(T);
			return type switch
			{
				_ when typeof(NetCollider2D).IsAssignableFrom(type)
					=> netObj.World.ColliderSystem.GetComponent(netObj.ObjectId) as T,
				_ when typeof(NetCharacter).IsAssignableFrom(type)
					=> netObj.World.CharacterSystem.GetComponent(netObj.ObjectId) as T,
				_ when typeof(NetEnv).IsAssignableFrom(type)
					=> netObj.World.EnvSystem.GetComponent(netObj.ObjectId) as T,
				_ when typeof(NetProjectile).IsAssignableFrom(type)
					=> netObj.World.ProjectileSystem.GetComponent(netObj.ObjectId) as T,
				_ => null,
			};
		}

		public static T GetComponent<T>(this NetBaseComponent component) where T : NetBaseComponent
		{
			var type = typeof(T);
			return type switch
			{
				_ when typeof(NetCollider2D).IsAssignableFrom(type)
					=> component.World.ColliderSystem.GetComponent(component.NetObj.ObjectId) as T,
				_ when typeof(NetCharacter).IsAssignableFrom(type)
					=> component.World.CharacterSystem.GetComponent(component.NetObj.ObjectId) as T,
				_ when typeof(NetEnv).IsAssignableFrom(type)
					=> component.World.EnvSystem.GetComponent(component.NetObj.ObjectId) as T,
				_ when typeof(NetProjectile).IsAssignableFrom(type)
					=> component.World.ProjectileSystem.GetComponent(component.NetObj.ObjectId) as T,
				_ => null,
			};
		}

		public static T AddComponent<T>(this NetObject netObj) where T : NetBaseComponent, new()
		{
			var inst = new T() { NetObj = netObj };
			inst.Start();
			var world = netObj.World;

			switch (inst)
			{
				case NetCollider2D collider:
					world.ColliderSystem.AddComponent(netObj.ObjectId, collider);
					break;
				case NetCharacter character:
					world.CharacterSystem.AddComponent(netObj.ObjectId, character);
					break;
				case NetEnv env:
					world.EnvSystem.AddComponent(netObj.ObjectId, env);
					break;
				case NetProjectile projectile:
					world.ProjectileSystem.AddComponent(netObj.ObjectId, projectile);
					break;
				default:
					throw new Exception($"invalid type {inst.GetType()}");
			}

			return inst;
		}
	}
}
