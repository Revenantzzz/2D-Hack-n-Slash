using UnityEngine;
using UnityEngine.UI;

public abstract class CanInteractObject : MonoBehaviour
{
    [SerializeField] Image selected;
    public abstract void Interacted();

    private void FixedUpdate()
    {
        SetSelected();
    }
    private void SetSelected()
    {
        if (PlayerCombatController.Instance == null || PlayerCombatController.Instance.IsResting) return;
        
        if(PlayerCombatController.Instance.InteractingObject == this)
        {
            if(!selected.gameObject.activeSelf)
            selected.gameObject.SetActive(true);
        }
        else
        {
            if(selected.gameObject.activeSelf)
            selected.gameObject.SetActive(false);
        }
    }
}
