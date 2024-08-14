using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG2D
{
    [CreateAssetMenu(fileName = "Player Setting Data")]
    public class PlayerSettingData : ScriptableObject
    {
        [Header("Player Size")]
        public float playerHeight;
        public float playerWidth;
        [Space(10)]

        [Header("Movement Setting")]
        public float maxSpeed; //Move max speed
        public float runAccel; //Acceleration when move 
        public float runDeccel;
        [Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
        [Range(0f, 1)] public float deccelInAir; //Multipliers applied to decceleration rate when airborne.
        public bool conserveMomentum = true;
        [Space(10)]

        [Header("Jump up Setting")]
        public float jumpHeight; //Max jump height
        public float jumpDuration; //After press jump,while jumpduration if player met requiment for jump (such as is on ground) Player will jump
        public float jumpCoolDown; // Cooldown for single jump
        public float maxFallSpeed;
        [Space(10)]

        [Header("Wall Jump Setting")]
        public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
        [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
        [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
        public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction
        [Space(10)]

        [Header("All Jump Setting")]
        [Range(0f, .5f)]public float jumpCoyoteTime;//Time after leave wall that we can perform jump
        public float jumpHangTimeThreshold;  
        public float jumpHangAccelerationMultiple;
        public float jumpHangMaxSpeedMultiple;
        [Space(10)]

        [Header("Gravity Multiple")]
        public float fastFallGravityMultiple; // Higher gravity when perform a hard land
        public float jumpCutGravityMultiple; //Higher gravity after perform a jump
        public float fallGravityMultiple; // gravity when falling
        [Range(0f, 1)] public float jumpHangGravityMultiple;
        [Space(10)]

        [HideInInspector] public float jumpForce;
        [HideInInspector] public float gravityScale;
        [HideInInspector] public float gravityStrength;

        [Header("Dash")]
        public float dashDuration;
        public float dashCooldown;
        public int dashAmount;
        public float dashSpeed;
        public float dashSleepTime; //Duration for which the game freezes when we press dash but before we read directional input and apply a force
        public float dashAttackTime;
        public float dashEndTime; //Time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
        public Vector2 dashEndSpeed; //Slows down player, makes dash feel more responsive (used in Celeste)
        [Range(0f, 1f)] public float dashEndRunLerp; //Slows the affect of player movement while dashing
        [HideInInspector] public float dashForce;

        [Header("Attack")]
        public float attackCooldown;
        public float maxAttackNumber;
        public float maxComboWaitTime;
        public void SetGravityData()
        {
            //free fall formula h = 1/2gt^2 => g = 2h / t^2
            gravityStrength = -(2 * jumpHeight) / (jumpDuration * jumpDuration);
            gravityScale = gravityStrength / Physics2D.gravity.y;
            jumpForce = Mathf.Abs(gravityStrength) * jumpDuration;
            dashForce = dashAmount / dashDuration;
        }

    }
}
