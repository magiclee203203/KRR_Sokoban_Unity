using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
{
    public static GridState CreateGridStateFromData(LevelData data)
    {
        var layout = new TileType[data.Width, data.Height];
        var playerPos = Vector2Int.zero;
        var cratesPos = new List<Vector2Int>();

        for (var y = 0; y < data.Height; y++)
        {
            var rowStr = data.rows[y];

            for (var x = 0; x < data.Width; x++)
            {
                var tileChar = rowStr[x];
                var tileType = TileType.Empty;

                switch (tileChar)
                {
                    case '#':
                        tileType = TileType.Wall;
                        break;

                    case '.':
                        tileType = TileType.Floor;
                        break;

                    case '+':
                        tileType = TileType.Goal;
                        break;

                    case '@':
                        tileType = TileType.Floor;
                        playerPos = new Vector2Int(x, GetInversedY(data.Height, y));
                        break;

                    case '$':
                        tileType = TileType.Floor;
                        cratesPos.Add(new Vector2Int(x, GetInversedY(data.Height, y)));
                        break;
                }

                layout[x, GetInversedY(data.Height, y)] = tileType;
            }
        }

        return new GridState(layout, playerPos, cratesPos);
    }

    private static int GetInversedY(int boardHeight, int y)
    {
        return boardHeight - y - 1;
    }
}