using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    private int maxEnergy;
    private int currentEnergy;
    [SerializeField] List<Slider> EnergyIconList = new List<Slider>();

    public void SetEnergy(int energy)
    {
        maxEnergy = energy;
        currentEnergy = energy;
        SetEnergyIcon();
    }
    public void useEnergy(int amount)
    {
        if (currentEnergy > 0)
        {
            currentEnergy = (currentEnergy - amount) > 0 ? (currentEnergy - amount) : 0;
        }
        SetEnergyIcon();
    }
    public void IncreaseEnergy()
    {

        if (currentEnergy < EnergyIconList.Count) currentEnergy++;
        SetEnergyIcon();
    }
    public bool HaveEnergy => currentEnergy > 0;

    private void FixedUpdate()
    {
        //SetEnergyIcon();
    }
    private void SetEnergyIcon()
    {
        if (currentEnergy < EnergyIconList.Count)
        {
            for (int i = currentEnergy; i < EnergyIconList.Count; i++)
            {
                EnergyIconList[i].value = 0;
            }
            return;
        }
        //if (currentEnergy == 0 || currentEnergy == EnergyIconList.Count) return;
        for (int i = 0; i < currentEnergy; i++)
        {
            EnergyIconList[i].value = 1;
        }
    }
}
