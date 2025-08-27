using Items;
using Managers;
using UnityEngine;

// A component for all objects that want to display a tooltip
// * sends events to TooltipManager when mouse hover/leave
// * takes ItemDefinition from a hovered item to show propper info
namespace UI
{
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

        public void OnMouseEnterHandle()
        {
            // TODO: need to add ItemDefinition for UI items in Inventory
            PickupItem pickupComponent = GetComponent<PickupItem>();
            if(pickupComponent)
            {
                ItemDefinition itemDesc = pickupComponent.ItemDescription;
                GetTooltipManagerInstance().SetAndShowTooltip(itemDesc);
            }
        }

        public void OnMouseExitHandle()
        {
            GetTooltipManagerInstance().HideTooltip();
        }
    }
}