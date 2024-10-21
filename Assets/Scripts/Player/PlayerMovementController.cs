using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private ScriptableStats _stats;
    private PlayerCombatController _playerCombat;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private PlayerInput _input;

    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    private bool _isDead => _playerCombat.CheckIsDead();
    public event UnityAction<bool, float> GroundedChanged;
    public event UnityAction Jumped;
    public event UnityAction Dash;

    private float _time;

    private void Awake()
    { 
        _playerCombat = GetComponent<PlayerCombatController>();
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<PlayerInput>();
        _col = GetComponent<CapsuleCollider2D>();
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }
    private void Start()
    {
        InitializeInput();
    }
    private void InitializeInput()
    {
        _input.JumpHeld += PlayerJumpHeld;
        _input.JumpPressed += PlayerJumpPressed;
        _input.Dash += PlayerDash;

    }

    private void Update()
    {
        _time += Time.deltaTime;
        HandleMoveInput();
        _jumpPressed = false;
    }
 
    private void FixedUpdate()
    {
        if (_isDead) return;
        if (_playerCombat.IsResting) return;
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    #region Horizontal Move

    private Vector2 _move;
    public bool CanMove => !_playerCombat.IsInCombat();
    public Vector2 PlayerVelocity => _rb.velocity;

    private bool _isFacingRight = true;
    private void HandleMoveInput()
    {
        _move = _input.MoveVector;
        _move.x = Mathf.Abs(_move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_move.x);
        _move.y = Mathf.Abs(_move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_move.y);
    }
    private void HandleDirection()
    {
        if (_move.x == 0 || !CanMove)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
        }
        if (_isFacingRight && _move.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            _isFacingRight = false;
        }
        else if (!_isFacingRight && _move.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            _isFacingRight = true;
        }
    }

    #endregion

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion

    #region Jumping

    private bool _jumpPressed;
    private bool _jumpHeld;
    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private void PlayerJumpPressed()
    {
        _jumpPressed = true;
        _jumpToConsume = true;
        _timeJumpWasPressed = _time;
    }

    private void PlayerJumpHeld(bool pressed)
    {
        _jumpHeld = pressed;
    }

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_jumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        if (!CanMove) return;
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region  Dash
    private bool _isDashing = false;
    private bool _canDash = true;

    private void PlayerDash()
    {
        if (_canDash && _grounded && CanMove)
        {
            StartCoroutine(Dashing());
        }
    }
    private IEnumerator Dashing()
    {
        _canDash = false;
        _isDashing = true;
        _playerCombat.SetInvincilbe(true);
        Dash?.Invoke();
        yield return new WaitForSeconds(0.1f);

        float orginGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;
        _frameVelocity = new Vector2(transform.localScale.x * _stats.DashForce, 0f);
        
        yield return new WaitForSeconds(_stats.DashingTime - .1f);
        _isDashing = false;
        _rb.gravityScale = orginGravity;
        _playerCombat.SetInvincilbe(false);

        yield return new WaitForSeconds(_stats.DashingCooldown - .1f);
        _canDash = true;
    }
    #endregion

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

}


