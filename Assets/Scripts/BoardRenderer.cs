using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System;

public class BoardRenderer : MonoBehaviour
{
    [Header("Prefabs")] public GameObject playerPrefab;
    public GameObject cratePrefab;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject goalPrefab;

    [Header("Animation Settings")] public float moveDuration = 0.4f;

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

    public void AnimateMove(MovementState movementState, Action onCompleteCallback = null)
    {
        var seq = DOTween.Sequence();
        seq.OnComplete(() => { onCompleteCallback?.Invoke(); });

        // move player
        var playerEndPos = movementState.PlayerStartPos + movementState.Direction;
        var playerEndWorldPos = GetWorldPosition(playerEndPos.x, playerEndPos.y, true);
        var playerMove = _playerObj.transform
            .DOMove(playerEndWorldPos, moveDuration).SetEase(Ease.OutQuad);

        seq.Append(playerMove);

        if (!movementState.PushedCrate)
        {
            seq.Play();
            return;
        }

        if (!_crateObjs.Remove(movementState.CrateStartPos, out var movingCrate)) return;

        // update dictionary
        _crateObjs.Add(movementState.CrateEndPos, movingCrate);

        // move crate
        var crateEndWorldPos = GetWorldPosition(
            movementState.CrateEndPos.x, movementState.CrateEndPos.y, true
        );
        var crateMove = movingCrate.transform
            .DOMove(crateEndWorldPos, moveDuration).SetEase(Ease.OutQuad);

        seq.Join(crateMove);
        seq.Play();
    }
}