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
                return m_isPressed;
            }
            private set
            {
                if (m_isPressed != value)
                {
                    m_isPressed = value;
                    OnButtonStateChanged?.Invoke(m_isPressed);
                }
            }
        }

        private bool m_isPressed = false;

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

