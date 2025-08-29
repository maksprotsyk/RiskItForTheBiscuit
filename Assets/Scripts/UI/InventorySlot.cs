using Characters.Inventory;
using Items;
using System;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using Characters;
using static UnityEditor.Progress;
using System.Collections.Generic;
using UnityEngine.UI;

// UI widget of an Inventory slot
// * handles drop logic for 'UI-to-UI', 'World-to-UI' interactions
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public static event Action<GameObject> OnItemDropInCell;

    public int row;
    public int column;

    public GameObject slotItem {  get; private set; }

    private InventoryComponent InventoryComp;
    private Vector3 slotItemOrigPosition;
    private bool isCurrentItemInDrag = false;
    
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

        // Check if we already have item in slot at the start
        if (transform.childCount > 0)
        {
            Transform itemTransform = transform.GetChild(0);
            slotItem = itemTransform.gameObject;
            slotItem.GetComponent<DraggableUIItem>().OnBeginDragEvent += HandleObjectBeginDrag;
            slotItem.GetComponent<DraggableUIItem>().OnEndDragEvent += HandleObjectEndDrag;
            slotItem.GetComponent<PickupItem>().OnItemConsumeEvent += HandleObjectConsume;

            bool isPlaced = InventoryComp.UI_TryPlace(slotItem.GetComponent<PickupItem>().ItemDescription, row, column);
            if (!isPlaced)
            {
                Debug.LogError("Wrong type of item is placed in the slot[" + row + "," + column + "]");
            }
        }
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
            slotItem.GetComponent<DraggableUIItem>().OnBeginDragEvent += HandleObjectBeginDrag;
            slotItem.GetComponent<DraggableUIItem>().OnEndDragEvent += HandleObjectEndDrag;
            pickupComp.OnItemConsumeEvent += HandleObjectConsume;
            Debug.Log("Successfully placed an item in the slot[" + row + "," + column + "]");
        }
        else
        {
            Debug.Log("Failed to place an item in the slot[" + row + "," + column + "]");
        }
    }

    void HandleObjectBeginDrag(PointerEventData eventData)
    {
        if (IsPointerOver(eventData))
        {
            isCurrentItemInDrag = true;
            slotItemOrigPosition = slotItem.transform.position;
        }
    }

    void HandleObjectEndDrag(PointerEventData eventData)
    {
        if (isCurrentItemInDrag && IsPointerOver(eventData) == false)
        {
            if (!slotItem.transform.IsChildOf(transform))
            {
                // We moved the item to another slot
                slotItem.GetComponent<DraggableUIItem>().OnBeginDragEvent -= HandleObjectBeginDrag;
                slotItem.GetComponent<DraggableUIItem>().OnEndDragEvent -= HandleObjectEndDrag;
                slotItem.GetComponent<PickupItem>().OnItemConsumeEvent -= HandleObjectConsume;

                // Check which slot we dropped on
                GraphicRaycaster raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
                List<RaycastResult> results = new List<RaycastResult>();
                raycaster.Raycast(eventData, results);

                InventorySlot targetInventorySlot = null;
                foreach (RaycastResult result in results)
                {
                    InventorySlot invSlot = result.gameObject.GetComponent<InventorySlot>();
                    if (invSlot)
                    {
                        targetInventorySlot = invSlot;                        
                        break;
                    }
                }

                if (targetInventorySlot)
                {
                    // Found 'to-move' slot, drop object into it
                    bool isRemoved = InventoryComp.UI_TryRemove(row, column);
                    if (isRemoved)
                    {
                        Debug.Log("Successfully removed an item in the slot[" + row + "," + column + "]");
                    }
                    else
                    {
                        Debug.Log("Failed to remove an item in the slot[" + row + "," + column + "]");
                    }
                }

                slotItem = null;
                //Debug.Log("Moved an item to another slot");
                return;
            }
        }

        isCurrentItemInDrag = false;
        //Debug.Log("Moved an item to same slot");
    }

    void HandleObjectConsume()
    {
        if (InventoryComp.UI_TryConsume(row, column))
        {
            slotItem.GetComponent<DraggableUIItem>().OnBeginDragEvent -= HandleObjectBeginDrag;
            slotItem.GetComponent<DraggableUIItem>().OnEndDragEvent -= HandleObjectEndDrag;
            slotItem.GetComponent<PickupItem>().OnItemConsumeEvent -= HandleObjectConsume;

            bool isRemoved = InventoryComp.UI_TryRemove(row, column);
            if (isRemoved)
            {
                Debug.Log("Successfully removed an item in the slot[" + row + "," + column + "]");
            }
            else
            {
                Debug.Log("Failed to remove an item in the slot[" + row + "," + column + "]");
            }

            // We destroy objects on consume
            Destroy(slotItem);
            slotItem = null;
            Debug.Log("Consumed an item in the slot[" + row + "," + column + "]");
        }
    }

    public bool IsPointerOver(PointerEventData eventData)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera  // for Screen Space - Camera, null if Screen Space - Overlay
        );
    }
}
