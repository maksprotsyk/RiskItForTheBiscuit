using Items;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class ItemTooltip : MonoBehaviour
{
    private TooltipManager tooltipManager;

    private TooltipManager GetTooltipManagerInstance()
    {
        if (tooltipManager == null)
        {
            tooltipManager = ManagersOwner.GetManager<TooltipManager>();
            if (tooltipManager == null)
            {
                Debug.LogError($"Managers owner doesn't contain {nameof(TooltipManager)}");
            }
        }
        return tooltipManager;
    }

    private void OnMouseEnter()
    {
        PickupItem pickupComponent = GetComponent<PickupItem>();
        if(pickupComponent)
        {
            ItemDefinition itemDesc = pickupComponent.ItemDescription;
            GetTooltipManagerInstance().SetAndShowTooltip(itemDesc);
        }
    }

    private void OnMouseExit()
    {
        GetTooltipManagerInstance().HideTooltip();
    }
}
