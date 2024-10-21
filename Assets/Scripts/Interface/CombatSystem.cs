using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CombatSystem : MonoBehaviour
{
    protected HealthSystem _healthSystem;

    protected bool _canAttack = true;
    protected int _attackNum;
    protected bool _attack = false;

    protected bool _canCast = true;
    
    protected bool _cast = false;
    protected bool _canHeal = true;

    public bool IsAttacking { get; protected set; }
    protected bool _isCasting = false;
    protected bool _isBlocking = false;
    protected bool _isHealing = false;
    protected bool _isHurting = false;

    public float CurrentATK { get; protected set; }
    public float CurrentReduceDMG { get; protected set; } 
    public bool IsInvincible { get; protected set; }
    public bool IsBlocking { get; protected set; }


    public abstract event UnityAction<int> Attacked;
    public abstract event UnityAction<bool> Blocking;
    public abstract event UnityAction Blocked;
    public abstract event UnityAction Heal;
    public abstract event UnityAction Casted;

    public abstract event UnityAction<bool> Dead;

    public void SetInvincilbe(bool invincilbe)
    {
        IsInvincible = invincilbe;
    }
    public virtual bool IsInCombat()
    {
        if (IsAttacking) return true;
        if (_isBlocking) return true;
        if (_isHealing) return true;
        if (_isHurting) return true;
        if (_isCasting) return true;
        return false;
    }
    protected bool _isDying = false;
    public bool CheckIsDead()
    {
        if (_healthSystem.IsDead && !_isDying)
        {
            StartCoroutine(Dying());
        }
        return _healthSystem.IsDead;
    }
    protected virtual IEnumerator Dying()
    {
        _isDying = true;
        yield return new WaitForSeconds(2f);
        _isDying = false;
    }
}
