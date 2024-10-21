using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    private float _maxHP { get; set; }
    private float _currentHP {  get; set; }
    public float Shield { get; set; }

    private int tempMaxHPLost = 0;

    public bool isAlive => _currentHP > 0;

    public void GengerateHP(float hp)
    {
        _maxHP = hp;
        _currentHP = hp;
    }    
    public float GetCurrentHealth()
    {
        return _currentHP;
    }
    public void GainShield(float amount)
    {
        Shield += amount;
    }
    public void Damaged(float damage)
    {
        Shield = Mathf.Max(0, Shield - damage);
        damage = Mathf.Max(0, damage - Shield);
        _currentHP -= damage;
    }
    public void Heal (float healAmount)
    {
        _currentHP = Mathf.Min(_maxHP, _currentHP+healAmount);
    }
    public void IncreaseMaxHP(int amount)
    {
        _maxHP += amount;
        _currentHP += amount;   
    }    
    public void Cursed(int lostHP, int curseTime)
    {
        StartCoroutine(TemporaryLoseMaxHP(lostHP, curseTime));
    }
    private IEnumerator TemporaryLoseMaxHP(int lostHP, int curseTime)
    {
        tempMaxHPLost += lostHP;
        _maxHP = Mathf.Max(1, _maxHP - lostHP);
        _currentHP = Mathf.Min(_currentHP, _maxHP);
        yield return new WaitForSeconds(curseTime);
        _maxHP += tempMaxHPLost;
        tempMaxHPLost = 0;
    }
    public float GetHealthBarValue()
    {
        return _currentHP / _maxHP;
    }
    public void RestoreHealth()
    {
        _currentHP = _maxHP;
    }
}
