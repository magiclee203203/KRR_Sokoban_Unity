using UnityEngine;
using System.Collections.Generic;
using System.Text;

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

    public bool TryMove(Vector2Int direction, out MovementState movementState)
    {
        movementState = new MovementState
        {
            PlayerStartPos = _playerPos,
            Direction = direction
        };

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

        // move crate
        movementState.PushedCrate = true;
        movementState.CrateStartPos = playerTargetPos;
        movementState.CrateEndPos = crateTargetPos;

        // move player
        _playerPos = playerTargetPos;
        return true;
    }

    public string GetGridStateString()
    {
        var sb = new StringBuilder();

        for (var y = Height - 1; y >= 0; y--)
        {
            for (var x = 0; x < Width; x++)
            {
                var pos = new Vector2Int(x, y);
                var tileType = GetTile(pos);

                var isPlayerOnTile = pos == _playerPos;

                switch (tileType)
                {
                    case TileType.Empty:
                        sb.Append(" ");
                        break;

                    case TileType.Wall:
                        sb.Append("#");
                        break;

                    case TileType.Floor:
                        if (isPlayerOnTile)
                        {
                            sb.Append("p");
                            break;
                        }

                        if (IsCrateAt(pos))
                        {
                            sb.Append("c");
                            break;
                        }

                        sb.Append(".");
                        break;

                    case TileType.Goal:
                        if (isPlayerOnTile)
                        {
                            sb.Append("P");
                            break;
                        }

                        if (IsCrateAt(pos))
                        {
                            sb.Append("C");
                            break;
                        }

                        sb.Append("+");
                        break;
                }
            }

            sb.Append("\n");
        }

        return sb.ToString();
    }
}