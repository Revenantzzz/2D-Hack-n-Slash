using UnityEngine;

public abstract class Attack : MonoBehaviour
{
    protected CombatSystem _combatSytem;
    protected float _atk => _combatSytem.CurrentATK;
    [SerializeField]protected float _attackDmgScale = 1f;

    public float DamageDeal { get; protected set; }

    protected void Awake()
    {
        _combatSytem = GetComponentInParent<CombatSystem>();
    }
    protected virtual void Start()
    {
       DamageDeal = _atk* _attackDmgScale;
    }

}
