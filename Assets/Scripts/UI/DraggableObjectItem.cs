using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Data;

public class DraggableObjectItem : MonoBehaviour
{
    private GameObject dragIcon;
    private RectTransform dragIconRect;
    private bool isDragging = false;

    void OnMouseDown()
    {
        Debug.Log("Begin Object drag");
        isDragging = true;

        dragIcon = new GameObject("DragIcon", typeof(RectTransform), typeof(Image), typeof(DraggableUIItem));
        dragIconRect = dragIcon.GetComponent<RectTransform>();
        dragIcon.transform.SetParent(GameObject.Find("Canvas").transform, false);

        var sr = GetComponent<SpriteRenderer>();
        dragIcon.GetComponent<Image>().sprite = sr.sprite;
        dragIcon.GetComponent<Image>().color = sr.color;

        dragIcon.GetComponent<RectTransform>().position = transform.position;
        dragIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);

        dragIcon.GetComponent<DraggableUIItem>().OnUIPick(dragIcon.GetComponent<Image>());
        SetVisible(false);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Debug.Log("Dragging Object");

            if (dragIconRect != null)
                dragIconRect.position = Input.mousePosition;
        }
    }

    void OnMouseUp()
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

    bool SendRaycastEvent(GameObject dragObject)
    {
        // Cast a ray to find the UI element under the mouse
        bool isUIHit = false;
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        pointerData.pointerDrag = dragObject;

        var raycastResults = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            var dropHandler = result.gameObject.GetComponent<IDropHandler>();
            if (dropHandler != null)
            {
                // Manually call OnDrop
                dropHandler.OnDrop(pointerData);
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
