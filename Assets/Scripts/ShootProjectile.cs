using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] ProjectileAttack _projectile;
    GameObject attack;
    CombatSystem _combat;

    private void Awake()
    {
        _combat = GetComponentInParent<CombatSystem>();
    }
    private void Start()
    {
        attack = Instantiate(_projectile.gameObject, this.transform.position, Quaternion.identity, this.transform.parent);
        _combat.Attacked += _combat_Attacked;
        attack.SetActive(false);
    }
    private void Update()
    {
        if(attack.transform.parent != this.transform && !attack.gameObject.activeSelf)
        {
            attack.transform.SetParent(transform.parent, false);
        }
    }
    private void _combat_Attacked(int arg0)
    {
        Vector2 shotPos = new Vector2(this.transform.position.x, PlayerCombatController.Instance.transform.position.y);
        attack.transform.position = shotPos;
        _projectile.Dir = (PlayerCombatController.Instance.transform.position - transform.position).normalized;
        attack.transform.SetParent(null);
        //attack.transform.localScale = this.transform.localScale;
        attack.SetActive(true);
    }
}
