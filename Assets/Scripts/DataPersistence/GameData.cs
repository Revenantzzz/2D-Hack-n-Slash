using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData 
{
    public long lastUpdated;
    public Bonfire LastCheckPoint;
    public AtributeData playerUpgradeData;

    public GameData(Bonfire bonfire)
    {
        LastCheckPoint = bonfire;
        playerUpgradeData = new AtributeData();
    }
}
