using UnityEngine;

public struct MovementState
{
    public Vector2Int Direction;
    public Vector2Int PlayerStartPos;
    public bool PushedCrate;
    public Vector2Int CrateStartPos;
    public Vector2Int CrateEndPos;

    public override string ToString()
    {
        return $"MovementState(Direction={Direction}, " +
               $"PlayerStartPos={PlayerStartPos}, " +
               $"PushedCrate={PushedCrate}, " +
               $"CrateStartPos={CrateStartPos}, " +
               $"CrateEndPos={CrateEndPos})";
    }
}