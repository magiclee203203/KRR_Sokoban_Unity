using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFile", menuName = "Sokoban/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Layout(#=Wall, .=Floor, @=Player, $=Crate, +=Goal)")]
    public List<string> rows = new();

    public int Height => rows.Count;

    public int Width
    {
        get
        {
            if (rows.Count == 0)
            {
                return 0;
            }

            var longestRow = 0;

            foreach (var row in rows)
            {
                if (row.Length > longestRow)
                {
                    longestRow = row.Length;
                }
            }

            return longestRow;
        }
    }
}