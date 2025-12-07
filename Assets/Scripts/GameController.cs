using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [Header("References")] public BoardRenderer boardRenderer;
    public APIRequestor apiRequestor;
    public CameraAdjuster cameraAdjuster;

    // Game Board
    private GridState _currentGridState;

    // Player Control
    private PlayerInput _playerInput;

    // used to lock user input
    private bool _isPlayerMoving;
    private bool _isAutoMoving;
    private bool _stopAutoMoving;

    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();

        _playerInput.Player.Move.performed += OnPlayerMoved;
        _playerInput.Player.Reset.performed += OnResetBoard;
        _playerInput.Player.AutoMove.performed += OnAutoMove;
    }

    private void Start()
    {
        InitBoard();
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _playerInput.Player.Move.performed -= OnPlayerMoved;
        _playerInput.Player.Reset.performed -= OnResetBoard;
        _playerInput.Player.AutoMove.performed -= OnAutoMove;
    }

    private void OnPlayerMoved(InputAction.CallbackContext ctx)
    {
        // lock user input
        if (_isPlayerMoving || _isAutoMoving) return;

        var val = ctx.ReadValue<Vector2>();
        var direction = new Vector2Int(Mathf.RoundToInt(val.x), Mathf.RoundToInt(val.y));

        // prevent diagonal movement
        if (direction.x != 0 && direction.y != 0) return;

        // rotate player
        boardRenderer.RotatePlayer(direction);

        // move
        _ = MoveOnBoard(direction);
    }

    private async Task MoveOnBoard(Vector2Int direction)
    {
        if (!_currentGridState.TryMove(direction, out var movementState)) return;

        _isPlayerMoving = true;
        await boardRenderer.AnimateMove(movementState);
        _isPlayerMoving = false;
    }

    private void OnResetBoard(InputAction.CallbackContext ctx)
    {
        _stopAutoMoving = true;
        _isAutoMoving = false;
        _isPlayerMoving = false;

        DOTween.KillAll();
        InitBoard();
    }

    private void OnAutoMove(InputAction.CallbackContext ctx)
    {
        if (_isPlayerMoving || _isAutoMoving) return;

        // send GridState
        apiRequestor.PostGridState(_currentGridState.GetGridStateString(), async (commands) =>
        {
            _isAutoMoving = true;
            _stopAutoMoving = false;

            foreach (var cmd in commands)
            {
                if (_stopAutoMoving)
                {
                    break;
                }

                var dir = ParseDirection(cmd.direction);

                // rotate player
                boardRenderer.RotatePlayer(dir);

                // move player
                await MoveOnBoard(dir);
            }

            _isAutoMoving = false;
        });
    }

    private void InitBoard()
    {
        apiRequestor.GetLevelData(levelData =>
        {
            _currentGridState = LevelBuilder.CreateGridStateFromRawText(levelData);
            boardRenderer.InitBoard(_currentGridState);
            cameraAdjuster.AdjustCamera(boardRenderer.MapBounds);
        });
    }

    private static Vector2Int ParseDirection(string dirStr)
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
}