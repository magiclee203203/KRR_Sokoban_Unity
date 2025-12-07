using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [Header("References")] public BoardRenderer boardRenderer;
    public APIRequestor apiRequestor;
    public CameraAdjuster cameraAdjuster;
    public ResultPanel resultPanel;

    // Game Board
    private GridState _currentGridState;

    // Player Control
    private PlayerInput _playerInput;

    // used to lock user input
    private bool _isPlayerMoving;
    private bool _isAutoMoving;
    private bool _stopAutoMoving;

    // store hints used in manual mode 
    private Queue<Vector2Int> _hintMoveCommands = new();

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
        _playerInput.Player.Hint.performed += OnAskForHint;
    }

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        _playerInput.Disable();

        _playerInput.Player.Move.performed -= OnPlayerMoved;
        _playerInput.Player.Reset.performed -= OnResetBoard;
        _playerInput.Player.AutoMove.performed -= OnAutoMove;
        _playerInput.Player.Hint.performed -= OnAskForHint;
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

        // handle hint arrow
        if (_hintMoveCommands.Count == 0) return;

        var currentHintDirection = _hintMoveCommands.Dequeue();
        if (currentHintDirection == direction && _hintMoveCommands.Count > 0)
        {
            ShowHintArrow();
        }
        else
        {
            _hintMoveCommands.Clear();
            boardRenderer.ShowHintArrow(false, Vector2Int.zero);
        }
    }

    private async Task MoveOnBoard(Vector2Int direction)
    {
        if (!_currentGridState.TryMove(direction, out var movementState)) return;

        _isPlayerMoving = true;
        await boardRenderer.AnimateMove(movementState);
        _isPlayerMoving = false;

        // check success state
        if (IsSuccess())
        {
            resultPanel.ShowSuccess();
        }
    }

    private void OnResetBoard(InputAction.CallbackContext ctx)
    {
        _stopAutoMoving = true;
        _isAutoMoving = false;
        _isPlayerMoving = false;

        DOTween.KillAll();
        Init();
    }

    private void OnAutoMove(InputAction.CallbackContext ctx)
    {
        if (_isPlayerMoving || _isAutoMoving) return;

        // send GridState
        apiRequestor.PostGridState(_currentGridState.GetGridStateString(), async resp =>
        {
            if (!resp.solvable)
            {
                resultPanel.ShowFail();
                return;
            }

            _isAutoMoving = true;
            _stopAutoMoving = false;

            foreach (var cmd in resp.commands)
            {
                if (_stopAutoMoving)
                {
                    break;
                }

                var dir = Utils.ParseDirectionString(cmd.direction);

                // rotate player
                boardRenderer.RotatePlayer(dir);

                // move player
                await MoveOnBoard(dir);
            }

            _isAutoMoving = false;
        });
    }

    private void OnAskForHint(InputAction.CallbackContext ctx)
    {
        resultPanel.ShowThinking();

        apiRequestor.PostGridState(_currentGridState.GetGridStateString(), resp =>
        {
            resultPanel.Hide();

            if (!resp.solvable)
            {
                resultPanel.ShowFail();
                return;
            }

            // parse commands
            foreach (var cmd in resp.commands)
            {
                _hintMoveCommands.Enqueue(Utils.ParseDirectionString(cmd.direction));
            }

            // _hintMoveCommands
            ShowHintArrow();
        });
    }

    private void Init()
    {
        // hide result panel
        resultPanel.Hide();

        // clear all hints
        _hintMoveCommands.Clear();

        // request map
        apiRequestor.GetLevelData(levelData =>
        {
            _currentGridState = LevelBuilder.CreateGridStateFromRawText(levelData);
            boardRenderer.InitBoard(_currentGridState);
            cameraAdjuster.AdjustCamera(boardRenderer.MapBounds);
        });
    }

    private void ShowHintArrow()
    {
        boardRenderer.ShowHintArrow(true, _hintMoveCommands.Peek());
    }

    private bool IsSuccess()
    {
        var gridStateStr = string.Join("", _currentGridState.GetGridStateString());
        return !gridStateStr.Contains('c');
    }
}