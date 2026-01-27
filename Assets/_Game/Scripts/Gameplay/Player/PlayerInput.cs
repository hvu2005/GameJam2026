
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerInput : EventTarget
{
    private Input _inputActions;
    private Vector2 _moveInput;

    private void Awake()
    {
        _inputActions = new Input();
    }

    private void OnEnable()
    {
        _inputActions.GamePlay.Enable();
        
        _inputActions.GamePlay.Move.performed += OnMovePerformed;
        _inputActions.GamePlay.Move.canceled += OnMoveCanceled;
        _inputActions.GamePlay.Jump.performed += OnJumpPerformed;
        _inputActions.GamePlay.Dash.performed += OnDashPerformed;
        _inputActions.GamePlay.Skill.started += OnSkillPerformed;
    }

    private void OnDisable()
    {
        _inputActions.GamePlay.Move.performed -= OnMovePerformed;
        _inputActions.GamePlay.Move.canceled -= OnMoveCanceled;
        _inputActions.GamePlay.Jump.performed -= OnJumpPerformed;
        _inputActions.GamePlay.Dash.performed -= OnDashPerformed;
        _inputActions.GamePlay.Skill.started -= OnSkillPerformed;
        
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
}
