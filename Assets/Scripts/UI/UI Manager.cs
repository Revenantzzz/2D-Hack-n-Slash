using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerHUD;
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject DeadPanel;
    [SerializeField] GameObject UpgradePanel;
    [SerializeField] GameObject PausePanel;
    private void Awake()
    {
        PlayerHUD.SetActive(false);
        StartLevel();
    }

    private void Start()
    {
        PlayerCombatController.Instance.Dead += Instance_Dead;
    }

    private void StartLevel()
    {
        Time.timeScale = 0f;
    }
    public void ContinuteButton()
    {
        Time.timeScale = 1f;
        PlayerHUD.SetActive(true);
        StartPanel.SetActive(false);
        PausePanel.SetActive(false);
    }
    private void Instance_Dead(bool arg0)
    {
        if(arg0)
        {
            DeadPanel.SetActive(true);
        }
    }
}
