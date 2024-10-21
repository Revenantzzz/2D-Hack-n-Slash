using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : Attack
{
    public float EnergyPerSecond;
    public void IncreaseAtkScale(float dmg)
    {
        _attackDmgScale = dmg;
    }
}
