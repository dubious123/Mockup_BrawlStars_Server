namespace Server;

public class PlayerMgr
{
	private static PlayerMgr _intstance = new();
	private ConcurrentDictionary<int, Player> _playerDict;

	private PlayerMgr()
	{
		_playerDict = new();
	}

	public static Player GetOrAddPlayer(int userId, ClientSession session)
	{
		if (_intstance._playerDict.TryGetValue(userId, out var player) == true)
		{
			return player;
		}

		player = new Player(userId, session);
		_intstance._playerDict.TryAdd(userId, player);
		return player;
	}

	public static Player GetPlayer(int userId)
	{
		_intstance._playerDict.TryGetValue(userId, out var player);
		return player;
	}
}
