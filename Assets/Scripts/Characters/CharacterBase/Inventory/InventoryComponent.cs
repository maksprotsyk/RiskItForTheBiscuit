using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Characters.Inventory
{
    public class InventoryComponent : MonoBehaviour
    {
        [SerializeField] private List<ItemDefinition> _startingItems = new();
        private InventoryRuntime _runtime;

        public InventoryRuntime Runtime => _runtime;

        private void Awake()
        {
            var statsHub = GetComponent<CharacterStatsHub>();
            _runtime = new InventoryRuntime(statsHub.Stats);

            foreach (var item in _startingItems)
                _runtime.Equip(item);
        }

        // Unity-friendly API
        public void Equip(ItemDefinition item) => _runtime.Equip(item);
        public void Unequip(ItemDefinition item) => _runtime.Unequip(item);
    }
}