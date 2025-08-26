using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;
using System;

// Draggable UI widget that exists within UI canvas space
// * sends calls to UI widgets that can handle drop events
// * is used for moving items between UI widgets only (for now,
//   maybe will add 'drop-into-world' logic later)
public class DraggableUIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image inventoryItemImage;
    [HideInInspector] public Transform parentAfterDrag;

    public static event Action<PointerEventData> OnDroppedOutsideCanvas;

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("Begin UI drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        OnUIPick(inventoryItemImage);
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        // Debug.Log("Dragging UI");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("End UI drag");

        OnUIDrop();
        //bool isUIHit = SendRaycastUICheck(eventData);
        //if (isUIHit)
        //{
        //    OnUIDrop();
        //}
        //else
        //{
        //    OnDroppedOutsideCanvas?.Invoke(eventData);
        //}
    }

    public void OnUIPick(Image dragImage)
    {
        if (dragImage && !inventoryItemImage)
        {
            inventoryItemImage = dragImage;
        }
        inventoryItemImage.raycastTarget = false;
    }

    public void OnUIDrop()
    {
        transform.SetParent(parentAfterDrag);
        inventoryItemImage.raycastTarget = true;
    }

    private bool SendRaycastUICheck(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
