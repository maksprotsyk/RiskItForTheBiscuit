using UnityEngine;
using Characters.Player;
using System.Reflection;
using Characters.Inventory;

namespace Managers
{

    public class GameplayManager : MonoBehaviour
    {
        private LayerMask targetLayer;
        private PlayerController _playerController;
		
        public PlayerController PlayerController
		{
            get
            {
                if (_playerController == null)
                {
                    _playerController = FindObjectOfType<PlayerController>();
                }
                return _playerController;
            }
        }

        void Awake()
        {
            targetLayer = LayerMask.GetMask("Drag");
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
        }

        // Triggering custom mouse events for objects on "Drag" layer
        void OnMouseLeftButtonEvent(bool IsDown)
        {
            Vector3 mouseWorldPos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos3D.z = 0f; // Force onto the same plane as 2D colliders
            Vector2 mouseWorldPos = mouseWorldPos3D;

            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, targetLayer);
            if (!hit)
            {
                return;
            }

            DraggableObjectItem DragComponent = hit.GetComponent<DraggableObjectItem>();
            if (!DragComponent)
            {
                return;
            }

            if (IsDown)
            {
                DragComponent.OnMouseDownHandle();
            }
            else
            {
                DragComponent.OnMouseUpHandle();
            }
        }
    }

}
