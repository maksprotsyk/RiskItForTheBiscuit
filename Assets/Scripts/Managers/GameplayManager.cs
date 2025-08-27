using UnityEngine;
using Characters.Player;

namespace Managers
{
    public class GameplayManager : MonoBehaviour
    {
        private LayerMask _targetLayer;
        private PlayerController _playerController;
		
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
    }

}
