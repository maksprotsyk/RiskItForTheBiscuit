using System;
using Managers;
using UnityEngine;

namespace UI
{
    public class BaseUIWindow : MonoBehaviour, IUIWindow
    {
        private UIManager m_uiManager;

        public virtual void Init(object data)
        {
        }

        public virtual void Show(Action onShowComplete)
        {
            gameObject.SetActive(true);
            onShowComplete?.Invoke();
        }

        public virtual void Hide(Action onHideComplete)
        {
            onHideComplete?.Invoke();
            gameObject.SetActive(false);
        }

        protected UIManager UserInterfaceManager
        {
            get
            {
                if (m_uiManager == null)
                {
                    m_uiManager = ManagersOwner.GetManager<UIManager>();
                    if (m_uiManager == null)
                    {
                        Debug.LogError($"Managers owner doesn't contain {nameof(UIManager)}");
                    }
                }
                return m_uiManager;
            }
        }
    }
}
