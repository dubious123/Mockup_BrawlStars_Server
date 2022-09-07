using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Numerics;
using static Server.Utils.Enums;

namespace Server.Game
{
	public class MapData
	{
		readonly int offset_X;
		readonly int offset_Y;

		int[][] _map;
		public MapData(string path)
		{
			var lines = File.ReadAllLines(path);
			offset_X = lines[0].Length / 2;
			offset_Y = lines.Length / 2;
			_map = new int[lines.Length][];
			for (int i = 0; i < lines.Length; i++)
			{
				var line = lines[i];
				_map[i] = new int[line.Length];
				for (int j = 0; j < line.Length; j++)
				{
					_map[i][j] = line[j] == '0' ? (int)TileType.Emtpy : (int)TileType.Wall;
				}
			}
		}

		public bool CanGo(Vector2 pos)
		{
			var x = (int)(pos.X - 0.5f) + offset_X;
			var y = (int)(pos.Y - 0.5f) + offset_Y;
			Console.WriteLine($"X:{x} Y:{y}");
			return HasTile(x, y) && _map[y][x] != (int)TileType.Wall;

		}
		bool HasTile(int x, int y)
		{
			if (_map.Length <= y || y < 0)
				return false;
			return _map[y].Length > x && x >= 0;
		}
	}
}
