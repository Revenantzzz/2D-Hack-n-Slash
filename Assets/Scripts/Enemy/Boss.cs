using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] Transform _bossBonfire;
    [SerializeField] Enemy _summonedDemon;
    [SerializeField] GameObject _bossRoom;
    [SerializeField] Transform _spawnPos;
    [SerializeField] Transform _bossRoomGate;
    private List<Enemy> _summonedList = new List<Enemy>();
    private Animator _anim;

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < 7; i++)
        {
            _summonedList.Add(Instantiate(_summonedDemon, _spawnPos.position, Quaternion.identity));
        }
        foreach (Enemy enemy in _summonedList)
        {
            enemy.gameObject.SetActive(false);
        }
    }
    protected override void InitializeRoute()
    {
        base.InitializeRoute();
        _anim = GetComponent<Animator>();     
        _bossRoom.SetActive(false );
        _bossBonfire.gameObject.SetActive(false );
    }
    protected override void FixedUpdate()
    {
        if(_healthSystem.IsDead) return;
        PlayerInBossRoom();
        if (!_seenPlayer) return;
        _rb.velocity = new Vector2(MoveVelocity.x, _rb.velocity.y);
        AttackPlayer();
        if(_healthSystem.GetCurrentHeal() >= 0.5f)
        {
            SummonSkill();
            StayAtPoint();
        }
        else
        {
            _isChasingPlayer = true;
            ChasePlayer();
        }      
    }
    protected void PlayerInBossRoom()
    { 
        if(PlayerCombatController.Instance.transform.position.x > _bossRoomGate.position.x)
        {
            _bossRoom.SetActive(true);
            _seenPlayer = true;
        }
    }
    protected void StayAtPoint()
    {
        Debug.Log(this.transform.position.x + "  " + _bossBonfire.transform.position.x);
        if (this.transform.position.x != _bossBonfire.position.x)
        {
            
            this.transform.position = new Vector3(_bossBonfire.transform.position.x, this.transform.position.y,0);
        }
    }
    private bool _canSummon = true;
    private float _summonCooldown = 10f;
    private bool _isSummoning = false;
    public void SummonSkill()
    {
        if (_seenPlayer && _canSummon)
        {
            StartCoroutine(Summon());
        }
    }
    private IEnumerator Summon()
    {
        _canSummon = false;
        _isSummoning = true;

        yield return new WaitForSeconds(2f);
        _anim.SetTrigger("Summon");
        yield return new WaitForSeconds(.5f);
        foreach (Enemy enemy in _summonedList)
        {      
            if(!enemy.gameObject.activeSelf)
            {
                _summonedList.Remove(enemy);
                enemy.transform.position = _spawnPos.position;
                enemy.gameObject.SetActive(true);
                break;
            }          
        }
        _isSummoning = false;
        yield return new WaitForSeconds(_summonCooldown);
        _canSummon = true;
    }
}
