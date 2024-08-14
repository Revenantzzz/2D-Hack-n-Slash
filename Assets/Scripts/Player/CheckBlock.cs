using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG2D
{
    public class CheckBlock : MonoBehaviour
    {
        [SerializeField] PlayerSettingData Data;

        [SerializeField] LayerMask groundLayer;
        [SerializeField] LayerMask wallLayer;

        Vector2 size;
        float groundCheckDistance = 0f;
        float wallCheckDistance = 0f;

        public bool IsGrounded => CheckGround();
        public bool HasRightWall => CheckRightWall();
        public bool HasLeftWall => CheckLeftWall(); 
        private void Start()
        {
            size = new Vector2(Data.playerWidth -.02f, Data.playerHeight -.02f);
            groundCheckDistance = 0.2f;
            wallCheckDistance =  0.5f;
        }
        private void Update()
        {
            CheckGround();
            CheckLeftWall();
            CheckRightWall();
        }
        private bool CheckGround()
        {
           return Physics2D.BoxCast(transform.position, size, 0f, Vector2.down, groundCheckDistance, groundLayer);
        }
        private bool CheckRightWall()
        {
            return Physics2D.BoxCast(transform.position, size, 0f, Vector2.right, wallCheckDistance, wallLayer);
        }
        private bool CheckLeftWall()
        {
            return Physics2D.BoxCast(transform.position, size, 0f, Vector2.left, wallCheckDistance, wallLayer);
        }
    }
}
