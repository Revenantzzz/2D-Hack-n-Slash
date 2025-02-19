using System.Collections.Generic;
using UnityEngine;

public class EnemyDisable : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies = new List<Enemy>();

    void Start()
    {
        PlayerCombatController.Instance.Rest += Rest;
    }
    private void Rest()
    {
        foreach(Enemy enemy in enemies)
        {
            if(PlayerCombatController.Instance.IsResting)
            {
                enemy.gameObject.SetActive(false);
            }
            else
            {
                enemy.gameObject.SetActive(true);
            }
        }
    }
}
