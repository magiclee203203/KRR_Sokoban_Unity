using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder
{
    public static GridState CreateGridStateFromRawText(string rawText)
    {
        var rows = new List<string>(rawText.Split("\n"));

        // calculate size
        var height = rows.Count;
        var width = 0;

        foreach (var row in rows)
        {
            if (row.Length > width)
            {
                width = row.Length;
            }
        }

        return CreateGridStateFromRows(rows, width, height);
    }

    private static GridState CreateGridStateFromRows(List<string> rows, int width, int height)
    {
        var layout = new TileType[width, height];
        var playerPos = Vector2Int.zero;
        var cratesPos = new List<Vector2Int>();

        for (var y = 0; y < height; y++)
        {
            var rowStr = rows[y];

            for (var x = 0; x < width; x++)
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

                    case 'p':
                        tileType = TileType.Floor;
                        playerPos = new Vector2Int(x, GetInversedY(height, y));
                        break;

                    case 'c':
                        tileType = TileType.Floor;
                        cratesPos.Add(new Vector2Int(x, GetInversedY(height, y)));
                        break;
                }

                layout[x, GetInversedY(height, y)] = tileType;
            }
        }

        return new GridState(layout, playerPos, cratesPos);
    }

    private static int GetInversedY(int boardHeight, int y)
    {
        return boardHeight - y - 1;
    }
}