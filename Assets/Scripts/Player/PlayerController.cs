using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG2D
{
    public class PlayerController : MonoBehaviour
    {
        public Rigidbody2D RB { get; private set; }
        public Animator Animator { get; private set; }
        public Vector2 MoveInput => Input.MoveDirection;
        public bool IsGrounded => checkBlock.IsGrounded;
        public bool HasRightWall => checkBlock.HasRightWall;
        public bool HasLeftWall => checkBlock.HasLeftWall;
        public bool IsJumping => RB.linearVelocity.y > 0.1f;
        public bool IsFalling => RB.linearVelocity.y < -0.1f;
        private PlayerInput Input;
        private CheckBlock checkBlock;

        public event UnityAction PlayerJump;
        public event UnityAction PlayerDash;
        public event UnityAction<bool> PlayerAttack;

        private void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            checkBlock = GetComponent<CheckBlock>();
            Input = gameObject.GetComponent<PlayerInput>();
        }
        private void Start()
        {
            Input.JumpPressed += Jump;
            Input.Dash += Dash;
            Input.Attack += Attack;
        }
        private void Update()
        {
            Animator.SetBool("IsGrounded", IsGrounded);
        }
        public void Jump()
        {
            PlayerJump.Invoke();    
        }
        public void Dash()
        {
            PlayerDash.Invoke();
        }
        public void Attack(bool arg0)
        {
            PlayerAttack.Invoke(arg0);  
        }
        public void SetGravityScale(float scale)
        {
            RB.gravityScale = scale;
        }
    }
}
