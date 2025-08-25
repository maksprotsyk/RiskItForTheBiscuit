using Characters.Inventory;
using Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// UI widget of an Inventory slot
// * handles drop logic for 'UI-to-UI', 'World-to-UI' interactions
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public static event Action<GameObject> OnItemDropInCell;

    public int row;
    public int column;

    public GameObject slotItem {  get; private set; }

    private InventoryComponent InventoryComp;

    void Awake()
    {
        InventoryComp = FindObjectOfType<InventoryComponent>();
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
