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
    public static readonly Vector2 DefaultPivot = new Vector2(0.0f, 1.0f);
    public static readonly Vector2 DragPivot = new Vector2(0.5f, 0.5f);

    public event Action<PointerEventData> OnBeginDragEvent;
    public event Action<PointerEventData> OnEndDragEvent;
    public event Action<PointerEventData> OnDroppedOutsideCanvas;

    public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("Begin UI drag");
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        OnUIPick(inventoryItemImage);
        OnBeginDragEvent?.Invoke(eventData);
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        // Debug.Log("Dragging UI");
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        Debug.Log("End UI drag");

        OnUIDrop();
        OnEndDragEvent?.Invoke(eventData);

        // Code to check if we dropped outside the UI (on a scene).
        // Currently unused.
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
        GetComponent<RectTransform>().pivot = DragPivot;
        inventoryItemImage.raycastTarget = false;
    }

    public void OnUIDrop()
    {
        transform.SetParent(parentAfterDrag);
        GetComponent<RectTransform>().pivot = DefaultPivot;
        inventoryItemImage.raycastTarget = true;
    }

    private bool SendRaycastUICheck(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
