using UnityEngine;
using System.Collections.Generic;

public class GridState
{
    private readonly TileType[,] _board;
    private Vector2Int _playerPos;
    private readonly List<Vector2Int> _cratesPos;

    public int Width => _board.GetLength(0);
    public int Height => _board.GetLength(1);

    public Vector2Int PlayerPos => _playerPos;
    public IReadOnlyList<Vector2Int> CratesPos => _cratesPos;

    public GridState(TileType[,] initLayout, Vector2Int playerStartPos, List<Vector2Int> cratesStartPos)
    {
        _board = initLayout;
        _playerPos = playerStartPos;
        _cratesPos = cratesStartPos;
    }

    public TileType GetTile(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= Width || pos.y < 0 || pos.y >= Height) return TileType.Wall;
        return _board[pos.x, pos.y];
    }

    private bool IsWalkable(Vector2Int pos)
    {
        var tile = GetTile(pos);
        return tile != TileType.Wall && tile != TileType.Empty;
    }

    private bool IsCrateAt(Vector2Int pos)
    {
        return _cratesPos.Contains(pos);
    }

    public bool TryMove(Vector2Int direction)
    {
        var playerTargetPos = _playerPos + direction;

        if (!IsWalkable(playerTargetPos)) return false;

        if (!IsCrateAt(playerTargetPos))
        {
            // move player
            _playerPos = playerTargetPos;
            return true;
        }

        // deal with pushing a crate
        var crateTargetPos = playerTargetPos + direction;
        if (!IsWalkable(crateTargetPos) || IsCrateAt(crateTargetPos)) return false;

        var targetCrateIdx = _cratesPos.IndexOf(playerTargetPos);
        _cratesPos[targetCrateIdx] = crateTargetPos;

        // move player
        _playerPos = playerTargetPos;

        return true;
    }
}