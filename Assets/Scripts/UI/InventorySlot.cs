using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(UnityEngine.EventSystems.PointerEventData eventData)
    {
        GameObject droppedObject = eventData.pointerDrag;
        DraggableUIItem draggableItem = droppedObject.GetComponent<DraggableUIItem>();
        draggableItem.parentAfterDrag = transform;
    }
}
