using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCombatController : CombatSystem, IDataPersistence
{
    public static PlayerCombatController Instance { get; private set; }

    public Bonfire LastBonFire ;

    [SerializeField] protected ScriptableStats _stats;
    private PlayerMovementController _playerMovementController;
    private PlayerInput _input;
    private Energy _engergy;

    public bool IsResting { get; private set; }

    public override event UnityAction<int> Attacked; 
    public override event UnityAction<bool> Blocking;
    public override event UnityAction Blocked;
    public override event UnityAction Heal;
    public override event UnityAction Casted;
    public event UnityAction<bool> Charging;
    public override event UnityAction<bool> Dead;
    public event UnityAction Rest;

    [SerializeField] ChargeAttack castAttack;
    private void Awake()
    {
        Instance = this; 
        _input = GetComponent<PlayerInput>();
        _healthSystem = GetComponentInChildren<HealthSystem>();
        _playerMovementController = GetComponent<PlayerMovementController>();
        _engergy = GetComponent<Energy>();

        CurrentATK = 50f;
        CurrentReduceDMG = 1f;
        SetInvincilbe(false);
    }
    private void Start()
    {
        InitializeInput();
        InitializeStats();

        IsResting = false;
        _engergy.SetEnergy(5);
    }
    private void InitializeStats()
    {
        CurrentATK = _stats.AttackDmg;
        CurrentReduceDMG = 1;
    }
    private void InitializeInput()
    {
        _input.Attack += PlayerAttack;
        _input.BlockHeld += PlayerBlockHeld;
        _input.BlockPressed += PlayerBlockPressed;
        _input.Heal += PlayerHeal;
        _input.Interact += PlayerInteract;
        _input.Cast += PlayerCast;
        _input.ChargeCast += PlayerCharging;

        _healthSystem.Damaged += Hurt;
    }

    private void FixedUpdate()
    {
        Dead.Invoke(CheckIsDead());
        if(CheckIsDead()) return;
        if (IsResting) return;
        HandleAttack();
        HandleCast();
        GetInteractObject();
        IsInCombat();

        Dead?.Invoke(_healthSystem.IsDead);
    }

    #region Attack
    private float _lastPressedAttack;
    private void HandleAttack()
    {
        _lastPressedAttack += Time.deltaTime;
        if (_lastPressedAttack > _stats.ComboTime)
        {
            _attackNum = 0;
        }
        if (_attack && _canAttack)
        {
            StartCoroutine(Attack());
            _lastPressedAttack = 0f;
        }
    }
    private void PlayerAttack(bool attack)
    {
        _attack = attack;       
    }
    private IEnumerator Attack()
    {
        _canAttack = false;
        Attacked?.Invoke(_attackNum);
        IsAttacking = true;
        _attackNum++;
        if (_attackNum > 2) _attackNum = 0;


        yield return new WaitForSeconds(_stats.AttackTime);
        IsAttacking = false;

        yield return new WaitForSeconds(_stats.AttackCooldown);
        _canAttack = true;
    }
    #endregion

    #region Cast
    private bool _charge = false;
    private bool _isCharging = false;
    private float _chargeTimer = 0;
    private int _engergyCost = 1;
    public void GainEnergy()
    {
        _engergy.IncreaseEnergy();
    }
    private void HandleCast()
    {
        if (_charge && (_engergy.HaveEnergy || _isCharging))
        {
            _isCharging = true;
            _chargeTimer += Time.deltaTime;
            if (_chargeTimer > (1/castAttack.EnergyPerSecond))
            {
                castAttack.IncreaseAtkScale(1f);
                _engergyCost++;
                _chargeTimer = 0;
            }
        }    
    }
    private void PlayerCast()
    {
        if (_isCharging)
        {
            castAttack.IncreaseAtkScale(_engergyCost);
            _engergy.useEnergy(_engergyCost);
            _isCharging = false;
            StartCoroutine(CastCooldown());
        }
    }
    private void PlayerCharging(bool arg)
    {
        _charge = arg;
        Charging?.Invoke(arg);
    }

    private IEnumerator CastCooldown()
    {
        _canCast = false;
        Casted?.Invoke();
        _isCasting  = true;

        yield return new WaitForSeconds(_stats.CastTime);
        _isCasting = false;

        yield return new WaitForSeconds(_stats.CastCooldown);
        _canCast = true;
    }
    #endregion

    #region Block
    private void PlayerBlockHeld(bool obj)
    {
        float dir = transform.localScale.x;
        Blocking?.Invoke(obj);
        IsBlocking = obj;
    }
    private void PlayerBlockPressed()
    {
        Blocked?.Invoke();
    }
    #endregion

    #region Heal
    private void PlayerHeal()
    {
        if (_canHeal && _engergy.HaveEnergy)
            StartCoroutine(GetHealing());
    }
    private IEnumerator GetHealing()
    {
        Heal?.Invoke();
        _isHealing = true;
        _healthSystem.GetHeal(10);
        _engergy.useEnergy(1);
        _canHeal = false;
        yield return new WaitForSeconds(1);
        _canHeal = true;
        _isHealing = false;
    }
    #endregion

    #region Interact
    float interactDistance = 2f;
    private List<RaycastHit2D> hitList = new List<RaycastHit2D>();
    private ContactFilter2D contactFilter;
    public CanInteractObject InteractingObject;
    private void PlayerInteract()
    {
        if (InteractingObject != null)
        {
            InteractingObject.Interacted();
        }
    }
    private void GetInteractObject()
    {
        contactFilter.layerMask = 12;
        Vector2 dir = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Debug.DrawRay(transform.position, dir, Color.red);
        if (Physics2D.Raycast(transform.position, dir, contactFilter, hitList, interactDistance) > 0)
        {
            foreach (RaycastHit2D hit in hitList)
                if (!hit.transform.gameObject.TryGetComponent(out InteractingObject))
                {
                    InteractingObject = null;
                }
        }
    }
    #endregion

    #region Hurt

    private void Hurt(float arg0)
    {
        if (arg0 <= 0) return;
        HurtTime(.25f);       
    }
    IEnumerator HurtTime(float arg0)
    {
        _canAttack = false;
        _canHeal = false;
        _isHurting = true;
        yield return new WaitForSeconds(arg0);
        _canAttack = true;
        _canHeal = true;
        _isHurting = false;
    }
    #endregion

    public void RestAtCheckpoint()
    {
        IsResting = !IsResting;
        _healthSystem.RestoreHPAtCheckPoint();
        Rest?.Invoke();
    }
    protected override IEnumerator Dying()
    {
        yield return base.Dying();
        transform.position = LastBonFire.transform.position - (Vector3)Vector2.right;
        transform.localScale = Vector3.one;
        RestAtCheckpoint();
        this.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        transform.position = LastBonFire.transform.position - (Vector3)Vector2.right;
        transform.localScale = Vector3.one;
        RestAtCheckpoint();
    }
    public override bool IsInCombat()
    {
        if(_isCharging) return true;
        return base.IsInCombat();
    }

    public void LoadData(GameData gameData)
    {
        this.LastBonFire = gameData.LastCheckPoint;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.LastCheckPoint = this.LastBonFire;
    }
}
