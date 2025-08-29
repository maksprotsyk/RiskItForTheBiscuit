using Characters;
using Characters.Inventory;
using Managers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

// Object that holds equipable item description
// * does nothing else for now
namespace Items
{
    public class PickupItem : MonoBehaviour
    {
        public event Action OnItemConsumeEvent;

        public ItemDefinition ItemDescription;

        public void OnMouseRightButton(PointerEventData eventData)
        {
            OnItemConsumeEvent?.Invoke();
        }
    }
}
