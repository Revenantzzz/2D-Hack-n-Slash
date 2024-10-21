using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class EnemyCombat : CombatSystem
{
    [SerializeField] EnemyStats _stats;
    public override event UnityAction<int> Attacked;
    public override event UnityAction<bool> Blocking;
    public override event UnityAction Blocked;
    public override event UnityAction Heal;
    public override event UnityAction Casted;
    public override event UnityAction<bool> Dead;

    public bool IsAttacking => base.IsAttacking;
    private void Awake()
    {
        _healthSystem = GetComponentInChildren<HealthSystem>();

        CurrentATK = _stats.Atk;
        CurrentReduceDMG = 1f;
    }
    private void Start()
    {
        //_healthSystem.Damaged += GetDamaged;
    }
    private void FixedUpdate()
    {
        Dead?.Invoke(CheckIsDead());    
        if(CheckIsDead()) return;
        IsInCombat();
    }
    private void GetDamaged(float arg0)
    {
        StartCoroutine(DamagedRecover());
    }
    IEnumerator DamagedRecover()
    {
        _canAttack = false;
        _canCast = false;
        _canHeal = false;
        _isHurting = true;
        yield return new WaitForSeconds(.1f);
        if(!base.IsAttacking) _canAttack = true;
        _canCast = true;
        _canHeal = true;
        _isHurting = false;
    }

    public void HandleAttack()
    {
        if(_canAttack)
        {
            StartCoroutine(Attack());
        }      
    }
    private IEnumerator Attack()
    {
        base.IsAttacking = true; 
        _canAttack = false;
        yield return new WaitForSeconds(_stats.AttackDelay);  
        Attacked?.Invoke(_attackNum);
        
        _attackNum++;
        if (_attackNum > 2) _attackNum = 0;

        yield return new WaitForSeconds(_stats.AttackTime);
        base.IsAttacking = false;

        yield return new WaitForSeconds(_stats.AttackCooldown);
        if(!_isHurting) _canAttack = true;
    }
    protected override IEnumerator Dying()
    {
        _isDying = true;
        PlayerCombatController.Instance.GainEnergy();
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        _isDying = false;
    }
}
