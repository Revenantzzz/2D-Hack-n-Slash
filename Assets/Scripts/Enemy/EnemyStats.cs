using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyStats : ScriptableObject
{
    [Header("Health")]
    [Tooltip("basic Hp")]
    public float Hp = 500f;

    [Header("Movement")]
    public bool CanHaveMovement = true;
    [Tooltip("Movement speed when move normally")]
    public float MoveSpeed = 5f;
    [Tooltip("Movement speed when chase player")]
    public float ChaseSpeed = 7f;
    public float TimeToChangeDis = 1f;

    [Header("Enemy Behavior")]
    public float DetectingRange = 5f;
    public float StopDetectTime = 2.5f;
    public float DetectDelay = .5f;

    [Header("Attack")]
    public bool CanAttackPlayer = true;
    public float Atk = 50f;
    public float AttackRange = 2f;
    public float AttackDelay = 0.5f;
    public float AttackTime = 1f;
    public float AttackCooldown = 05f;

    
}
