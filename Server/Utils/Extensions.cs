
namespace Server.Utils
{
	public static partial class Extensions
	{
		public static Dictionary<K, V> ResetValues<K, V>(this Dictionary<K, V> dic, V value = default)
		{
			dic.Keys.ToList().ForEach(x => dic[x] = value);
			return dic;
		}
	}
}
