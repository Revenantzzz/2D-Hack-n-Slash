using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [SerializeField]protected EnemyStats _stats;
    protected Rigidbody2D _rb;
    protected EnemyCombat _combat;
    protected HealthSystem _healthSystem;
    protected SpriteRenderer _sprite;
    protected CapsuleCollider2D _enemyCol;

    [SerializeField] protected Transform _firstPoint;
    [SerializeField] protected Transform _secondPoint;
    protected Vector2 _currentPoint;

    protected Vector2 pointA;
    protected Vector2 pointB;

    public bool CanMove => !_combat.IsInCombat();
    public Vector2 MoveVelocity { get; protected set; }

    protected float moveDir;
    protected bool _isWaiting = false;
    protected bool _isChasingPlayer = false;
    protected bool _seenPlayer = false;

    protected float _detectDelayTimer = 0;
    protected float _stopDetectTimer = 0;

    public event UnityAction Attack;
    public event UnityAction<bool> Move;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _combat = GetComponent<EnemyCombat>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _sprite = GetComponent<SpriteRenderer>();
        _enemyCol = GetComponent<CapsuleCollider2D>();
        InitializeRoute();
    }
    protected virtual void InitializeRoute()
    {
        pointA = new Vector2(_firstPoint.position.x, transform.position.y);
        pointB = new Vector2(_secondPoint.position.x, transform.position.y);
        _currentPoint = transform.localScale.x > 0 ? pointB : pointA;
    }
    protected virtual void Start()
    {
        MoveVelocity = Vector2.zero;
        moveDir = transform.localScale.x;
        PlayerCombatController.Instance.Rest += ResetCheckpoint;
    }
    protected virtual void FixedUpdate()
    {
        GetDistanceToPlayer();
        if (_combat.CheckIsDead()) return;
        if (_stats.CanHaveMovement)
        {
            MoveOnRoute();
            _rb.linearVelocity = new Vector2(MoveVelocity.x, _rb.linearVelocity.y);
            ChasePlayer();
            Move?.Invoke((Mathf.Abs(MoveVelocity.x) > 0) && !_combat.IsInCombat());
        }
        if (_stats.CanAttackPlayer)
        {
            FindPlayer();
        }
    }
    protected void MoveToPoint(Vector2 currentTarget, float speed)
    {
        if (!CanMove) return;
        moveDir = currentTarget.x - transform.position.x > 0 ? 1f : -1f;
        MoveVelocity = new Vector2(speed * moveDir, _rb.linearVelocity.y);
    }
    protected virtual void MoveOnRoute()
    {
        if (_isChasingPlayer) return;
        if (_seenPlayer) return;
        if (_isWaiting) return;


        MoveToPoint(_currentPoint, _stats.MoveSpeed);
        if (Mathf.Abs(transform.position.x - _currentPoint.x) < 0.2f)
            StartCoroutine(WaitAtPoint());
    }
    IEnumerator WaitAtPoint()
    {
        _isWaiting = true;
        MoveVelocity = Vector2.zero;
        _currentPoint = (_currentPoint.x == pointA.x) ? pointB : pointA;
        yield return new WaitForSeconds(_stats.TimeToChangeDis);
        _isWaiting = false;

        if (!_seenPlayer)
            transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
    protected void ChasePlayer()
    {
        Vector2 movepos = new Vector2(PlayerCombatController.Instance.transform.position.x - (transform.localScale.x * (_stats.AttackRange - 0.5f)), PlayerCombatController.Instance.transform.position.y);
        if (_isChasingPlayer)
        {
            _isWaiting = false;
            AttackPlayer();
            if(GetDistanceToPlayer() >= _stats.AttackRange)
            MoveToPoint(movepos, _stats.ChaseSpeed);
        }
    }

    protected void AttackPlayer()
    {
        if (GetDistanceToPlayer() < _stats.AttackRange && !_combat.IsAttacking)
        {
            MoveVelocity = Vector2.zero;
            moveDir = PlayerCombatController.Instance.transform.position.x - transform.position.x > 0 ? 1f : -1f;
            transform.localScale = new Vector2(moveDir, transform.localScale.y);
            _combat.HandleAttack();
            StartCoroutine(WaitAtPoint());
        }
    }

    public float GetDistanceToPlayer()
    {
        return Mathf.Abs(PlayerCombatController.Instance.transform.position.x - transform.position.x);
    }
    protected void FindPlayer()
    {
        if (GetDistanceToPlayer() <= _stats.DetectingRange)
        {
            _seenPlayer = true;
            _stopDetectTimer = 0;

            if (_detectDelayTimer > _stats.DetectDelay)
            {
                _detectDelayTimer = 0;
                _isChasingPlayer = true;
            }
            else
            {
                _detectDelayTimer += Time.deltaTime;
            }
        } 
        if (!_seenPlayer) return;
        if (_stopDetectTimer > _stats.StopDetectTime)
        {
            _stopDetectTimer = 0;
            _seenPlayer = false;
            StopChasingPlayer();
        }
        else
        {
            _stopDetectTimer += Time.fixedDeltaTime;
        }

    }
    protected void StopChasingPlayer()
    {
        _isChasingPlayer = false;
        //_currentPoint = Vector2.Distance(this.transform.position, pointA) > Vector2.Distance(this.transform.position, pointB) ? pointA : pointB;
    }

    protected void ResetCheckpoint()
    {
        Debug.Log(PlayerCombatController.Instance.IsResting);

        StopChasingPlayer();
        this.transform.position = new Vector3(pointA.x, this.transform.position.y, this.transform.position.z);
        _currentPoint = pointB;
        transform.localScale = new Vector3(1, 1, 1);
        _isWaiting = false;
        if (!PlayerCombatController.Instance.IsResting)
        {
            _healthSystem.RestoreHPAtCheckPoint();
        }
    }
    protected void OnDisable()
    {
        StopChasingPlayer();
        this.transform.position = new Vector3(pointA.x, this.transform.position.y, this.transform.position.z);
        _currentPoint = pointB;
        _isWaiting = false;
    }
    protected void Onable()
    {
        _healthSystem.RestoreHPAtCheckPoint();
        Debug.Log(_healthSystem.ToString());
    }
}
