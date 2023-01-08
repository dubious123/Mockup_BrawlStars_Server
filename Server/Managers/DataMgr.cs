using Server.Game.Data;

namespace Server;

public class DataMgr
{
	private static DataMgr _instance;
	private readonly WorldData _data;

	private DataMgr()
	{
		var str = File.ReadAllText("../../../Data/WorldData.txt");
		var options = new JsonSerializerOptions
		{
			IncludeFields = true,
			WriteIndented = true,
			Converters =
				{
					new sfloatConverter(),
				}
		};
		_data = JsonSerializer.Deserialize<WorldData>(str, options);
	}

	public static void Init() => _instance = new();

	public static WorldData GetWorldData()
	{
		return _instance._data;
	}
}
