using Characters.Inventory;
using Items;
using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Characters;

// UI widget of an Inventory slot
// * handles drop logic for 'UI-to-UI', 'World-to-UI' interactions
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public static event Action<GameObject> OnItemDropInCell;

    public int row;
    public int column;

    public GameObject slotItem {  get; private set; }

    private InventoryComponent InventoryComp;

    private void Start()
    {
        GameplayManager gameplayManager = ManagersOwner.GetManager<GameplayManager>();
        if (!gameplayManager)
        {
            Debug.LogError("InventorySlot: No GameplayManager found.");
            return;
        }
        CharacterBase playerCharacter = gameplayManager.PlayerController.GetComponent<CharacterBase>();
        if (!playerCharacter)
        {
            Debug.LogError("InventorySlot: No CharacterBase found on PlayerController.");
            return;
        }
        InventoryComp = playerCharacter.Inventory;
    }

    public bool IsValidSlot(ItemDefinition itemToDrop)
    {
        return InventoryComp.UI_IsValidSlot(itemToDrop, row, column);
    }

    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        DraggableUIItem draggableItem = droppedObject.GetComponent<DraggableUIItem>();
        if (!draggableItem)
        {
            // Object is not a draggable type
            return;
        }

        PickupItem pickupComp = droppedObject.GetComponent<PickupItem>();
        bool addedItem = InventoryComp.UI_TryPlace(pickupComp.ItemDescription, row, column);
        if (addedItem)
        {
            draggableItem.parentAfterDrag = transform;
            slotItem = droppedObject;
        }
    }
}
