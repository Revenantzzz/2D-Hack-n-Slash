using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyAnim : MonoBehaviour
{
    private Enemy _controller;
    private Animator _anim;
    private HealthSystem _healthSystem;
    private CombatSystem _combatSystem;

    private void Awake()
    {
        _combatSystem = GetComponent<CombatSystem>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _anim = GetComponent<Animator>();
        _controller = GetComponent<Enemy>();
    }
    private void Start()
    {
        _healthSystem.Damaged += GetHit;
        _combatSystem.Dead += Dead;
        _combatSystem.Attacked += Attack;
        _controller.Move += Move;
    }

    private void Attack(int arg0)
    {
        _anim.SetTrigger("Attack");
    }

    private void Move(bool isMoving)
    {
        _anim.SetBool("IsMoving",isMoving);
    }
    private void Dead(bool arg0)
    {
        _anim.SetBool("IsDead", arg0);
    }
    private void GetHit(float arg)
    {
        if (_combatSystem.IsAttacking) return;
        _anim.SetTrigger("Hit");
    }
}
