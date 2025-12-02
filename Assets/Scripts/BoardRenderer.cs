using System.Collections.Generic;
using UnityEngine;

public class BoardRenderer : MonoBehaviour
{
    [Header("Prefabs")] public GameObject playerPrefab;
    public GameObject cratePrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject goalPrefab;

    private GameObject _playerObj;
    private Dictionary<Vector2Int, GameObject> _crateObjs = new();

    public void InitBoard(GridState gridState)
    {
        ClearBoard();

        // Spawn Static Grid (Floor, Wall, Goal)
        for (var x = 0; x < gridState.Width; x++)
        {
            for (var y = 0; y < gridState.Height; y++)
            {
                var tileType = gridState.GetTile(new Vector2Int(x, y));
                var pos = GetWorldPosition(x, y);

                switch (tileType)
                {
                    case TileType.Floor:
                        Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                        break;

                    case TileType.Goal:
                        Instantiate(goalPrefab, pos, Quaternion.identity, transform);
                        break;

                    case TileType.Wall:
                        Instantiate(wallPrefab, pos, Quaternion.identity, transform);
                        break;
                }
            }
        }

        // Spawn Player
        var playerWorldPos = GetWorldPosition(gridState.PlayerPos.x, gridState.PlayerPos.y);
        _playerObj = Instantiate(
            playerPrefab,
            new Vector3(playerWorldPos.x, 1, playerWorldPos.z),
            Quaternion.identity,
            transform
        );

        // Spawn Crates
        foreach (var cratePos in gridState.CratesPos)
        {
            var crateWorldPos = GetWorldPosition(cratePos.x, cratePos.y);
            var crateObj = Instantiate(
                cratePrefab,
                new Vector3(crateWorldPos.x, 1, crateWorldPos.z),
                Quaternion.identity,
                transform
            );
            _crateObjs[cratePos] = crateObj;
        }
    }

    private Vector3Int GetWorldPosition(int gridX, int gridY)
    {
        // Map Grid (x, y) to 3D world (x, 0, z)
        return new Vector3Int(gridX, 0, gridY);
    }

    private void ClearBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _crateObjs.Clear();
    }
}