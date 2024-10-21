using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public event UnityAction<bool> JumpHeld;
    public event UnityAction JumpPressed;
    public event UnityAction<bool> Attack;
    public event UnityAction Dash;
    public event UnityAction<bool> BlockHeld;
    public event UnityAction BlockPressed;
    public event UnityAction Heal;
    public event UnityAction Interact;
    public event UnityAction Cast;
    public event UnityAction<bool> ChargeCast;
    public Vector2 MoveVector { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveVector = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            JumpHeld?.Invoke(true);
            JumpPressed?.Invoke();
        }
        else if(context.canceled)
        {
            JumpHeld?.Invoke(false);  
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Attack.Invoke(true);
        }
        else if(context.canceled)
        {
            Attack.Invoke(false);
        }
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Dash?.Invoke();
        }
    }
    public void OnBlock(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            BlockPressed?.Invoke();
            BlockHeld?.Invoke(true);
        }
        else if(context.canceled)
        {
            BlockHeld?.Invoke(false);
        }
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Heal?.Invoke();
        }
    }  
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Interact?.Invoke();
        }
    }
    public void OnCast(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            ChargeCast?.Invoke(true);
        }
        if(context.canceled)
        {
            ChargeCast?.Invoke(false);
            Cast?.Invoke();
        }        
    }
}
