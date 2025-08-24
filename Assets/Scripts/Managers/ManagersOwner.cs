using UnityEngine;

namespace Managers
{
    public class ManagersOwner : MonoBehaviour
    {
        public static ManagersOwner Instance { get; private set; } = null;

        public static T GetManager<T>() where T : Component
        {
            return Instance.GetComponent<T>();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}