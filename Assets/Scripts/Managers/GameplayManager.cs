using UnityEngine;
using Characters.Player;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Items;

namespace Managers
{
    public class GameplayManager : MonoBehaviour
    {
        private LayerMask _targetLayer;
        private PlayerController _playerController;
        private Canvas canvas;
        private GraphicRaycaster raycaster;

        public PlayerController PlayerController
		{
            get
            {
                if (!_playerController)
                {
                    _playerController = FindObjectOfType<PlayerController>();
                }
                return _playerController;
            }
        }

        void Awake()
        {
            _targetLayer = LayerMask.GetMask("Drag");
        }

        private void Start()
        {
            canvas = FindObjectOfType<Canvas>();
            raycaster = canvas.GetComponent<GraphicRaycaster>();
        }

        void Update()
        {
            // Left Mouse Down
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseLeftButtonEvent(true);
            }

            // Left Mouse Up
            if (Input.GetMouseButtonUp(0))
            {
                OnMouseLeftButtonEvent(false);
            }

            // Right Mouse Down
            if (Input.GetMouseButtonDown(1))
            {
                OnMouseRightButtonEvent(true);
            }
        }

        // Triggering custom mouse events for objects on "Drag" layer
        void OnMouseLeftButtonEvent(bool isDown)
        {
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos3D.z = 0f; // Force onto the same plane as 2D colliders
            Vector2 mouseWorldPos = mouseWorldPos3D;

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, _targetLayer);
            if (!hit)
            {
                return;
            }

            DraggableObjectItem dragComponent = hit.GetComponent<DraggableObjectItem>();
            if (!dragComponent)
            {
                return;
            }

            if (isDown)
            {
                dragComponent.OnMouseDownHandle();
            }
            else
            {
                dragComponent.OnMouseUpHandle();
            }
        }

        void OnMouseRightButtonEvent(bool isDown)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerEventData, results);

            foreach (var result in results)
            {
                PickupItem pickupComponent = result.gameObject.GetComponent<PickupItem>();
                if (pickupComponent)
                {
                    pickupComponent.OnMouseRightButton(pointerEventData);
                }
            }
        }
    }

}
