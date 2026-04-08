using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
{
    public event UnityAction<Vector2> OnMoveEvent = delegate { };
    public event UnityAction<Vector2> OnLookEvent = delegate { };
    public event UnityAction OnSprintStarted = delegate { };
    public event UnityAction OnSprintCanceled = delegate { };
    public event UnityAction OnAttackEvent = delegate { };
    public event UnityAction OnBlockStarted = delegate { };
    public event UnityAction OnBlockCanceled = delegate { };

    private PlayerInputActions _actions;

    private void OnEnable()
    {
        if (_actions == null)
        {
            _actions = new PlayerInputActions();
            _actions.Player.SetCallbacks(this);
        }
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions?.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext ctx)
        => OnMoveEvent.Invoke(ctx.ReadValue<Vector2>());

    public void OnLook(InputAction.CallbackContext ctx)
        => OnLookEvent.Invoke(ctx.ReadValue<Vector2>());

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) OnSprintStarted.Invoke();
        if (ctx.phase == InputActionPhase.Canceled) OnSprintCanceled.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) OnAttackEvent.Invoke();
    }

    public void OnBlock(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Started) OnBlockStarted.Invoke();
        if (ctx.phase == InputActionPhase.Canceled) OnBlockCanceled.Invoke();
    }

    public void OnCrouch(InputAction.CallbackContext ctx) { }
    public void OnJump(InputAction.CallbackContext ctx) { }
    public void OnInteract(InputAction.CallbackContext ctx) { }
    public void OnPrevious(InputAction.CallbackContext ctx) { }
    public void OnNext(InputAction.CallbackContext ctx) { }
}