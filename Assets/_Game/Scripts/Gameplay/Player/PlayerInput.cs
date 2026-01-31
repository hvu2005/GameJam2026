
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerInput : EventTarget
{
    private Input _inputActions;
    private Vector2 _moveInput;

    public Vector2 MoveInput => _moveInput;

    public void ClearInput()
    {
        _moveInput = Vector2.zero;

        // Flush Input System buffer by disabling/re-enabling
        if (_inputActions != null && _inputActions.GamePlay.enabled)
        {
            _inputActions.GamePlay.Disable();
            _inputActions.GamePlay.Enable();
        }
    }

    private void Awake()
    {
        _inputActions = new Input();
    }

    private void OnEnable()
    {
        // Clear any cached input when re-enabling
        _moveInput = Vector2.zero;

        _inputActions.GamePlay.Enable();

        _inputActions.GamePlay.Move.performed += OnMovePerformed;
        _inputActions.GamePlay.Move.canceled += OnMoveCanceled;
        _inputActions.GamePlay.Jump.performed += OnJumpPerformed;
        _inputActions.GamePlay.Dash.performed += OnDashPerformed;
        _inputActions.GamePlay.Skill.started += OnSkillPerformed;
        _inputActions.GamePlay.Form1.performed += OnForm1Performed;
        _inputActions.GamePlay.Form2.performed += OnForm2Performed;
        _inputActions.GamePlay.Form3.performed += OnForm3Performed;
    }

    private void OnDisable()
    {
        if (_inputActions == null) return;

        // Clear input buffer when disabling
        _moveInput = Vector2.zero;

        _inputActions.GamePlay.Move.performed -= OnMovePerformed;
        _inputActions.GamePlay.Move.canceled -= OnMoveCanceled;
        _inputActions.GamePlay.Jump.performed -= OnJumpPerformed;
        _inputActions.GamePlay.Dash.performed -= OnDashPerformed;
        _inputActions.GamePlay.Skill.started -= OnSkillPerformed;
        _inputActions.GamePlay.Form1.performed -= OnForm1Performed;
        _inputActions.GamePlay.Form2.performed -= OnForm2Performed;
        _inputActions.GamePlay.Form3.performed -= OnForm3Performed;

        _inputActions.GamePlay.Disable();
    }

    private void OnDestroy()
    {
        _inputActions?.Dispose();
    }

    private void Update()
    {
        if (_moveInput.x != 0)
        {
            Vector2 horizontalMove = new Vector2(_moveInput.x, 0f);
            this.Emit<Vector2>(PlayerInputType.Move, horizontalMove);
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
        this.Emit<Vector2>(PlayerInputType.Move, Vector2.zero);
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        this.Emit<bool>(PlayerInputType.Jump, true);
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        this.Emit<bool>(PlayerInputType.Dash, true);
    }

    private void OnSkillPerformed(InputAction.CallbackContext context)
    {
        this.Emit<bool>(PlayerInputType.Skill, true);
    }

    private void OnForm1Performed(InputAction.CallbackContext context)
    {
        this.Emit<int>(PlayerInputType.FormSelect, 1);
    }

    private void OnForm2Performed(InputAction.CallbackContext context)
    {
        this.Emit<int>(PlayerInputType.FormSelect, 2);
    }

    private void OnForm3Performed(InputAction.CallbackContext context)
    {
        this.Emit<int>(PlayerInputType.FormSelect, 3);
    }
}
