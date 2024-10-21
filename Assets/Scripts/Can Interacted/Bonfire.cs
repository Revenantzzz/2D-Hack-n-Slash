using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonfire : CanInteractObject
{
    public override void Interacted()
    {
        PlayerCombatController.Instance.RestAtCheckpoint();
    }
}
