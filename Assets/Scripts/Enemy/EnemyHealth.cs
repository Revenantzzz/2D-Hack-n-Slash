using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : HealthSystem
{
    [SerializeField] EnemyStats _stats;
    public override event UnityAction<float> Damaged;
    public override event UnityAction<float> Heal;
    public override event UnityAction<float> IncreaseMaxHP;
    public override event UnityAction<float> LostMaxHP;

    private void Start()
    {
        _hp.GengerateHP(_stats.Hp);
    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.transform.gameObject.TryGetComponent<Attack>(out Attack attack))
            {
                if (_combatSystem.IsBlocking)
                {
                    float dir = attack.transform.position.x - this.transform.position.x >= 0 ? 1 : -1;
                    _blocked = (dir == this.transform.parent.localScale.x);
                }
                float dmgReduced = _blocked ? _combatSystem.CurrentReduceDMG * 10 : _combatSystem.CurrentReduceDMG;
                GetDMG(attack.DamageDeal / dmgReduced);
            }
        }
    }
    public override void GetDMG(float dmg)
    {
        _hp.Damaged(dmg);
        Damaged?.Invoke(dmg);
    }
    public override void GetHeal(float amount)
    {
        _hp.Heal(amount);
        Heal?.Invoke(amount);
    }
    public override void GetIncreaseMaxHP(float amount)
    {
        IncreaseMaxHP?.Invoke(amount);
    }
    public override void GetDecreaseMaxHP(float amount)
    {
        LostMaxHP?.Invoke(amount);
    }
}
