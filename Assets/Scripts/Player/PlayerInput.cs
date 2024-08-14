using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace RPG2D
{
    public class PlayerInput : MonoBehaviour
    {
        public event UnityAction OnPlayerJump;
        public event UnityAction OnPlayerDash;
        public event UnityAction OnPlayerAttack;
        public Vector2 MoveDirection => move.normalized;

        private Vector2 move;
        public void OnMove(InputAction.CallbackContext context)
        {
            move = context.ReadValue<Vector2>();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                OnPlayerJump?.Invoke();
            }          
        }
        public void OnDash(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerDash?.Invoke();
            }          
        }
        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                OnPlayerAttack.Invoke();
            }
        }
    }
}
