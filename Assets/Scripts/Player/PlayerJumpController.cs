using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG2D
{
    public class PlayerJumpController : MonoBehaviour
    {
        const string ANIMBOOL_ISJUMPING = "IsJumping";
        const string ANIMBOOL_ISFALLING = "IsFalling";
        const string ANIMBOOL_ISWALLJUMP = "WallJump";

        [SerializeField] PlayerSettingData Data;
        PlayerController player;    

        public bool IsHardFalling {  get; private set; }
        public bool IsJumpCut { get; private set; }
        public bool IsWallJumping { get; private set; }

        List<Timer> timerList = new List<Timer>();
        CountDownTimer leaveGroundTimer;
        CountDownTimer jumpTimer;
        CountDownTimer jumpCoolDownTimer;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            jumpTimer = new CountDownTimer(Data.jumpDuration);
            jumpCoolDownTimer = new CountDownTimer(Data.jumpCoolDown);
            leaveGroundTimer = new CountDownTimer(Data.jumpCoyoteTime);

            timerList = new List<Timer>(capacity: 3) { jumpTimer, jumpCoolDownTimer, leaveGroundTimer};
            jumpTimer.OnTimerStop += () => jumpCoolDownTimer.OnTimerStart();
        }
        private void Start()
        {
            Data.SetGravityData();
            player.SetGravityScale(Data.gravityScale);
            player.PlayerJump += Jump;
        }
        private void Update()
        {
            IsGrounded(player.IsGrounded);
            HandleTimer();
            HandleJump();
            ManageGravity();
            SetAnimation(player.Animator);
        }
        
       private void SetAnimation(Animator animator)
        {
            animator.SetBool(ANIMBOOL_ISJUMPING, player.IsJumping);
            animator.SetBool(ANIMBOOL_ISFALLING, player.IsFalling);
            
        }
        private void HandleTimer()
        {
            foreach(Timer timer in timerList)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        private bool IsGrounded(bool data)
        {
            if (!data) leaveGroundTimer.StartTimer();
            return data;
        }
        private void Jump()
        {
            if(!jumpCoolDownTimer.IsRunning && !jumpTimer.IsRunning)
            {
                jumpTimer.StartTimer();
            }          
        }
        private void HandleJump()
        {
            if(!jumpTimer.IsRunning)
            {
                IsJumpCut = false;
            }
            if(jumpTimer.IsRunning && (IsGrounded(player.IsGrounded)))
            {
                float force = Data.jumpForce;
                if (player.RB.linearVelocity.y < 0)
                {
                    force -= player.RB.linearVelocity.y;
                }
                player.RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
                IsJumpCut = true;
                jumpTimer.StopTimer();
                return;
            }
            if (jumpTimer.IsRunning && (player.HasLeftWall || player.HasRightWall))
            {
                IsWallJumping = true;
                float dir = 0f;
                player.Animator.SetTrigger(ANIMBOOL_ISWALLJUMP);
                if (player.HasRightWall) dir = -1f;
                if (player.HasLeftWall) dir = 1f;
                Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
                force.x *= dir; //apply force in opposite direction of wall

                if (Mathf.Sign(player.RB.linearVelocity.x) != Mathf.Sign(force.x))
                    force.x -= player.RB.linearVelocity.x;

                if (player.RB.linearVelocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
                    force.y -= player.RB.linearVelocity.y;

                player.RB.AddForce(force, ForceMode2D.Impulse);
                IsJumpCut = true;
                jumpTimer.StopTimer(); 
                return;
            }
        }

        private void ManageGravity()
        {
            //Hard falling
            if (player.RB.linearVelocity.y < 0 && player.MoveInput.y < 0)
            {
                IsHardFalling = true;
                player.SetGravityScale(Data.gravityScale * Data.fastFallGravityMultiple);
                player.RB.linearVelocity = 
                    new Vector2(player.RB.linearVelocity.x, Mathf.Max(player.RB.linearVelocity.y, - Data.maxFallSpeed));
                return;
            }
            //After a jump perform
            if(IsJumpCut)
            {
                player.SetGravityScale(Data.gravityScale * Data.jumpCutGravityMultiple);
                player.RB.linearVelocity =
                    new Vector2(player.RB.linearVelocity.x, Mathf.Max(player.RB.linearVelocity.y, -Data.maxFallSpeed));
                return;
            }
            if((player.IsJumping || player.IsFalling || IsWallJumping) && Mathf.Abs(player.RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
            {
                player.SetGravityScale(Data.gravityScale * Data.jumpCutGravityMultiple);
            }
            if(player.IsFalling)
            {
                player.SetGravityScale(Data.gravityScale * Data.fallGravityMultiple);
                player.RB.linearVelocity =
                    new Vector2(player.RB.linearVelocity.x, Mathf.Max(player.RB.linearVelocity.y, -Data.maxFallSpeed));
                return;
            }

            player.SetGravityScale(Data.gravityScale);
        }
    }
}
