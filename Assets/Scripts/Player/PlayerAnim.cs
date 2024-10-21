using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnim : MonoBehaviour
{
    private PlayerMovementController _playerMovement;
    private PlayerCombatController _playerCombat;
    private Animator _anim;
    private SpriteRenderer _spriteRenderer;
    private HealthSystem _healthSystem;

    private bool _isGrounded;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovementController>();
        _playerCombat = GetComponentInChildren<PlayerCombatController>();
        _anim = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _healthSystem = GetComponentInChildren<PlayerHealth>();
    }
    private void Start()
    {
        _playerMovement.GroundedChanged += GroundedChanged;
        _playerMovement.Dash += Dashing;

        _playerCombat.Attacked += GroundAttack;
        _playerCombat.Blocked += Blocked;
        _playerCombat.Blocking += Blocking;
        _playerCombat.Heal += Healing;
        _playerCombat.Casted += Cast;
        _playerCombat.Charging += Charging;
        _playerCombat.Dead += OnDead;
        _playerCombat.Rest += Resting;

        _healthSystem.Damaged += Hurt;
    }


    private void FixedUpdate()
    {
        GroundMovement();
        AirMovement();
    }
    private void GroundMovement()
    {
        if(_isGrounded && _playerMovement.PlayerVelocity.x != 0)
        {
            if(_playerMovement.PlayerVelocity.x > 0) _anim.SetInteger("Running",1);
            else if (_playerMovement.PlayerVelocity.x < 0) _anim.SetInteger("Running", -1);
        }
        else
        {
            _anim.SetInteger("Running", 0);
        }
    }
    private void GroundedChanged(bool arg1, float arg2)
    {
        _isGrounded = arg1;
        _anim.SetBool("IsGrounded", _isGrounded);
    }
    private void AirMovement()
    {
        if (!_isGrounded)
            _anim.SetFloat("Jumping", _playerMovement.PlayerVelocity.y);
        else
            _anim.SetFloat("Jumping", 0);
    }
    private void Dashing()
    {
        _anim.SetTrigger("Dash");
    }
    private void GroundAttack(int _attackNum)
    {
        _anim.SetTrigger("Attack");
        _anim.SetInteger("AttackNum", _attackNum);
    }
    private void Cast()
    {
        _anim.SetTrigger("Cast");
    }
    private void Charging(bool isCharging)
    {
        _anim.SetBool("Charging", isCharging);
    }
    private void Blocked()
    {
        
    }
    private void Blocking(bool obj)
    {
        _anim.SetBool("IsBlocking", obj);
    }
    private void Healing()
    {
        Debug.Log("heal");
        _anim.SetTrigger("Heal");
    }
    private void Hurt(float dmg)
    {
        _anim.SetTrigger("Hurt");
    }
    private void OnDead(bool isDead)
    {
        _anim.SetBool("IsDead", isDead);
    }
    private void Resting()
    {
        _anim.SetBool("Resting", !_anim.GetBool("Resting"));
    }
}
