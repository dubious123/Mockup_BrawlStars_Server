namespace Server.Game
{
	public class MapData
	{
		public Vector2[] SpawnPosArr { get; init; } = new Vector2[6];

		private readonly int offsetX;
		private readonly int offsetY;
		private int[][] _map;

		public MapData(string path)
		{
			var lines = File.ReadAllLines(path);
			offsetX = lines[0].Length / 2;
			offsetY = lines.Length / 2;
			_map = new int[lines.Length][];
			int spawnCount = default;
			for (int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				_map[i] = new int[line.Length];
				for (int j = 0; j < line.Length; j++)
				{
					if (line[j] == '2' || line[j] == '3')
					{
						SpawnPosArr[spawnCount++] = new Vector2(j - offsetX + 0.5f, i - offsetY + 0.5f);
					}

					_map[i][j] = line[j] == '1' ? (int)TileType.Wall : (int)TileType.Emtpy;
				}
			}
		}

		public bool CanGo(Vector2 pos)
		{
			var x = (int)(pos.X - 0.5f) + offsetX;
			var y = (int)(pos.Y - 0.5f) + offsetY;
			return HasTile(x, y) && _map[y][x] != (int)TileType.Wall;
		}

		private bool HasTile(int x, int y)
		{
			if (_map.Length <= y || y < 0)
				return false;
			return _map[y].Length > x && x >= 0;
		}
	}
}
