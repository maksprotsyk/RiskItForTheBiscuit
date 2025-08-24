using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class TrackableUIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event System.Action<bool> OnButtonStateChanged;
        public bool IsPressed
        { 
            get
            {
                return _isPressed;
            }
            private set
            {
                if (_isPressed != value)
                {
                    _isPressed = value;
                    OnButtonStateChanged?.Invoke(_isPressed);
                }
            }
        }

        private bool _isPressed = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
        }
    }
}

