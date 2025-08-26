using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;
using static UnityEditor.Progress;
using Unity.VisualScripting;

// Draggable game object that is part of a scene
// * upon drag creates a UI item-copy of an object that can be placed in Inventory
// * sends calls to UI widgets that can handle drop events
public class DraggableObjectItem : MonoBehaviour
{
    private GameObject dragIcon;
    private RectTransform dragIconRect;
    private bool isDragging = false;

    public void OnMouseDownHandle()
    {
        Debug.Log("Begin Object drag");
        isDragging = true;

        dragIcon = CreateUIItem();
        dragIcon.GetComponent<DraggableUIItem>().OnUIPick(dragIcon.GetComponent<Image>());

        // Save ref for drag object rect to handle move logic
        dragIconRect = dragIcon.GetComponent<RectTransform>();

        // Hide scene object during drag
        SetVisible(false);
    }

    public void OnMouseUpHandle()
    {
        Debug.Log("End Object drag");

        bool isUIHit = SendRaycastEvent(dragIcon);
        if (isUIHit)
        {
            dragIcon.GetComponent<DraggableUIItem>().OnUIDrop();
        }
        else
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            SetVisible(true);
            Destroy(dragIcon);
        }

        isDragging = false;
    }

    private void Update()
    {
        if (isDragging)
        {
            OnMouseDrag();
        }
    }

    void OnMouseDrag()
    {
        Debug.Log("Dragging Object");

        if (dragIconRect != null)
        {
            dragIconRect.position = Input.mousePosition;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        }
    }


    // Creates and inits an UI item from a scene object that we can
    // drag over inventory UI
    private GameObject CreateUIItem()
    {
        GameObject UIItem = new GameObject("DragIcon", typeof(RectTransform), typeof(Image), typeof(DraggableUIItem), typeof(PickupItem));
        UIItem.transform.SetParent(GameObject.Find("Canvas").transform, false);        

        var spriteRenderer = GetComponent<SpriteRenderer>();
        UIItem.GetComponent<Image>().sprite = spriteRenderer.sprite;
        UIItem.GetComponent<Image>().color = spriteRenderer.color;

        UIItem.GetComponent<RectTransform>().position = transform.position;
        UIItem.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);

        var pickupComp = GetComponent<PickupItem>();
        UIItem.GetComponent<PickupItem>().ItemDescription = pickupComp.ItemDescription;

        return UIItem;
    }

    bool SendRaycastEvent(GameObject dragObject)
    {
        // Cast a ray to find the UI element under the mouse
        bool isUIHit = false;
        PickupItem pickupComp = GetComponent<PickupItem>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        pointerData.pointerDrag = dragObject;

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            var dropSlot = result.gameObject.GetComponent<InventorySlot>();
            if (dropSlot != null && dropSlot.IsValidSlot(pickupComp.ItemDescription))
            {
                // Manually call OnDrop
                dropSlot.OnDrop(pointerData);
                Debug.Log("OnDrop triggered on " + result.gameObject.name);
                isUIHit = true;
                break;
            }
        }

        return isUIHit;
    }

    void SetVisible(bool visible)
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = visible;
        }
    }
}
