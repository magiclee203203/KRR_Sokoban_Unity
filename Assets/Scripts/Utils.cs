using UnityEngine;

public class Utils
{
    public static Quaternion GetEulerAngle(Vector2Int direction)
    {
        return direction switch
        {
            { x: 0, y: 1 } => Quaternion.Euler(0, 180, 0),
            { x: 0, y: -1 } => Quaternion.Euler(0, 0, 0),
            { x: 1, y: 0 } => Quaternion.Euler(0, -90, 0),
            { x: -1, y: 0 } => Quaternion.Euler(0, 90, 0),
            _ => Quaternion.Euler(0, 0, 0)
        };
    }

    public static Vector2Int ParseDirectionString(string dirStr)
    {
        return dirStr switch
        {
            "up" => Vector2Int.up,
            "down" => Vector2Int.down,
            "left" => Vector2Int.left,
            "right" => Vector2Int.right,
            _ => Vector2Int.zero
        };
    }

    public static Quaternion GetHintArrowAngle(Vector2Int direction)
    {
        return direction switch
        {
            { x: 0, y: 1 } => Quaternion.LookRotation(Vector3.forward, Vector3.up),
            { x: 0, y: -1 } => Quaternion.LookRotation(Vector3.back, Vector3.up),
            { x: 1, y: 0 } => Quaternion.LookRotation(Vector3.right, Vector3.up),
            { x: -1, y: 0 } => Quaternion.LookRotation(Vector3.left, Vector3.up),
            _ => Quaternion.LookRotation(Vector3.zero, Vector3.up)
        };
    }
}