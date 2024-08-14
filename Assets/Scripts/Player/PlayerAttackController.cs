using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG2D
{
    public class PlayerAttackController : MonoBehaviour
    {
        private PlayerController player;
        [SerializeField] PlayerSettingData data;

        private int attackCount = 0;
        private float attackAddValue = 0;

        CountDownTimer AttackCoolDown;
        CountDownTimer AttackComboCountdown;
        private void Awake()
        {
            player = GetComponent<PlayerController>();
            AttackCoolDown = new CountDownTimer(data.attackCooldown);
            AttackComboCountdown = new CountDownTimer(data.maxComboWaitTime);

            AttackCoolDown.OnTimerStart += () => AttackComboCountdown.StartTimer();
        }
        private void Start()
        {
            player.PlayerAttack += Attack;
            attackAddValue = 1f / (float)(data.maxAttackNumber - 1);
            Debug.Log(attackAddValue);
        }
        private void Update()
        {
            AttackCoolDown.Tick(Time.deltaTime);
            AttackComboCountdown.Tick(Time.deltaTime);
        }
        private void Attack()
        {
            if(!AttackCoolDown.IsRunning)
            {
                if(player.IsGrounded)
                {                   
                    if(AttackComboCountdown.IsRunning)
                    {
                        if (attackCount > 2)
                        {
                            attackCount = 0;
                            AttackComboCountdown.StopTimer();
                        }
                    }
                    else
                    {
                        attackCount = 0;
                    }
                    AttackCoolDown.StartTimer();
                    player.Animator.SetTrigger("Attack");
                    player.Animator.SetFloat("Attack Number", attackCount * attackAddValue);
                    attackCount++;
                }
                else
                {
                    if(player.IsJumping)
                    {
                        AttackCoolDown.StartTimer();
                        attackCount = 0;
                        player.Animator.SetFloat("Jump", 1);
                    }                    
                }              
            }          
        }
    }
}
