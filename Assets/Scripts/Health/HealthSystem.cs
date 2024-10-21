using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class HealthSystem : MonoBehaviour
{
    [SerializeField] public Slider healthBar;
    protected CapsuleCollider2D hitbox;
    protected HP _hp;
    protected CombatSystem _combatSystem;

    public abstract event UnityAction<float> Damaged;
    public abstract event UnityAction<float> Heal;
    public abstract event UnityAction<float> IncreaseMaxHP;
    public abstract event UnityAction<float> LostMaxHP;

    public float Atk => _combatSystem.CurrentATK;
    public bool IsDead => !_hp.isAlive;

    protected bool _blocked;

    protected void Awake()
    {
        _combatSystem = GetComponentInParent<CombatSystem>();
        _hp = GetComponent<HP>();
        hitbox = GetComponent<CapsuleCollider2D>();
    }
    protected void Update()
    {
        SetHealthbarValue();
    }
 
    protected void SetHealthbarValue()
    {
       healthBar.value = _hp.GetHealthBarValue();
    }
    public abstract void GetDMG(float dmg);
    public abstract void GetHeal(float amount);
    public abstract void GetIncreaseMaxHP(float amount);
    public abstract void GetDecreaseMaxHP(float amount);
    public void RestoreHPAtCheckPoint()
    {
        _hp.RestoreHealth();
    }
    public float GetCurrentHeal()
    {
        return _hp.GetHealthBarValue();
    }
}
