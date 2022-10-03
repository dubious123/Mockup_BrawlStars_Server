namespace Server
{
	public class MapMgr
	{
		private static MapMgr _instance = new();
		private ConcurrentDictionary<ushort, MapData> _mapDict;

		private MapMgr()
		{
			_mapDict = new();
		}

		public static void Init()
		{
			_instance._mapDict.TryAdd(0, new MapData("../../../Data/MapData_00.txt"));
		}

		public static MapData GetMapData(ushort mapId)
		{
			return _instance._mapDict.TryGetValue(mapId, out var value) ? value : null;
		}
	}
}
