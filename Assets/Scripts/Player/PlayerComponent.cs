using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG2D
{
    public class PlayerComponent : MonoBehaviour
    {
        public Rigidbody2D RB { get; private set; }
        public Animator Animator { get; private set; }
        public Vector2 MoveInput => Input.MoveDirection;
        public bool IsGrounded => checkBlock.IsGrounded;
        public bool HasRightWall => checkBlock.HasRightWall;
        public bool HasLeftWall => checkBlock.HasLeftWall;
        public PlayerInput Input { get; private set; }
        private CheckBlock checkBlock;

        private void Awake()
        {
            RB = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            checkBlock = GetComponent<CheckBlock>();
            Input = gameObject.GetComponent<PlayerInput>();
        }
        private void Update()
        {
            Animator.SetBool("IsGrounded", IsGrounded);
        }
        public void SetGravityScale(float scale)
        {
            RB.gravityScale = scale;    
        }
    }
}
