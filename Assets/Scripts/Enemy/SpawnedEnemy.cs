using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedEnemy : Enemy
{
    protected override void Start()
    {
        base.Start();
        _seenPlayer = true;
        _isChasingPlayer = true;
        ChasePlayer();
    }
    protected override void FixedUpdate()
    {
        GetDistanceToPlayer();
        if (_combat.CheckIsDead()) return;
        _rb.linearVelocity = new Vector2(MoveVelocity.x, _rb.linearVelocity.y);
        if (!this.gameObject.activeSelf) return;
        _seenPlayer = true;
        _isChasingPlayer = true;
        ChasePlayer();
    }
}
