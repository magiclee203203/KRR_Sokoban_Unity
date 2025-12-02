using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [Header("Config")] public LevelData currentLevel;
    [Header("References")] public BoardRenderer boardRenderer;

    // Game Board
    private GridState _currentGridState;

    // Player Control
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        _playerInput.Player.Move.performed += OnPlayerMoved;
    }

    private void Start()
    {
        _currentGridState = LevelBuilder.CreateGridStateFromData(currentLevel);
        boardRenderer.InitBoard(_currentGridState);
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        _playerInput.Player.Move.performed -= OnPlayerMoved;
    }

    private void OnPlayerMoved(InputAction.CallbackContext ctx)
    {
        var val = ctx.ReadValue<Vector2>();
        var direction = new Vector2Int(Mathf.RoundToInt(val.x), Mathf.RoundToInt(val.y));

        // prevent diagonal movement
        if (direction.x != 0 && direction.y != 0) return;

        MoveOnBoard(direction);
    }

    private void MoveOnBoard(Vector2Int direction)
    {
        if (_currentGridState.TryMove(direction))
        {
            Debug.Log("Move!");
            boardRenderer.InitBoard(_currentGridState);
        }
        else
        {
            Debug.Log("Blocked!");
        }
    }
}