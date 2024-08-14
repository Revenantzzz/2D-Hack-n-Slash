using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace RPG2D
{
    public class PlayerDashController : MonoBehaviour
    {
        [SerializeField]PlayerSettingData Data;
        PlayerMovementController movement;
        PlayerController player;

        public bool IsDashing => isDashingTimer.IsRunning;
        public bool IsEndDashing => isDashingTimer.IsRunning && isDashingTimer.progress < .5f;

        List<Timer> timerList = new List<Timer>();
        CountDownTimer dashTimer;
        CountDownTimer isDashingTimer;
        CountDownTimer dashCoolDownTimer;

        private void Awake()
        {
            movement = GetComponent<PlayerMovementController>();
            player = GetComponent<PlayerController>();

            dashTimer = new CountDownTimer(Data.dashDuration);
            dashCoolDownTimer = new CountDownTimer(Data.dashCooldown);
            isDashingTimer = new CountDownTimer(Data.dashDuration);
            timerList = new List<Timer>(capacity: 3) { dashTimer, dashCoolDownTimer, isDashingTimer};
            isDashingTimer.OnTimerStop += () => dashCoolDownTimer.StartTimer();
            dashTimer.OnTimerStart += () => isDashingTimer.StartTimer();
        }
        private void Start()
        {
            player.PlayerDash += Dash;
        }
        private void Update()
        {
            HandleTimer();
            HandleDash();
        }
        private void HandleTimer()
        {
            foreach (Timer timer in timerList)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        private bool CanDash()
        {
            return true;
        }
        private void Dash()
        {
            if(CanDash() && !dashTimer.IsRunning && !dashCoolDownTimer.IsRunning && !isDashingTimer.IsRunning)
            {                
                dashTimer.StartTimer();  
            }
        }
        private void HandleDash()
        {          

            if(!dashTimer.IsRunning)
            {
                player.SetGravityScale(Data.gravityScale);
            }
            if (dashTimer.IsRunning)
            {               
                player.Animator.SetTrigger("Dash");
                player.SetGravityScale(0);
                Vector2 dir = Vector2.right;
                if (!movement.FacingRight)
                {
                    dir = Vector2.left;
                }
                float force = Data.dashForce;               
                player.RB.AddForce(dir * force, ForceMode2D.Impulse);
                dashTimer.StopTimer();
            }
        }
    }
}
