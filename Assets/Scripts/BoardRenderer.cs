using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Threading.Tasks;

public class BoardRenderer : MonoBehaviour
{
    [Header("Prefabs")] public Player playerPrefab;
    public GameObject cratePrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject goalPrefab;

    [Header("Animation Settings")] public float moveDuration = 0.4f;

    private Player _playerObj;
    private Dictionary<Vector2Int, GameObject> _crateObjs = new();

    public Bounds MapBounds
    {
        get
        {
            var mapBounds = new Bounds(Vector3.zero, Vector3.one);
            foreach (Transform tile in transform)
            {
                mapBounds.Encapsulate(tile.position);
            }

            return mapBounds;
        }
    }

    public void InitBoard(GridState gridState)
    {
        ClearBoard();

        // Spawn Static Grid (Floor, Wall, Goal)
        for (var x = 0; x < gridState.Width; x++)
        {
            for (var y = 0; y < gridState.Height; y++)
            {
                var tileType = gridState.GetTile(new Vector2Int(x, y));
                var pos = GetWorldPosition(x, y, false);

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
        var playerWorldPos = GetWorldPosition(gridState.PlayerPos.x, gridState.PlayerPos.y, true);
        _playerObj = Instantiate(playerPrefab, playerWorldPos, Quaternion.identity, transform);

        // Spawn Crates
        foreach (var cratePos in gridState.CratesPos)
        {
            var crateWorldPos = GetWorldPosition(cratePos.x, cratePos.y, true);
            var crateObj = Instantiate(cratePrefab, crateWorldPos, Quaternion.identity, transform);
            _crateObjs[cratePos] = crateObj;
        }
    }

    private Vector3Int GetWorldPosition(int gridX, int gridY, bool higher)
    {
        // Map Grid (x, y) to 3D world (x, 0, z)
        var y = higher ? 1 : 0;
        return new Vector3Int(gridX, y, gridY);
    }

    private void ClearBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        _crateObjs.Clear();
    }

    public async Task AnimateMove(MovementState movementState)
    {
        var seq = DOTween.Sequence();

        // move player
        var playerEndPos = movementState.PlayerStartPos + movementState.Direction;
        var playerEndWorldPos = GetWorldPosition(playerEndPos.x, playerEndPos.y, true);
        seq.Append(_playerObj.transform.DOMove(playerEndWorldPos, moveDuration).SetEase(Ease.OutQuad));

        if (movementState.PushedCrate && _crateObjs.Remove(movementState.CrateStartPos, out var movingCrate))
        {
            // update dictionary
            _crateObjs.Add(movementState.CrateEndPos, movingCrate);

            // move crate
            var crateEndWorldPos = GetWorldPosition(
                movementState.CrateEndPos.x, movementState.CrateEndPos.y, true
            );
            seq.Join(movingCrate.transform.DOMove(crateEndWorldPos, moveDuration).SetEase(Ease.OutQuad));
        }

        await seq.Play().AsyncWaitForCompletion();
    }

    public void RotatePlayer(Vector2Int direction)
    {
        _playerObj.transform.rotation = Utils.GetEulerAngle(direction);
    }

    public void ShowHintArrow(bool show, Vector2Int direction)
    {
        _playerObj.ShowHintArrow(show);

        if (show)
        {
            _playerObj.AdjustArrowDirection(direction);
        }
    }
}