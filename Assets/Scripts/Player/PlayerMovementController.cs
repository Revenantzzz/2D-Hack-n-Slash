using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG2D
{
    public class PlayerMovementController : MonoBehaviour
    {
        const string ANIMBOOL_ISWALKING = "IsWalking";

        [SerializeField] PlayerSettingData Data;
        PlayerController player;
        PlayerJumpController JumpController;
        PlayerDashController DashController;

        float accelRate = 0;
        float movement;
        float runAccelAmount = 0;
        float runDeccelAmount = 0;

        private bool MovingRight => player.MoveInput != Vector2.left;
        public bool FacingRight => transform.localScale.x == 1;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
            JumpController = GetComponent<PlayerJumpController>();
            DashController = GetComponent<PlayerDashController>();
        }
        private void Start()
        {
            runAccelAmount = (50 * Data.runAccel) / Data.maxSpeed;
            runDeccelAmount = (50 * Data.runDeccel) / Data.maxSpeed;
        }
        private void FixedUpdate()
        {
            if(DashController.IsDashing)
            {
                if (DashController.IsEndDashing)
                {
                    HandleMove(Data.dashEndRunLerp);
                }
                return;
            }           
            if(JumpController.IsWallJumping)
            {
                HandleMove(Data.wallJumpRunLerp);
                return;
            }
            HandleMove(1);
        }
        private void HandleMove(float lerp)
        {
            Turn();         
            float targetSpeed = player.MoveInput.x * Data.maxSpeed;
            targetSpeed = Mathf.Lerp(player.RB.velocity.x, targetSpeed, lerp);
            if(player.IsGrounded)
            {
                accelRate = ((Mathf.Abs(targetSpeed)) > 0.01f) ? runAccelAmount : runDeccelAmount;               
            }
            else
            {
                accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccelAmount * Data.accelInAir : runDeccelAmount * Data.deccelInAir;
            }
            if ((player.IsJumping || player.IsFalling || JumpController.IsWallJumping) && Mathf.Abs(player.RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                accelRate *= Data.jumpHangAccelerationMultiple;
                targetSpeed *= Data.jumpHangMaxSpeedMultiple;
            }
            if (Data.conserveMomentum &&
                Mathf.Abs(player.RB.velocity.x) > Mathf.Abs(targetSpeed)&&
                Mathf.Sign(player.RB.velocity.x) == Mathf.Sign(targetSpeed) && 
                Mathf.Abs(targetSpeed) > 0.01f && 
                player.IsGrounded)
                {                
                accelRate = 0;
                } 
            if(!player.IsGrounded && (player.HasLeftWall || player.HasRightWall))
            {
               accelRate = 0;
            }
            float speedDif = targetSpeed - player.RB.velocity.x;
            movement = speedDif * accelRate;
            player.RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
            player.Animator.SetBool(ANIMBOOL_ISWALKING, Mathf.Abs(movement) > 0.01f && !player.IsJumping);
        }
        private void Turn()
        {
            if ((MovingRight ^ FacingRight) && player.MoveInput.magnitude != 0f)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
        }
    }
}
